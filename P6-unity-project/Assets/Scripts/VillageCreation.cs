using UnityEngine;
using System.Collections.Generic;

public class VillageCreation : MonoBehaviour
{
    public List<BuildingType> buildingTypes;
    public int villageSize = 10;
    public int cellSize = 2;
    public Transform villageParent;
    public Terrain terrain; // Reference to the terrain

    public float placementRandomness = 0.5f;

    private bool[,] grid;

    [System.Serializable]
    public class BuildingType
    {
        public GameObject prefab;
        public int minCount;
        public int maxCount;
        public bool isUnique; 
    }

    void Start()
    {
        GenerateVillage();
    }

    public void GenerateVillage()
    {
        grid = new bool[villageSize, villageSize];

        foreach (var buildingType in buildingTypes)
        {
            int buildingCount = buildingType.isUnique ? 1 : Random.Range(buildingType.minCount, buildingType.maxCount + 1);

            for (int i = 0; i < buildingCount; i++)
            {
                if (buildingType.isUnique && i > 0) continue; // Ensure only one unique building is placed

                Vector2Int position = FindEmptyPosition();
                if (position == new Vector2Int(-1, -1)) break; // No more space

                PlaceBuilding(buildingType.prefab, position);
            }
        }
    }

    Vector2Int FindEmptyPosition()
    {
        int attempts = 0;
        while (attempts < 100) // Try 100 times to find a valid position
        {
            int x = Random.Range(0, villageSize);
            int z = Random.Range(0, villageSize);

            if (!grid[x, z])
            {
                return new Vector2Int(x, z);
            }
            attempts++;
        }
        return new Vector2Int(-1, -1); // No empty position found
    }

    void PlaceBuilding(GameObject buildingPrefab, Vector2Int position)
    {
        float randomOffsetX = (Random.value - 0.5f) * placementRandomness * cellSize;
        float randomOffsetZ = (Random.value - 0.5f) * placementRandomness * cellSize;

        Vector3 basePosition = new Vector3(
            position.x * cellSize + randomOffsetX,
            0, // We'll set the height dynamically
            position.y * cellSize + randomOffsetZ
        );

        // Get terrain height at this position
        float terrainHeight = GetTerrainHeight(basePosition);

        // Adjust the position based on terrain height
        Vector3 buildingPosition = new Vector3(basePosition.x, terrainHeight + 10, basePosition.z);

        GameObject building = Instantiate(buildingPrefab, buildingPosition, Quaternion.identity, villageParent);
        grid[position.x, position.y] = true;

        // Mark surrounding spaces as occupied to prevent overlapping
        int buildingSize = Mathf.CeilToInt(cellSize);
        for (int x = -buildingSize; x <= buildingSize; x++)
        {
            for (int z = -buildingSize; z <= buildingSize; z++)
            {
                int newX = position.x + x;
                int newZ = position.y + z;

                if (newX >= 0 && newX < villageSize && newZ >= 0 && newZ < villageSize)
                {
                    grid[newX, newZ] = true;
                }
            }
        }
    }

    float GetTerrainHeight(Vector3 position)
    {
        if (terrain != null)
        {
            return terrain.SampleHeight(position);
        }
        else
        {
            // Fallback using Raycast if no terrain reference is set
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(position.x, 1000f, position.z), Vector3.down, out hit, Mathf.Infinity))
            {
                return hit.point.y;
            }
        }
        return 0f; // Default to 0 if terrain height cannot be found
    }
}
