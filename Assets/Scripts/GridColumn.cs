using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridColumn : MonoBehaviour
{
    public int Height;
    public int Width;
    public bool IsPredefined;
    public int NumberOfBlocks;
    public int CurrentBlocks;
    public int RowIndex;
    public int Index;
    public bool IsAvailable;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void YouAreDead()
    {
        transform.SetParent(null);
        IsAvailable = true;

        // move the grid out of view.
        transform.position = new Vector3(transform.position.x, 25, transform.position.z);
    }
}
