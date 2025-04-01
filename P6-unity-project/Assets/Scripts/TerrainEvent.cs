using UnityEngine;

public class TerrainEvent : MonoBehaviour
{
    private Transform player;
    private bool hasEnteredTerrain = false; // Track if the player has stepped on this terrain
    private VillageCreation villageCreation; // Reference to village generator
    public bool createVillage = true;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        villageCreation = FindObjectOfType<VillageCreation>();
    }

    void Update()
    {
        if (player == null) return; // Prevent null reference

        bool isOnThisTerrain = IsPlayerOnThisTerrain();

        if (isOnThisTerrain && !hasEnteredTerrain)
        {
            hasEnteredTerrain = true; // Prevent multiple triggers
            Debug.Log("Player stepped on: " + gameObject.name);

            if (villageCreation != null && createVillage)
            {
                villageCreation.villageParent = transform.GetChild(0);
                //villageCreation.terrain = GetComponent<Terrain>();
                villageCreation.GenerateVillage();
                foreach (var terrainEvent in FindObjectsOfType<TerrainEvent>())
                {
                      terrainEvent.createVillage = false;
                }
                Debug.Log("Village generated on: " + gameObject.name);
            }
        }
        else if (!isOnThisTerrain && hasEnteredTerrain)
        {
            hasEnteredTerrain = false; // Reset when the player leaves (optional, doesn’t affect generation)
        }
    }

    bool IsPlayerOnThisTerrain()
    {
        RaycastHit hit;
        int groundLayerMask = LayerMask.GetMask("Ground");

        Vector3 rayOrigin = player.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, groundLayerMask))
        {
            return hit.collider.gameObject == gameObject; // Only react if player is on *this* terrain
        }
        return false;
    }
}
