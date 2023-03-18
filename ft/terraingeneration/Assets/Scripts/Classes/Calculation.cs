using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculation
{
    public static List<Shape> ArrangeAreas(ShapePrefab[] prefabs, int size, float distanceBetweenVertices)
    {
        List<Shape> shapes = new List<Shape>();

        int totalPercent = 100;

        int lastRadius = 0;

        for(int i=10; i<size/2; i++)
        {
            for(int k=10; k<size/2; k++)
            {

                for(int p=0; p<prefabs.Length; p++)
                {
                    ShapePrefab prefab = prefabs[p];
                    int percent = prefab.percent;

                    int checkRadius = (int) (prefab.radius * 2);

                    if(Random.Range(0, totalPercent) > percent)
                    {
                        continue;
                    }

                    Vector3 center = new Vector3(i * distanceBetweenVertices * 2f, 0, k * distanceBetweenVertices * 2f);

                    shapes.Add(prefab.GetShape(center));

                    k+=checkRadius / 2;

                    lastRadius = checkRadius;
                    
                }

                

                
            }

            i += lastRadius / 2;
        }

        return shapes;
    }
    public static Vector3[] ArrangeVertexHeights(Vector3[] vertices, List<Shape> shapes, float distanceBetweenVertices)
    {
        Vector3[] result = vertices;

        for(int i=0; i<shapes.Count; i++)
            {
                int range = shapes[i].radius;
                Vector2 center = GetXZ(shapes[i].center);

                float height = shapes[i].height;

                bool hasHeightMap = false;
                if(shapes[i].heightMap != null) hasHeightMap = true;
                for(int k=0; k<vertices.Length; k++)
                {
                    Vector2 vec = GetXZ(vertices[k]);
                    float distance = ( center - vec).magnitude;

                    if(distance < range * distanceBetweenVertices)
                    {
                        if(!hasHeightMap)
                        {
                            result[k] = new Vector3(vec.x, GetHeight(height * distanceBetweenVertices, distance , range * distanceBetweenVertices, shapes[i].up), vec.y);
                            //new GameObject(result[k]+" "+distance);
                        }
                        else
                        {
                            result[k] = new Vector3(vec.x, GetHeight(center, vec, shapes[i].heightMap, range, shapes[i].up, shapes[i].height, distanceBetweenVertices), vec.y);
                            //new GameObject(result[k]+" "+distance);
                        }

                    }
                }
            }

        

        return result;
    }

    public static Vector2 GetXZ(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    public static float GetHeight(float maxHeight, float difference, float maxDifference, bool up)
    {
        float h = (maxDifference - difference) * maxHeight;
        int a = 1;
        if(h < 0)
        {
            a = -1;
        }
        h = Mathf.Sqrt(Mathf.Abs(h));
        h *= a;
        return h;
    }

    public static float GetHeight(Vector2 center, Vector2 vertex, float[ , ] heightMap, int radius, bool up, float height, float vertexDistance)
    {

        Vector2 hPos = vertex - center;
        int pixelCount = heightMap.GetLength(0);
        hPos = hPos / (vertexDistance * 2);
        hPos *= (pixelCount) / radius;

        hPos = new Vector2(pixelCount / 2 + hPos.x, pixelCount / 2 + hPos.y);

        float result =  heightMap[(int) hPos.x, (int) hPos.y];

        if(!up) result -=1;

        result *= radius * height * vertexDistance;

        //result *= 5f;

        return result;

    }
}
