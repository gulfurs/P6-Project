using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Diagnostics;
using UnityEngine.TerrainUtils;

public class TerrainController : MonoBehaviour
{
    public int seed = 0;

    public float scale = 40f;

    public float offsetX = 0;
    public float offsetY = 0;

    public float offset = 0;
    public void OnValidate()
    {
        foreach (Terrain terrain in Terrain.activeTerrains)
        {
            TerrainData terrainData = terrain.terrainData;
            int HMR = terrain.terrainData.heightmapResolution;
            float[,] heights = new float[HMR,HMR];

            Random.InitState(seed); // Ensures different results with different seeds

            offsetX = terrain.transform.position.x + offset;

            for (int x = 0; x < HMR; x++)
            {
                for (int z = 0; z < HMR; z++)
                {

                    float worldPositionX = ((float)x / (float)HMR) * scale;
                    float worldPositionz = ((float)z / (float)HMR) * scale;

                    heights[z, x] += Mathf.PerlinNoise(worldPositionX + seed + offsetX, worldPositionz + seed + offsetY); // Adjust intensity
                    
                }
            }
            terrainData.SetHeights(0, 0, heights);
            
        }
    }
}


