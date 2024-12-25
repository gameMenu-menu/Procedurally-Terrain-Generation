using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Height Map Data", menuName = "Data/Height Map")]
public class HeightMapData : TerrainShapeData
{
    public Texture2D heightMap;

    public int smoothLength, smoothCount;

    public float smoothStrength;
    protected override void SetMap()
    {
        float ratio = Mathf.Sqrt(heightMap.GetPixels().Length) / (float)radius;

        _heightMap = new float[radius + smoothLength * 2, radius + smoothLength * 2];

        for (int i = smoothLength; i < radius; i++)
        {
            for (int k = smoothLength; k < radius; k++)
            {
                int index1 = (int)( (float) i * ratio);
                int index2 = (int)( (float) k * ratio);

                Color c = heightMap.GetPixel(index1, index2);

                float h = c.grayscale * height;

                _heightMap[i, k] = h;
            }
        }
        FixToGround();

        for(int i=0; i<smoothCount; i++)
        {
            SmoothHeight();
        }
    }

    void SmoothHeight()
    {
        for(int i=smoothLength + 1; i > 0; i--)
        {
            for(int k=0; k<_heightMap.GetLength(0); k++)
            {
                _heightMap[i-1, k] = Mathf.Lerp(_heightMap[i, k], _heightMap[i - 1, k], smoothStrength);
            }
        }

        for (int i = radius - 1; i < _heightMap.GetLength(0) - 1; i++)
        {
            for (int k = 0; k < _heightMap.GetLength(0); k++)
            {
                _heightMap[i + 1, k] = Mathf.Lerp(_heightMap[i, k], _heightMap[i + 1, k], smoothStrength);
            }
        }

        for (int k = smoothLength + 1; k > 0; k--)
        {
            for (int i = 0; i < _heightMap.GetLength(0); i++)
            {
                _heightMap[i, k - 1] = Mathf.Lerp(_heightMap[i, k], _heightMap[i, k - 1], smoothStrength);
            }
        }

        for (int k = radius - 1; k < _heightMap.GetLength(0) - 1; k++)
        {
            for (int i = 0; i < _heightMap.GetLength(0); i++)
            {
                _heightMap[i, k + 1] = Mathf.Lerp(_heightMap[i, k], _heightMap[i, k + 1], smoothStrength);
            }
        }

    }

