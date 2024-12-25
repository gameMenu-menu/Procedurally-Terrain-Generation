using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int size;
    public float distance;

    public MeshFilter filter;

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    Vector2[] uvS;

    public List<Shape> shapes = new List<Shape>();

    public TerrainShapeData[] shapePrefabs;

    public PlantGenerator planter;

    public int spawnCount;

    
    void Start()
    {
        PreparePrefabs();
        GenerateTerrain();

        planter.GeneratePlants();
    }


    void ArrangeTriangles()
    {
        triangles = new int[size * size * 6];

        int tris = 0;
        int vert = 0;

        for (int i = 0; i < size; i++)
        {
            for (int k = 0; k < size; k++)
            {
                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + size + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + size + 1;
                triangles[tris + 5] = vert + size + 2;

                vert++;
                tris += 6;

            }
            vert++;
        }

    }

    void PreparePrefabs()
    {
        for(int i=0; i<shapePrefabs.Length; i++)
        {
            shapePrefabs[i].PrepareMap();
        }
    }

    void GenerateTerrain()
    {
        ArrangeVertices(transform.position);

        ArrangeTriangles();

        ArrangeAreas();

        ArrangeUVs();

        RefreshMesh();

        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SaveMesh();
        }
    }

    void ArrangeAreas()
    {
        int count = 0;

        while(true)
        {
            var prefab = Data();

            count++;

            int i = Random.Range(0, size);
            int k = Random.Range(0, size);


            for (int j = 0; j < prefab._heightMap.GetLength(0); j++)
            {
                for (int l = 0; l < prefab._heightMap.GetLength(0); l++)
                {
                    int index = (i + j) * (size + 1) + k + l;

                    if (index >= vertices.Length)
                    {
                        break;
                    }

                    Vector3 vec = vertices[index];
                    vec.y += prefab._heightMap[j, l];

                    vertices[index] = vec;
                }
            }

            if (count == spawnCount) return;
        }

        /*for(int i=0; i<size;)
        {
            for(int k=0; k<size;)
            {
                var prefab = Data();

                for(int j=0; j<prefab._heightMap.GetLength(0); j++)
                {
                    for(int l=0; l<prefab._heightMap.GetLength(0); l++)
                    {
                        int index = ( i + j ) * (size + 1) + k + l;

                        if(index >= vertices.Length)
                        {
                            break;
                        }

                        Vector3 vec = vertices[index];
                        vec.y += prefab._heightMap[j, l];

                        vertices[index] = vec;
                    }
                }

                k += prefab._heightMap.GetLength(0);

            }
            i += 90;

        }*/
    }

    TerrainShapeData Data()
    {
        int[] percents = new int[shapePrefabs.Length];

        int last = 0;

        for (int i = 0; i < shapePrefabs.Length; i++)
        {
            int percent = shapePrefabs[i].percent;

            last += percent;

            percents[i] = last;
        }

        int random = Random.Range(0, last);

        for (int i = 0; i < percents.Length; i++)
        {
            if (random < percents[i])
            {
                return shapePrefabs[i];
            }
        }

        // Fallback (shouldn't reach here)
        return shapePrefabs[shapePrefabs.Length - 1];
    }

    void ArrangeVertices(Vector3 center)
    {
        vertices = new Vector3[(size + 1) * (size + 1)];
        int index = 0;

        Vector3 half = new Vector3(size * distance, 0, size * distance) / 2f;

        for (int i = 0; i < size + 1; i++)
        {
            for (int k = 0; k < size + 1; k++)
            {
                Vector3 pos = new Vector3(k * distance, 0, i * distance) + center;
                vertices[index] = pos - half;
                index++;
            }
        }
    }

    void RefreshMesh()
    {
        mesh = new Mesh();

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = vertices;

        mesh.triangles = triangles;

        mesh.uv = uvS;

        mesh.RecalculateNormals();

        filter.mesh = mesh;

    }

    void ArrangeUVs()
    {
        uvS = new Vector2[vertices.Length];

        float minZ = 0, maxZ = 0, minX = 0, maxX = 0;



        for (int i = 0; i < vertices.Length; i++)
        {

            if (vertices[i].x < minX) minX = vertices[i].x;
            if (vertices[i].x > maxX) maxX = vertices[i].x;

            if (vertices[i].z < minZ) minZ = vertices[i].z;
            if (vertices[i].z > maxZ) maxZ = vertices[i].z;
        }

        float xDiff = maxX - minX;
        xDiff = Mathf.Abs(xDiff);
        float zDiff = maxZ - minZ;
        zDiff = Mathf.Abs(zDiff);

        for (int i = 0; i < uvS.Length; i++)
        {
            uvS[i] = new Vector2(Mathf.Abs(vertices[i].x / xDiff), Mathf.Abs(vertices[i].z / zDiff));
        }

    }

    public void SaveMesh()
    {
        if (mesh == null)
        {
            Debug.LogError("No mesh to save.");
            return;
        }

        string path = EditorUtility.SaveFilePanelInProject(
            "Save Mesh",
            mesh.name + ".asset",
            "asset",
            "Please enter a file name to save the mesh."
        );

        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
            Debug.Log("Mesh saved to: " + path);
        }
    }
}
