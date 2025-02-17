using UnityEngine;

public class RandomTerrainGenerator : MonoBehaviour
{
    public Terrain terrain;
    public int seed = 0; // Change this to randomize

    void Start()
    {
        GenerateTerrain(seed);
    }

    void GenerateTerrain(int seed)
    {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = new float[width, height];

        Random.InitState(seed); // Ensures different results with different seeds

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * 10;
                float yCoord = (float)y / height * 10;
                heights[x, y] = Mathf.PerlinNoise(xCoord + seed, yCoord + seed) * 0.2f; // Adjust intensity
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
}
