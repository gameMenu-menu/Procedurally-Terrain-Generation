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

    void SetMap()
    {
        float ratio = Mathf.Sqrt(heightMap.GetPixels().Length) / (float) radius;

        _heightMap = new float[radius, radius];

        for (int i=0; i<radius; i++)
        {
            for(int k=0; k<radius; k++)
            {
                int index1 = (int) (i * ratio);
                int index2 = (int) (k * ratio);

                Color c = heightMap.GetPixel(index1, index2);

                float h = c.grayscale * height;

                _heightMap[i, k] = h;
            }
        }

        if (up) SmoothHeightUp();
        else SmoothHeightDown();
    }

    void SmoothHeightUp()
    {
        float minimum = 99999f;

        for (int i = 0; i < radius; i++)
        {
            float h = _heightMap[i, 0];

            if (h < minimum)
            {
                minimum = h;
            }
        }

        for (int i = 0; i < radius; i++)
        {
            float h = _heightMap[i, radius - 1];

            if (h < minimum)
            {
                minimum = h;
            }
        }

        for (int i = 0; i < radius; i++)
        {
            float h = _heightMap[0, i];

            if (h < minimum)
            {
                minimum = h;
            }
        }

        for (int i = 0; i < radius; i++)
        {
            float h = _heightMap[radius - 1, i];

            if (h < minimum)
            {
                minimum = h;
            }
        }

        for (int i = 0; i < radius; i++)
        {
            for (int k = 0; k < radius; k++)
            {

                _heightMap[i, k] -= minimum;
                //_heightMap[i, k] *= height;
            }
        }
    }

    void SmoothHeightDown()
    {
        float maksimum = 0f;

        for (int i = 0; i < radius; i++)
        {
            for (int k = 0; k < radius; k++)
            {
                //_heightMap[i, k] *= height;
            }
        }

        for (int i = 0; i < radius; i++)
        {
            float h = _heightMap[i, 0];

            if (h > maksimum)
            {
                maksimum = h;
            }
        }

        for (int i = 0; i < radius; i++)
        {
            float h = _heightMap[i, radius - 1];

            if (h > maksimum)
            {
                maksimum = h;
            }
        }

        for (int i = 0; i < radius; i++)
        {
            float h = _heightMap[0, i];

            if (h > maksimum)
            {
                maksimum = h;
            }
        }

        for (int i = 0; i < radius; i++)
        {
            float h = _heightMap[radius - 1, i];

            if (h > maksimum)
            {
                maksimum = h;
            }
        }

        for (int i = 0; i < radius; i++)
        {
            for (int k = 0; k < radius; k++)
            {

                _heightMap[i, k] -= maksimum + 1;
            }
        }
    }

    public void PrepareMap()
    {
        _heightMap = null;
        SetMap();
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
