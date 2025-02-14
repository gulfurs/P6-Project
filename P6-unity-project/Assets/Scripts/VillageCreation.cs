using UnityEngine;
using System.Collections.Generic;

public class VillageCreation : MonoBehaviour
{
    public List<BuildingType> buildingTypes;
    public int villageSize = 10;
    public int roadWidth = 2;
    public Transform villageParent;

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

    void GenerateVillage()
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
        for (int x = 0; x < villageSize; x++)
        {
            for (int z = 0; z < villageSize; z++)
            {
                if (!grid[x, z])
                {
                    return new Vector2Int(x, z);
                }
            }
        }
        return new Vector2Int(-1, -1); // No empty position found
    }

    void PlaceBuilding(GameObject buildingPrefab, Vector2Int position)
    {
        GameObject building = Instantiate(buildingPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity, villageParent);
        grid[position.x, position.y] = true;

        // Mark surrounding spaces as occupied for roads
        for (int x = -roadWidth; x <= roadWidth; x++)
        {
            for (int z = -roadWidth; z <= roadWidth; z++)
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
}

