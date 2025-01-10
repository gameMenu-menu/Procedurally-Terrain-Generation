using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine;

public class TerrainBake : MonoBehaviour
{
    public int size;

    float distance;

    public MeshFilter filter;

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    Vector2[] uvS;

    [SerializeField]
    MeshFilter f;


    void Start()
    {
        GenerateTerrain();
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

    void GenerateTerrain()
    {
        ArrangeVertices(transform.position);

        ArrangeTriangles();

        ArrangeUVs();

        StickToTerrain();

        RefreshMesh();

        SaveMesh();
    }

    void StickToTerrain()
    {
        Vector3 addVector = new Vector3(size * distance / 2f, 0, size * distance / 2f);
        for(int i=0; i<vertices.Length; i++)
        {
            Vector3 pos = vertices[i];

            RaycastHit hit;

            if (Physics.Raycast(pos + Vector3.up * 2000f + addVector, Vector3.down, out hit, 40000f))
            {
                pos.y = hit.point.y;

                vertices[i] = pos;
            }
        }
    }

    void ArrangeVertices(Vector3 center)
    {
        distance = f.mesh.bounds.size.x / (float) size;

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

        string path = "Assets/Export/Mesh.asset";

        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();

        transform.position = new Vector3(size * distance / 2f, 0, size * distance / 2f);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/Export/Terrain.prefab", out bool success);

        DestroyImmediate(prefab.GetComponent<TerrainBake>(), true);
    }
}
