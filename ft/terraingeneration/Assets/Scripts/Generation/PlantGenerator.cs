using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGenerator : MonoBehaviour
{
    public GameObject[] plantPrefabs;
    public Plant[] plants;
    public MeshFilter filter;

    public int interval;
    public float scale;

    public bool generatePlants;

    public float positionRandomizeValue;

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
            for(int k=0; k<plants.Length; k++)
            {
                if(Random.Range(0f, 1f) > plants[k].percent) continue;

                Vector3 slope = (normals[triangles[i]] + normals[triangles[i+1]]) * 90f / 2f;

                float s = Mathf.Abs(slope.x) + Mathf.Abs(slope.z);

                if(s < plants[k].startSlope || s > plants[k].endSlope) continue;

                Vector3 vec1 = vertices[triangles[i]];
                Vector3 vec2 = vertices[triangles[i+1]];

                Vector3 pos = (vec1 + vec2) / 2 + new Vector3(Random.Range(-positionRandomizeValue, positionRandomizeValue), Random.Range(-positionRandomizeValue, positionRandomizeValue), Random.Range(-positionRandomizeValue, positionRandomizeValue));

                GameObject plant = Instantiate(plants[k].prefab, pos, plants[k].prefab.transform.rotation);

                plant.transform.eulerAngles = slope;

                plant.isStatic = true;
            }
            
        }
    }
}
