using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BlockType
{
    Block,
    SidePerimeter,
    RedZone,
    Proppeler
}

public class Game : MonoBehaviour
{
    private static Game _game;
    public static Game Instance
    {
        get { return _game; }
    }
    //
    public MapController MapController;
    public Sphere Sphere;
    public ProppelerController ProppelerController;
    public GameObject Proppeler;
    public CanvasController CanvasController;

    public static Object _brickPrefab;
    public static Object _brick2Prefab;
    public static Object _cubePrefab;

    [Header("Global Game Options")]
    public float TimeForRowsToDescend = 1f;

    [Header("Color Definitions")]
    public Color red;
    public Color blue;
    public Color green;
    public Color yellow;

    public Color AmbientColor;
    public Color DamagedAmbientColor;

    //  object pools
    private List<Block> _blockPool;
    private List<GridColumn> _gridColumnPool;
    private List<GridRow> _gridRowPool;



    #region Public Methods

    private void Awake()
    {
        _game = this;

        _brick2Prefab = Resources.Load("Prefabs/Brick2", typeof(GameObject));
        _brickPrefab = Resources.Load("Prefabs/Brick", typeof(GameObject));
        _cubePrefab = Resources.Load("Prefabs/Cube", typeof(GameObject));
    }

    public Block GetAvailableBlock(string blockType)
    {
        if (_blockPool == null)
            _blockPool = new List<Block>();
        var block = _blockPool.FirstOrDefault(b => b.IsAvailable == true && b.TypeName == blockType);
        if (block == null)
        {
            Object prefab = Resources.Load("Prefabs/" + blockType, typeof(GameObject));
            GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(null);
            if (blockType.Contains("Circle") == false)
                go.transform.localEulerAngles = new Vector3(90f, 0, 0);
            block = go.GetComponent<Block>();
            block.TypeName = blockType;
            _blockPool.Add(block);
        }
        block.Init();
        return block;
    }

    internal GridRow GetAvailableRow()
    {
        if (_gridRowPool == null)
            _gridRowPool = new List<GridRow>();
        var gR = _gridRowPool.FirstOrDefault(g => g.IsAvailable == true);
        if (gR == null)
        {
            GameObject go = Instantiate(Resources.Load("Prefabs/GridRow", typeof(GameObject)),
                                Vector3.zero, Quaternion.identity
                                ) as GameObject;
            gR = go.GetComponent<GridRow>();
            _gridRowPool.Add(gR);
        }
        gR.IsAvailable = false;
        return gR;
    }

    public GridColumn GetAvailableGridColumn(int y, int x)
    {
        if (_gridColumnPool == null)
            _gridColumnPool = new List<GridColumn>();
        var gC = _gridColumnPool.FirstOrDefault(g => g.IsAvailable == true && g.Height == y && g.Width == x);
        if (gC == null)
        {
            var gridColumnVariation = "Prefabs/GridColumn_" + y + "_" + x;
            var isPredefined = true;
            GameObject go;
            try
            {
                go = Instantiate(Resources.Load(gridColumnVariation, typeof(GameObject)),
                                    Vector3.zero, Quaternion.identity
                                    ) as GameObject;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                go = Instantiate(Resources.Load("Prefabs/GridColumn", typeof(GameObject)),
                                Vector3.zero, Quaternion.identity
                                ) as GameObject;
                isPredefined = false;
            }
            gC = go.GetComponent<GridColumn>();
            _gridColumnPool.Add(gC);
            gC.IsPredefined = isPredefined;
            gC.Height = y;
            gC.Width = x;
        }
        gC.IsAvailable = false;
        return gC;
    }

    public void CheckIfRowIsDestroyed(int rowIndex, int colIndex, bool dangerZone = false)
    {
        var col = _gridColumnPool.FirstOrDefault(g => g.RowIndex == rowIndex && g.Index == colIndex);

        col.CurrentBlocks--;

        if (col.CurrentBlocks <= 0)
        {
            col.YouAreDead();

            var row = _gridRowPool.FirstOrDefault(r => r.Index == rowIndex);
            row.CurrentColumns--;

            if (row.CurrentColumns <= 0)
            {
                row.transform.position = new Vector3(-10, 25, 0);
                row.IsAvailable = true;

                if (dangerZone == false)
                    AddNewRow(rowIndex, (int)row.transform.localScale.y);

                // if it comes form danger zone, don't add new row.
                GameScore.Instance.ResetHits();

                //row.transform.position = new Vector3(-10, 25, 0);
                //row.IsAvailable = true;
            }
        }
    }

    private List<RowMover> rowMover;

    class RowMover
    {
        public int rowIndex;
        public int deletedRowHeight;
        //public int yScale;
        public RowMover(int index, int rowHeight)
        {
            rowIndex = index;
            deletedRowHeight = rowHeight;
            //yScale = scale;
        }
    }

    public void AddNewRow(int rowIndex = 0, int deletedRowHeight = 0)
    {
        if (rowMover == null)
            rowMover = new List<RowMover>();

        rowMover.Add(new RowMover(rowIndex, deletedRowHeight));

        if (_queueAddRow == null)
        {
            _queueAddRow = QueueAddRow();
            StartCoroutine(_queueAddRow);
        }
    }

    private IEnumerator _queueAddRow;
    private IEnumerator QueueAddRow()
    {
        var yScale = MapController.AddGridRow(infinite: true);
        var rowM = rowMover.FirstOrDefault();
        if (rowM == null)
            yield return null;
        if (rowM.rowIndex == 0)
        {
            MapController.DescendRows(yScale, _gridRowPool);
        }
        else
        {
            MapController.DescendRows(yScale, _gridRowPool.Where(r => r.Index > rowM.rowIndex));
            if (rowM.deletedRowHeight < yScale)
            {
                var valueToMoveTheBottomRows = yScale - rowM.deletedRowHeight;
                MapController.DescendRows(valueToMoveTheBottomRows, _gridRowPool.Where(r => r.Index < rowM.rowIndex));
            }
        }
        yield return new WaitForSeconds(TimeForRowsToDescend);

        rowMover.Remove(rowM);

        if (rowMover.Count > 0)
        {
            _queueAddRow = QueueAddRow();
            StartCoroutine(_queueAddRow);
        }
        else
        {
            _queueAddRow = null;
        }
    }

    #endregion

    #region Events

    void Start()
    {
        MapController.Init();
        ProppelerController.Init();
    }

    #endregion

    public int RowIndexToTest;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            var blocks = Game.Instance._blockPool.Where(b => b.RowIndex == RowIndexToTest).ToArray();
            foreach (var b in blocks)
            {
                b.Hit();
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            AddNewRow();
        }
    }
}
