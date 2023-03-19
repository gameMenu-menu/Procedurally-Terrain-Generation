using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGenerator : MonoBehaviour
{
    public GameObject[] plantPrefabs;
    public MeshFilter filter;

    public int interval;
    public float scale;

    public bool generatePlants;

    public void GeneratePlants()
    {
        if(!generatePlants) return;
        Mesh mesh = filter.mesh;

        Vector3[] vertices = mesh.vertices;
        var normals = mesh.normals;
        int[] triangles = mesh.triangles;

        float distance = Vector3.Distance(vertices[0], vertices[1]);
        int size = (int) Mathf.Sqrt(vertices.Length);

        for(int i=0; i<triangles.Length-1; i++)
        {
            Vector3 vec1 = vertices[triangles[i]];
            Vector3 vec2 = vertices[triangles[i+1]];

            Vector3 pos = (vec1 + vec2) / 2;

            GameObject plant = Instantiate(plantPrefabs[0], pos, plantPrefabs[0].transform.rotation);

            plant.transform.eulerAngles = (normals[triangles[i]] + normals[triangles[i+1]]) * 90f / 2f;

            plant.isStatic = true;
        }
    }
}
