using UnityEngine;
using Unity.Netcode;

public class PlayerAbilities : NetworkBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireballCooldown = 1f;
    private float nextFireTime;

    private void Update()
    {
        if (!IsOwner) return;
        
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            CastFireballServerRpc();
        }
    }

    [ServerRpc]
    private void CastFireballServerRpc()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireballCooldown;

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        fireball.GetComponent<Fireball>().SetShooter(OwnerClientId);
        fireball.GetComponent<NetworkObject>().Spawn();
    }
}
