using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    public int terrainSize = 512; // Size of each terrain tile
    public GameObject terrainPrefab; // Terrain prefab to drag in the inspector
    public float terrainHeight = 50f; // Max terrain height
    public Transform player; // Reference to player
    public int terrainLoadDistance = 200; // Distance from the edge to load new terrains
    public GameObject villagePrefab; // Village spawner prefab
    public float villageSpawnChance = 0.5f; // 50% chance to spawn a village

    private Dictionary<Vector2Int, Terrain> activeTerrains = new Dictionary<Vector2Int, Terrain>();
    private HashSet<Vector2Int> checkedTiles = new HashSet<Vector2Int>();

    void Start()
    {
        GenerateTerrain(Vector2Int.zero); // Start with a central terrain
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
                if (!activeTerrains.ContainsKey(newTile) && !checkedTiles.Contains(newTile))
                {
                    newTiles.Add(newTile);
                    checkedTiles.Add(newTile);
                }
            }
        }

        foreach (var tile in newTiles)
        {
            GenerateTerrain(tile);
        }
    }

    void GenerateTerrain(Vector2Int tileCoord)
    {
        if (activeTerrains.ContainsKey(tileCoord))
        {
            return; // Terrain already exists, no need to generate again
        }

        GameObject terrainObj = Instantiate(terrainPrefab, new Vector3(tileCoord.x * terrainSize, 0, tileCoord.y * terrainSize), Quaternion.identity);
        Terrain terrain = terrainObj.GetComponent<Terrain>();
        TerrainCollider terrainCollider = terrainObj.GetComponent<TerrainCollider>();

        terrain.terrainData = GenerateTerrainData(tileCoord);
        terrainCollider.terrainData = terrain.terrainData;

        activeTerrains[tileCoord] = terrain;

        // 50% chance to spawn a village
        if (Random.value < villageSpawnChance)
        {
            SpawnVillage(terrainObj.transform.position);
        }
    }

    TerrainData GenerateTerrainData(Vector2Int tileCoord)
    {
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = terrainPrefab.GetComponent<Terrain>().terrainData.heightmapResolution;
        terrainData.size = new Vector3(terrainSize, terrainHeight, terrainSize);

        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int z = 0; z < terrainData.heightmapResolution; z++)
            {
                float worldX = (tileCoord.x * terrainSize) + (x / (float)(terrainData.heightmapResolution - 1) * terrainSize);
                float worldZ = (tileCoord.y * terrainSize) + (z / (float)(terrainData.heightmapResolution - 1) * terrainSize);

                heights[x, z] = Mathf.PerlinNoise(worldX * 0.001f, worldZ * 0.001f); // Smooth Perlin noise terrain
            }
        }

        // Ensure the edges connect smoothly with neighboring tiles
        if (activeTerrains.TryGetValue(tileCoord + Vector2Int.left, out Terrain leftTerrain))
        {
            float[,] leftHeights = leftTerrain.terrainData.GetHeights(terrainData.heightmapResolution - 1, 0, 1, terrainData.heightmapResolution);
            for (int z = 0; z < terrainData.heightmapResolution; z++)
            {
                heights[0, z] = leftHeights[0, z];
            }
        }

        if (activeTerrains.TryGetValue(tileCoord + Vector2Int.down, out Terrain bottomTerrain))
        {
            float[,] bottomHeights = bottomTerrain.terrainData.GetHeights(0, terrainData.heightmapResolution - 1, terrainData.heightmapResolution, 1);
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                heights[x, 0] = bottomHeights[x, 0];
            }
        }

        terrainData.SetHeights(0, 0, heights);
        return terrainData;
    }

    void SpawnVillage(Vector3 terrainPosition)
    {
        Vector3 villagePosition = terrainPosition + new Vector3(terrainSize / 2, 0, terrainSize / 2); // Center of terrain
        GameObject newVillage = Instantiate(villagePrefab, villagePosition, Quaternion.identity);
        newVillage.GetComponent<VillageCreation>().GenerateVillage();
    }
}