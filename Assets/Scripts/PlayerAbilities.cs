using UnityEngine;
using Unity.Netcode;

public class PlayerAbilities : NetworkBehaviour
{
    // Abilities available to this player, configured in the Inspector.
    [SerializeField] private AbilityData[] abilities;

    // Transform that determines where and in which direction projectiles are spawned.
    [SerializeField] private Transform firePoint;

    // Stores the next valid cast time for each ability on the server.
    private float[] nextFireTime;

    public override void OnNetworkSpawn()
    {
        // Cooldowns are managed by the server to prevent client-side cheating.
        if (IsServer)
        {
            nextFireTime = new float[abilities.Length];
        }
    }

    private void Update()
    {
        // Only the owning client can provide input, and input is accepted
        // while the cursor is locked for gameplay.
        if (!IsOwner || Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        // Cast the first configured ability when the primary mouse button is pressed.
        if (Input.GetMouseButtonDown(0))
        {
            CastFireballServerRpc(0);
        }
    }

    [ServerRpc]
    private void CastFireballServerRpc(int index)
    {
        // Reject invalid ability indices before accessing the abilities array.
        if (abilities == null || index < 0 || index >= abilities.Length)
        {
            return;
        }

        AbilityData ability = abilities[index];

        // Enforce the ability cooldown on the server.
        if (Time.time < nextFireTime[index])
        {
            return;
        }

        nextFireTime[index] = Time.time + ability.cooldown;

        // Create the projectile at the configured fire point.
        GameObject projectileInstance = Instantiate(
            ability.prefab,
            firePoint.position,
            firePoint.rotation);

        // Configure the projectile with the ability's damage and movement values.
        if (projectileInstance.TryGetComponent<Fireball>(out Fireball fireball))
        {
            fireball.Initialize(
                OwnerClientId,
                ability.damage,
                ability.speed,
                ability.lifetime);
        }

        // Make the projectile visible and synchronized for all clients.
        if (projectileInstance.TryGetComponent<NetworkObject>(
                out NetworkObject networkObject))
        {
            networkObject.Spawn();
        }
    }
}