using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class Terrain_perlin_test : MonoBehaviour
{
[Header("Terrain Settings")]
    public int width = 50;
    public int depth = 50;
    public float scale = 10f;
    public float heightMultiplier = 5f;
    
    private MeshFilter meshFilter;
    private Mesh mesh;
    
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        GenerateTerrain();
    }
    
    void GenerateTerrain()
    {
        mesh = new Mesh();
        Vector3[] vertices = new Vector3[(width + 1) * (depth + 1)];
        int[] triangles = new int[width * depth * 6];
        
        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = Mathf.PerlinNoise(x * scale * 0.01f, z * scale * 0.01f) * heightMultiplier;
                vertices[z * (width + 1) + x] = new Vector3(x, y, z);
            }
        }
        
        int tris = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int vert = z * (width + 1) + x;
                
                triangles[tris] = vert;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;
                
                tris += 6;
            }
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        meshFilter.mesh = mesh;
    }
}
