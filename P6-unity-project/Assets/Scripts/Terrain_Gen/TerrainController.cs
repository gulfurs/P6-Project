using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Diagnostics;
using UnityEngine.TerrainUtils;

public class TerrainController : MonoBehaviour
{
    public int seed = 0;

    public float scale = 5f;

    public float offsetX = 0;
    public float offsetZ = 0;

    public float offset = 200.4f;
    private void Update()
    {   
        foreach (Terrain terrain in Terrain.activeTerrains)
        {
            
            TerrainData terrainData = terrain.terrainData;
            int HMR = terrain.terrainData.heightmapResolution;
            float[,] heights = new float[HMR, HMR];


            if (terrain.transform.position.x != 0)
            {
                offsetX = terrain.transform.position.x / offset;
            }
            if (terrain.transform.position.z != 0)
            {
                offsetZ = terrain.transform.position.z / offset;
            }

            for (int x = 0; x < HMR; x++)
            {
                for (int z = 0; z < HMR; z++)
                {

                    float worldPositionX = ((float)x / (float)HMR) * scale;
                    float worldPositionz = ((float)z / (float)HMR) * scale;

                    heights[z, x] += Mathf.PerlinNoise(worldPositionX + offsetX, worldPositionz + offsetZ); // Adjust intensity
                    heights[20, 20] = 1;
                }
            }
            terrainData.SetHeights(0, 0, heights);
        }
        
    }
}


