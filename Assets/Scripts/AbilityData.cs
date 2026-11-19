using UnityEngine;

// Stores the configurable data for an ability.
[CreateAssetMenu(fileName = "AbilityData", menuName = "Abilities/AbilityData")]
public class AbilityData : ScriptableObject
{
    // Display name of the ability.
    public string abilityName;

    // Time in seconds before the ability can be used again.
    public float cooldown;

    // Damage dealt by the ability.
    public int damage;

    // Movement speed of the ability projectile or effect.
    public float speed;

    // Time in seconds before the ability expires.
    public float lifetime;

    // Prefab spawned when the ability is used.
    public GameObject prefab;
}
    