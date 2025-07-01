csharp
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum ItemType
    {
        Health,
        Weapon,
        Score
        // Add other item types here
    }

    [SerializeField]
    private ItemType itemType; // Choose the type of this item in the Inspector
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
                // You will need to add an ApplyPickupEffect method to your PlayerController script
                player.ApplyPickupEffect(itemType, value);

                // Destroy the item GameObject after being picked up
                Destroy(gameObject);
            }
        }
    }
}