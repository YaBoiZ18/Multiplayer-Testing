using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private TextMeshPro healthText;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
        currentHealth.OnValueChanged += OnHealthChanged;
        UpdateHealthText(currentHealth.Value);
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        UpdateHealthText(newValue);
    }

    private void UpdateHealthText(int health)
    {
        healthText.text = $"Health: {health}";
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
    }

    [ServerRpc]
    private void TakeDamageServerRpc(int damage)
    {
        currentHealth.Value = Mathf.Max(currentHealth.Value - damage, 0);
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamageServerRpc(10);
        }
    }
}
