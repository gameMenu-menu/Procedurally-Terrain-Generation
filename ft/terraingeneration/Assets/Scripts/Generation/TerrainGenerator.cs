using System.Collections;
using System.Collections.Generic;
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

    public ShapePrefab[] shapePrefabs;

    public PlantGenerator planter;


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

    void ArrangeAreas()
    {

        shapes = Calculation.ArrangeAreas(shapePrefabs, size, distance);

        vertices = Calculation.ArrangeVertexHeights(vertices, shapes, distance);
    }

    void RefreshMesh()
    {
        mesh = new Mesh();


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

    void GenerateTerrain()
    {
        ArrangeVertices(transform.position);

        ArrangeTriangles();

        ArrangeAreas();

        ArrangeUVs();

        RefreshMesh();
    }

    void ArrangeVertices(Vector3 center)
    {
        vertices = new Vector3[(size + 1) * (size + 1)];
        int index = 0;
        for (int i = 0; i < size + 1; i++)
        {
            for (int k = 0; k < size + 1; k++)
            {
                Vector3 pos = new Vector3(k * distance, 0, i * distance) + center;
                vertices[index] = pos;
                index++;
            }
        }
        
    }


    /*void OnDrawGizmos()
    {
        if(triangles != null)
        for(int i=0; i<triangles.Length; i++)
        {
            if( i < triangles.Length-2 ) Gizmos.DrawLine(vertices[triangles[i]], vertices[triangles[i+1]]);
        }
    }*/
}
