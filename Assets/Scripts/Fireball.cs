using UnityEngine;
using Unity.Netcode;

public class Fireball : NetworkBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float lifetime = 5;
    private Rigidbody rb;

    [SerializeField] private int damage = 20;
    private ulong shooterId;

    public void SetShooter(ulong id)
    {
        shooterId = id;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (!IsSpawned) return;

        var playerHealth = other.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            NetworkObject.Despawn();
            return;
        }

        if (other.gameObject.GetComponent<NetworkObject>().OwnerClientId == shooterId)
        {
            return;
        }

        playerHealth.ApplyDamage(damage);

        NetworkObject.Despawn();
    }

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody>();
        if (IsServer)
        {
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, lifetime);
        }
    }
}
