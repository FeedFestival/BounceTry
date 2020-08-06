using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public Transform StartMapPoint;
    public Transform GeneratedMap;
    // privates
    public int currentYPoint;
    public int pointsUp;
    // constants
    private readonly float _xLength = 20f;
    private int _depthIndex = 0;

    private float minGridColWidth = 5f;
    private float maxGridColWidth = 13f;

    #region Public  Methods

    public void Init()
    {
        // Create initial map grids.
        pointsUp = 18;
        currentYPoint = 0;

        //CreateGridPlaneRow();

        // CreateGridRows();
    }

    public int AddGridRow(bool infinite = false)
    {
        int yScale = GetGridRowHeight(infinite: infinite);
        var xPos = StartMapPoint.transform.position.x;
        var yPos = currentYPoint + StartMapPoint.transform.position.y;

        GridRow row = CreateGridRow(xPos, yPos, yScale);
        row.gameObject.name = _depthIndex + "_row[" + (currentYPoint + yScale) + "]";
        row.Index = _depthIndex;

        int[] columnsWidths = GenerateColWidths();
        var cPosX = row.transform.position.x;
        for (var i = 0; i < columnsWidths.Length; i++)
        {
            GridColumn gC;
            if (columnsWidths.Length == 2 && i == 1)
            {
                cPosX = (_xLength - columnsWidths[i]) + row.transform.position.x;
                gC = CreateGridColumn(_depthIndex, i,
                        cPosX, currentYPoint, yScale, columnsWidths[i], row.transform);

                row.AddColumn(gC);
                break;
            }
            gC = CreateGridColumn(
                    _depthIndex, i,
                    cPosX, currentYPoint, yScale, columnsWidths[i], row.transform);

            row.AddColumn(gC);
            cPosX = cPosX + columnsWidths[i];
        }

        if (infinite == false && currentYPoint < pointsUp)
            currentYPoint = currentYPoint + yScale;
        _depthIndex++;

        return yScale;
    }

    public void DescendRows(int yScale, IEnumerable<GridRow> rows)
    {
        foreach (var r in rows)
        {
            LeanTween.move(r.gameObject, new Vector3(r.transform.position.x, r.transform.position.y - yScale, r.transform.position.z), Game.Instance.TimeForRowsToDescend);
        }
    }

    #endregion

    #region Private Methods

    private void CreateGridRows()
    {
#pragma warning disable 0219
        int n = 0;
#pragma warning restore 0219
        _depthIndex = 0;
        for (n = 0; currentYPoint < pointsUp;)
        {
            AddGridRow();
        }
    }

    private GridRow CreateGridRow(float xPos, float yPos, float yScale)
    {
        var gR = Game.Instance.GetAvailableRow();

        gR.transform.SetParent(GeneratedMap);
        gR.transform.position = new Vector3(xPos, yPos, 0);
        gR.transform.localScale = new Vector3(_xLength, yScale, 1);
        return gR;
    }

    private GridColumn CreateGridColumn(int rowIndex, int index,
        float xPos, float yPos, float yScale, float xScale, Transform tr)
    {
        var gC = Game.Instance.GetAvailableGridColumn((int)yScale, (int)xScale);

        gC.RowIndex = rowIndex;
        gC.Index = index;

        gC.transform.SetParent(GeneratedMap);
        gC.transform.position = new Vector3(xPos, yPos, 0);
        gC.transform.localScale = new Vector3(xScale, yScale, 1);
        gC.name = "[" + rowIndex + "]_" + index + "_gC";
        return gC;
    }

    private int[] GenerateColWidths()
    {
        var widths = new List<int>();
        var curLength = 0;
#pragma warning disable 0219
        int m = 0;
#pragma warning restore 0219
        for (m = 0; curLength < (int)_xLength;)
        {
            var value = (int)UnityEngine.Random.Range(minGridColWidth, maxGridColWidth);
            if (curLength + value > _xLength)
            {
                if (widths.Count == 2)  // TODO: if the length is 2, then leave space in beetween the columns.
                    break;
                value = (int)_xLength - curLength;

                if (value < minGridColWidth)
                    break;
            }
            curLength = curLength + value;
            widths.Add(value);
        }
        //widths.Clear();
        //widths.Add(13);
        return widths.ToArray();
    }

    private int GetGridRowHeight(bool infinite = false)
    {
        var value = 0;
        if (currentYPoint == 0)
        {
            value = 2;
        }
        else
        {
            value = (int)UnityEngine.Random.Range(minGridColWidth - 1, maxGridColWidth);
            //value = 13;
        }
        if (infinite)
            pointsUp = pointsUp + value;
        if (currentYPoint + value > pointsUp)
        {
            value = pointsUp - currentYPoint;
        }
        return value;
    }

    #endregion
}