    void SmoothHeight2()
    {
        int totalCount = _heightMap.GetLength(0);

        List<int[]> openList = new List<int[]>();
        List<int[]> closedList = new List<int[]>();

        openList.Add(new int[] { totalCount / 2, totalCount / 2 });

        closedList.Add(openList[0]);

        int min = smoothLength + 2;
        int max = radius - 2;

        while (openList.Count > 0)
        {
            int[] current = openList[0];
            openList.RemoveAt(0);

            int row = current[0];
            int column = current[1];

            if (row > min && column > min && row < max && column < max) continue;


            int rowMinDiff = min - row;
            int columnMinDiff = min - column;

            int rowMaxDiff = row - max;
            int columnMaxDiff = column - max;

            if (rowMaxDiff < 0) rowMaxDiff = 0;
            if (rowMinDiff < 0) rowMinDiff = 0;

            if (columnMaxDiff < 0) columnMaxDiff = 0;
            if (columnMinDiff < 0) columnMinDiff = 0;

            if(rowMaxDiff == 0 && rowMinDiff == 0 && columnMaxDiff == 0 && columnMinDiff == 0)
            {

            }
            else if (rowMaxDiff >= rowMinDiff && rowMaxDiff >= columnMaxDiff && rowMaxDiff >= columnMinDiff)
            {
                
                _heightMap[row, column] = Mathf.Lerp(_heightMap[row, column], _heightMap[row + 1, column], smoothStrength);
            }
            else if (rowMinDiff >= rowMaxDiff && rowMinDiff >= columnMaxDiff && rowMinDiff >= columnMinDiff)
            {
                _heightMap[row, column] = Mathf.Lerp(_heightMap[row, column], _heightMap[row - 1, column], smoothStrength);
            }
            else if (columnMaxDiff >= rowMinDiff && columnMaxDiff >= rowMaxDiff && columnMaxDiff >= columnMinDiff)
            {
                _heightMap[row, column] = Mathf.Lerp(_heightMap[row, column], _heightMap[row, column + 1], smoothStrength);
            }
            else if (columnMinDiff >= rowMaxDiff && columnMinDiff >= columnMaxDiff && columnMinDiff >= rowMinDiff)
            {
                _heightMap[row, column] = Mathf.Lerp(_heightMap[row, column], _heightMap[row, column - 1], smoothStrength);
            }


            List<int[]> neighbours = Neighbours(current, closedList);

            for(int i=0; i<neighbours.Count; i++)
            {
                int[] neighbour = neighbours[i];

                openList.Add(neighbour);
                closedList.Add(neighbour);

                /*continue;

                

                int row = neighbour[0];
                int column = neighbour[1];

                if (row > min && column > min && row < max && column < max) continue;

                //_heightMap[row, column] = Mathf.Lerp(_heightMap[row, column], _heightMap[current[0], current[1]], smoothStrength);


                int rowMinDiff = min - row;
                int columnMinDiff = min - column;

                int rowMaxDiff = row - max;
                int columnMaxDiff = column - max;

                if (rowMaxDiff < 0) rowMaxDiff = 0;
                if (rowMinDiff < 0) rowMinDiff = 0;

                if (columnMaxDiff < 0) columnMaxDiff = 0;
                if (columnMinDiff < 0) columnMinDiff = 0;

                if(rowMaxDiff > rowMinDiff && rowMaxDiff > columnMaxDiff && rowMaxDiff > columnMinDiff)
                {
                    _heightMap[row, column] = Mathf.Lerp(_heightMap[row, column], _heightMap[current[0], current[1]], smoothStrength);
                }*/



                /*if (neighbour[0] <= smoothLength + 2)
                {
                    _heightMap[neighbour[0], neighbour[1]] = Mathf.Lerp(_heightMap[neighbour[0], neighbour[1]], _heightMap[current[0], current[1]], smoothStrength);
                }

                if (neighbour[0] >= radius - 2)
                {
                    _heightMap[neighbour[0], neighbour[1]] = Mathf.Lerp(_heightMap[neighbour[0], neighbour[1]], _heightMap[current[0], current[1]], smoothStrength);
                }

                if (neighbour[1] <= smoothLength + 2)
                {
                    _heightMap[neighbour[0], neighbour[1]] = Mathf.Lerp(_heightMap[neighbour[0], neighbour[1]], _heightMap[current[0], current[1]], smoothStrength);
                }

                if (neighbour[1] >= radius - 2)
                {
                    _heightMap[neighbour[0], neighbour[1]] = Mathf.Lerp(_heightMap[neighbour[0], neighbour[1]], _heightMap[current[0], current[1]], smoothStrength);
                }*/
            }

        }
    }

    protected override void FixToGround()
    {
        float minimum = 99999f;

        for (int i = smoothLength; i < radius; i++)
        {
            float check = _heightMap[i, smoothLength];

            if (check < minimum) minimum = check;

            check = _heightMap[i, radius - 1];

            if (check < minimum) minimum = check;
        }

        for (int i = smoothLength; i < radius; i++)
        {
            float check = _heightMap[smoothLength, i];

            if (check < minimum) minimum = check;

            check = _heightMap[radius - 1, i];

            if (check < minimum) minimum = check;
        }

        for (int i = smoothLength; i < radius; i++)
        {
            for (int k = smoothLength; k < radius; k++)
            {

                _heightMap[i, k] -= minimum;
            }
        }
    }

    List<int[]> Neighbours(int[] index, List<int[]> closed)
    {
        int i = index[0];
        int k = index[1];

        int[] i1 = new int[] { i - 1, k };
        int[] i2 = new int[] {i + 1, k};
        int[] i3 = new int[] {i, k - 1};
        int[] i4 = new int[] {i, k + 1};

        int[] i5 = new int[] { i - 1, k - 1 };
        int[] i6 = new int[] { i + 1, k - 1 };
        int[] i7 = new int[] { i - 1, k + 1 };
        int[] i8 = new int[] { i + 1, k + 1 };

        List<int[]> n = new List<int[]>();

        if (MayAdd(i4, closed)) n.Add(i4);
        if (MayAdd(i3, closed)) n.Add(i3);
        if (MayAdd(i2, closed)) n.Add(i2);
        if (MayAdd(i1, closed)) n.Add(i1);
        
        
        

        /*if (MayAdd(i5, closed)) n.Add(i5);
        if (MayAdd(i6, closed)) n.Add(i6);
        if (MayAdd(i7, closed)) n.Add(i7);
        if (MayAdd(i8, closed)) n.Add(i8);*/

        return n;
    }

    bool MayAdd(int[] index, List<int[]> closed)
    {
        return closed.FindIndex(item => item[0] == index[0] && item[1] == index[1]) == -1 && index[0] > 1 && index[1] > 1 && index[0] < _heightMap.GetLength(0) - 1 && index[1] < _heightMap.GetLength(1) - 1;
    }

    

    
}
