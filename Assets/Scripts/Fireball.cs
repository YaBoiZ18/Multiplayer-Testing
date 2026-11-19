using UnityEngine;
using Unity.Netcode;

public class Fireball : NetworkBehaviour
{
    // Movement and lifetime settings assigned when the fireball is created.
    private float speed;
    private float lifetime;
    private Rigidbody rb;

    // Damage data used by the server when the fireball hits a player.
    private int damage;
    private ulong shooterId;

    private void OnTriggerEnter(Collider other)
    {
        // Only the server should process collisions and apply damage.
        if (!IsServer) return;

        // Ignore collision events that occur before the fireball is network-spawned.
        if (!IsSpawned) return;

        var playerHealth = other.gameObject.GetComponent<PlayerHealth>();

        // Despawn the fireball when it hits an object that is not a player.
        if (playerHealth == null)
        {
            NetworkObject.Despawn();
            return;
        }

        // Prevent the fireball from damaging the player who fired it.
        if (other.gameObject.GetComponent<NetworkObject>().OwnerClientId == shooterId)
        {
            return;
        }

        // Apply damage on the server, then remove the fireball.
        playerHealth.ApplyDamage(damage);
        NetworkObject.Despawn();
    }

    // Stores the fireball's owner and gameplay settings before it is spawned.
    public void Initialize(ulong shooterId, int damage, float speed, float lifetime)
    {
        this.shooterId = shooterId;
        this.damage = damage;
        this.speed = speed;
        this.lifetime = lifetime;
    }

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody>();

        if (IsServer)
        {
            // Launch the fireball in the direction it is facing.
            rb.linearVelocity = transform.forward * speed;

            // Automatically remove the fireball after its configured lifetime.
            Destroy(gameObject, lifetime);
        }
    }
}
