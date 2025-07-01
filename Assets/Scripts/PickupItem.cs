csharp
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [System.Serializable] // Enums can be serialized in the Inspector
    public enum PickupType
    {
        Health,
        Weapon,
        Score
        // Add other item types here
    }
    [SerializeField]
    private PickupType itemType; // Choose the type of this item in the Inspector
    [SerializeField]
    private GameObject weaponPrefab; // Assign the weapon prefab here if itemType is Weapon
    [SerializeField]
    private int value; // Value associated with the item (e.g., healing amount, score amount, weapon ID)


 void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the entering collider is the player
        if (other.CompareTag("Player"))
        {
            // Get the player's controller script
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // Apply the item's effect on the player
                ApplyEffectToPlayer(player);

                // Destroy the item GameObject after being picked up
                Destroy(gameObject);
            }
        }
    }
}