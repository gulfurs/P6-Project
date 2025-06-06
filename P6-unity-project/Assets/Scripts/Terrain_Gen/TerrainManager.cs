using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    public GameObject terrainPrefab; // Terrain prefab
    public Transform player; // The player
    public int terrainSize = 1000; // Size of terrain
    public float triggerDistance = 50f; // Distance from the edge before spawning

    private Dictionary<Vector2Int, GameObject> activeTerrains = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int initialTile; // The starting terrain tile

    void Start()
    {
        // Identify the initial terrain position based on where the player starts
        initialTile = new Vector2Int(
            Mathf.FloorToInt(player.position.x / terrainSize),
            Mathf.FloorToInt(player.position.z / terrainSize)
        );

        // Register the initial terrain as "already existing"
        activeTerrains[initialTile] = terrainPrefab;
    }

    void Update()
    {
        CheckPlayerPosition();
    }

    void CheckPlayerPosition()
    {
        Vector3 playerPos = player.position;
        Vector2Int currentTile = new Vector2Int(
            Mathf.FloorToInt(playerPos.x / terrainSize),
            Mathf.FloorToInt(playerPos.z / terrainSize)
        );

        List<Vector2Int> newTiles = new List<Vector2Int>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector2Int newTile = currentTile + new Vector2Int(x, z);
                if (!activeTerrains.ContainsKey(newTile))
                {
                    newTiles.Add(newTile);
                }
            }
        }

        foreach (var tile in newTiles)
        {
            SpawnTerrain(tile);
        }
    }

    void SpawnTerrain(Vector2Int tileCoord)
    {
        // Don't spawn a terrain on the initial starting position
        if (tileCoord == initialTile) return;

        Vector3 position = new Vector3(tileCoord.x * terrainSize, 0, tileCoord.y * terrainSize);
        GameObject newTerrain = Instantiate(terrainPrefab, position, Quaternion.identity);

        // Apply mirroring if necessary
        if (tileCoord.x % 2 != 0) // Mirror along X
        {
            newTerrain.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (tileCoord.y % 2 != 0) // Mirror along Z
        {
            newTerrain.transform.localScale = new Vector3(newTerrain.transform.localScale.x, 1, -1);
        }

        activeTerrains[tileCoord] = newTerrain;
    }
}