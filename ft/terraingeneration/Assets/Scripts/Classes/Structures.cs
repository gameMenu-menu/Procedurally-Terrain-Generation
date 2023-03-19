using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ShapeType { Mountain, Dent }

[System.Serializable]
public class ShapePrefab
{

    public ShapeType type;
    public int radius;
    public int percent;
    public float height;

    public Texture2D heightMap;

    public float[ , ] _heightMap;
    public bool up;


    
    public Shape GetShape(Vector3 center)
    {
        return new Shape(type, center, radius, height, _heightMap, up);
    }

    float[ , ] GetMap()
    {
        if(heightMap == null)
        {
            return null;
        }

        float[,] map = new float[ (int) Mathf.Sqrt(heightMap.GetPixels().Length),  (int) Mathf.Sqrt(heightMap.GetPixels().Length)];

        for(int i=0; i<map.GetLength(0); i++)
        {
            for(int k=0; k<map.GetLength(1); k++)
            {
                map[i, k] = heightMap.GetPixel(i, k).grayscale;
            }
        }

        return map;
    }

    public void PrepareMap()
    {
        _heightMap = GetMap();
    }
}

public struct Shape
{
    public Shape(ShapeType _type, Vector3 _center, int _radius, float _height, float[ , ] _heightMap, bool _up)
    {
        type = _type;
        center = _center;
        radius = _radius;
        height = _height;
        heightMap = _heightMap;
        up = _up;
    }

    public ShapeType type;
    public Vector3 center;
    public int radius;
    public float height;
    public float[ , ] heightMap;
    public bool up;
}

public struct HeightMap
{
    float[ , ] map;

}
