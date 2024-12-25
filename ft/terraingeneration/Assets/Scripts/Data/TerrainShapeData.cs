using UnityEngine;

public abstract class TerrainShapeData : ScriptableObject
{
    public int radius;
    public int percent;
    public float height;

    public float[,] _heightMap;

    protected virtual void SetMap()
    {
        _heightMap = new float[radius, radius];
    }

    public void PrepareMap()
    {
        
        SetMap();
    }

    protected virtual void FixToGround()
    {
        float minimum = 99999f;

        for (int i = 0; i < radius; i++)
        {
            float check = _heightMap[i, 0];

            if (check < minimum) minimum = check;

            check = _heightMap[i, radius - 1];

            if (check < minimum) minimum = check;
        }

        for (int i = 0; i < radius; i++)
        {
            float check = _heightMap[0, i];

            if (check < minimum) minimum = check;

            check = _heightMap[radius - 1, i];

            if (check < minimum) minimum = check;
        }

        for (int i = 0; i < radius; i++)
        {
            for (int k = 0; k < radius; k++)
            {

                _heightMap[i, k] -= minimum;
            }
        }
    }

}
