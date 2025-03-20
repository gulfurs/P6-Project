using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Terrain Generation Parameters
    [Header("Terrain Parameters")]
    public int heightMapResolution = 513;  // Must be 2^n + 1
    public float terrainSize = 500f;
    public float heightScale = 100f;

    // Noise Parameters
    [Header("Noise Parameters")]
    public float noiseScale = 50f;
    public int octaves = 6;
    public float persistence = 0.5f;
    public float lacunarity = 2.0f;
    public float offset = 200.4f;
    public int seed = 42;

    // Player and positioning
    [Header("Player Settings")]
    public GameObject player;
    public float updateDistance = 200f;
    private Vector3 lastPlayerPosition;
    private Terrain terrain;
    private TerrainData terrainData;

    private void Start()
    {
        // Initialize terrain
        terrain = GetComponent<Terrain>();

        // Create terrain data if it doesn't exist
        if (terrain.terrainData == null)
        {
            terrainData = new TerrainData();
            terrainData.heightmapResolution = heightMapResolution;
            terrainData.size = new Vector3(terrainSize, heightScale, terrainSize);
            terrain.terrainData = terrainData;
        }
        else
        {
            terrainData = terrain.terrainData;
            terrainData.heightmapResolution = heightMapResolution;
            terrainData.size = new Vector3(terrainSize, heightScale, terrainSize);
        }

        // Set initial player position
        if (player != null)
        {
            lastPlayerPosition = player.transform.position;
            PositionTerrainAroundPlayer();
        }

        // Generate initial terrain
        GenerateTerrain();
    }

    private void Update()
    {
        if (player != null && Vector3.Distance(player.transform.position, lastPlayerPosition) > updateDistance)
        {
            lastPlayerPosition = player.transform.position;
            PositionTerrainAroundPlayer();
            GenerateTerrain();
        }
    }

    private void PositionTerrainAroundPlayer()
    {
        // Position terrain with player in the center
        transform.position = new Vector3(
            player.transform.position.x - terrainSize / 2,
            0, // Keep terrain at y=0
            player.transform.position.z - terrainSize / 2
        );
    }

    private void GenerateTerrain()
    {
        // Get world position offsets
        float offsetX = transform.position.x / noiseScale;
        float offsetZ = transform.position.z / noiseScale;

        // Create seed-based random number generator
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float xOffset = (seed + i * 1000) + offsetX;
            float zOffset = (seed + i * 1000) + offsetZ;
            octaveOffsets[i] = new Vector2(xOffset, zOffset);
        }


        // Generate heightmap
        float[,] heightMap = new float[heightMapResolution, heightMapResolution];
        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;

        for (int y = 0; y < heightMapResolution; y++)
        {
            for (int x = 0; x < heightMapResolution; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                // Generate multiple octaves of noise
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / (float)heightMapResolution * noiseScale * frequency + octaveOffsets[i].x;
                    float sampleY = y / (float)heightMapResolution * noiseScale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                // Track min/max heights for normalization
                if (noiseHeight > maxHeight)
                    maxHeight = noiseHeight;
                if (noiseHeight < minHeight)
                    minHeight = noiseHeight;

                heightMap[y, x] = noiseHeight;
            }
        }

        // Normalize heightmap
        for (int y = 0; y < heightMapResolution; y++)
        {
            for (int x = 0; x < heightMapResolution; x++)
            {
                heightMap[y, x] = Mathf.InverseLerp(minHeight, maxHeight, heightMap[y, x]);

                // Apply additional height adjustments here if needed
                // Example: heightMap[y, x] = Mathf.Pow(heightMap[y, x], 1.5f); // Makes mountains more dramatic
            }
        }

        // Apply the heightmap to terrain
        terrainData.SetHeights(0, 0, heightMap);

        // Optional: Update terrain collider
        terrain.Flush();
    }
}