csharp
using UnityEngine;

    [SerializeField]
    private int areaIndex; // The index of the area this trigger corresponds to

    private LevelManager levelManager;

    void Start()
    {
        // Find the LevelManager in the scene
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene! AreaTrigger requires a LevelManager.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the entering collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Notify the LevelManager that the player has entered this area trigger
            if (levelManager != null)
            {
                levelManager.PlayerEnteredArea(areaIndex); // This method needs to be added in LevelManager
            }
        }
    }
}