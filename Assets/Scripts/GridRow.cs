using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRow : MonoBehaviour
{
    public int Index;
    public bool IsAvailable;

    public int CurrentColumns;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddColumn(GridColumn gC)
    {
        // First we instantiate the Boxes of this GridColumn Before we make this a parent of the GridRow.
        if (gC.IsPredefined == false)
        {
            var centerPoint = GetCenter(
                gC.transform.position,
                gC.Width, gC.Height
                );

            //Debug.Log("Column: " + gC.Width + "," + gC.Height);

            if (gC.Width >= 7 && gC.Height >= 7)
            {
                bool createLevel1 = true, createLevel2 = false, createLevel3 = false;

                CreateBlock("Circle4", gC.transform, centerPoint, gC.RowIndex, gC.Index);
                gC.NumberOfBlocks = 1;
                
                if (gC.Width > 9 && gC.Height > 9)
                {
                    createLevel2 = true;
                }
                if (gC.Width > 11 && gC.Height > 11)
                {
                    createLevel3 = true;
                }

                if (createLevel1 == true)
                {
                    var blocksCreated = CreateFirstLevelCircle(gC.transform, centerPoint, gC.RowIndex, gC.Index);
                    gC.NumberOfBlocks += blocksCreated;
                }
                if (createLevel2 == true)
                {
                    var blocksCreated = CreateSecondLevelCircle(gC.transform, centerPoint, gC.RowIndex, gC.Index);
                    gC.NumberOfBlocks += blocksCreated;
                }
                if (createLevel3 == true)
                {
                    var blocksCreated = CreateThreeLevelCircle(gC.transform, centerPoint, gC.RowIndex, gC.Index);
                    gC.NumberOfBlocks += blocksCreated;
                }
                gC.CurrentBlocks = gC.NumberOfBlocks;
            }
            else
            {
                CreateBlock("Cube", gC.transform, centerPoint, gC.RowIndex, gC.Index);
                gC.NumberOfBlocks = 1;
                gC.CurrentBlocks = gC.NumberOfBlocks;
            }
        }

        gC.transform.SetParent(transform);
        CurrentColumns++;
    }

    private int CreateFirstLevelCircle(Transform t, Vector3 c, int rI, int cI)
    {
        var blocksCreated = 0;
        var circleType = 2;
        for (var i = 1; i <= 4; i++)
        {
            var boolValue = (UnityEngine.Random.Range(0, 2) == 0);
            if (boolValue)
            {
                blocksCreated++;
                var blockType = "Circle" + circleType + "_" + i;
                CreateBlock(blockType, t, c, rI, cI);
            }
        }
        return blocksCreated;
    }

    private int CreateSecondLevelCircle(Transform t, Vector3 c, int rI, int cI)
    {
        var blocksCreated = 0;
        var circleType = 1;
        for (var i = 1; i <= 3; i++)
        {
            var boolValue = (UnityEngine.Random.Range(0, 2) == 0);
            if (boolValue)
            {
                blocksCreated++;
                var blockType = "Circle" + circleType + "_" + i;
                CreateBlock(blockType, t, c, rI, cI);
            }
        }
        return blocksCreated;
    }

    private int CreateThreeLevelCircle(Transform t, Vector3 c, int rI, int cI)
    {
        var blocksCreated = 0;
        for (var i = 1; i <= 3; i++)
        {
            var boolValue = (UnityEngine.Random.Range(0, 2) == 0);
            if (boolValue)
            {
                blocksCreated++;
                var blockType = "Circle" + i;
                CreateBlock(blockType, t, c, rI, cI);
            }
        }
        return blocksCreated;
    }

    private Block CreateBlock(string blockType, Transform t, Vector3 cPoint, int rowI, int colI)
    {
        Block block = Game.Instance.GetAvailableBlock(blockType);
        block.transform.position = cPoint;
        block.transform.SetParent(t);

        if (block.gameObject.activeSelf == false)
            block.gameObject.SetActive(true);

        block.IsAvailable = false;

        //
        block.RowIndex = rowI;
        block.ColumnIndex = colI;
        return block;
    }

    private Vector3 GetCenter(Vector3 pos, float scaleX, float scaleY)
    {
        var oppositePoint = new Vector3(pos.x + scaleX, pos.y + scaleY, pos.z);
        var centerPoint = Vector3.Lerp(pos, oppositePoint, 0.5f);
        return centerPoint;
    }
}
