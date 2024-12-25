using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Curval Data", menuName = "Data/Curval Map")]
public class CurveData : TerrainShapeData
{
    public AnimationCurve rowCurve, columnCurve;

    public bool up;

    protected override void SetMap()
    {
        base.SetMap();

        float step = 1f / (float)radius;

        for (int i = 0; i < radius; i++)
        {
            for (int k = 0; k < radius; k++)
            {
                float r = rowCurve.Evaluate(step * (float) i);
                float c = columnCurve.Evaluate(step * (float)k);

                //Debug.Log(r +" "+c+" "+step);

                _heightMap[i, k] = r * c * height;

                if (!up) _heightMap[i, k] *= -1f;
            }
        }

        FixToGround();
    }
}
