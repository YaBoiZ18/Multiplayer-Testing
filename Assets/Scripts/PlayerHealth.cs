using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerHealth : NetworkBehaviour
{
    // The maximum health this player starts with.
    [SerializeField] private int maxHealth = 100;

    // Displays the player's current health in the world.
    [SerializeField] private TextMeshPro healthText;

    // Stores the player's health and synchronizes it across the network.
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        // Only the server should initialize the player's health.
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        // Listen for health changes received through network synchronization.
        currentHealth.OnValueChanged += OnHealthChanged;

        // Update the health display when the object first spawns.
        UpdateHealthText(currentHealth.Value);
    }

    // Called whenever the networked health value changes.
    private void OnHealthChanged(int oldValue, int newValue)
    {
        UpdateHealthText(newValue);
    }

    // Updates the text shown above the player.
    private void UpdateHealthText(int health)
    {
        healthText.text = $"Health: {health}";
    }

    public override void OnNetworkDespawn()
    {
        // Stop listening for health changes when the object is removed.
        currentHealth.OnValueChanged -= OnHealthChanged;
    }

    // Applies damage to the player.
    public void ApplyDamage(int damage)
    {
        // Health can only be changed by the server.
        if (!IsServer) return;

        // Prevent health from dropping below zero.
        currentHealth.Value = Mathf.Max(currentHealth.Value - damage, 0);
    }

    // Requests that the server apply damage to this player.
    [ServerRpc]
    private void TakeDamageServerRpc(int damage)
    {
        ApplyDamage(damage);
    }

    private void Update()
    {
        // Only process input for the local player's object.
        if (!IsOwner) return;

        // Press K to request ten damage from the server.
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamageServerRpc(10);
        }
    }
}
