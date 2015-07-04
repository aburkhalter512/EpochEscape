using UnityEngine;
using System.Collections.Generic;

public class DynamicWallPlacer// : Manager<DynamicWallPlacer>, IActivatable
{
	/*#region Interface Variables
	#endregion
	
	#region Instance Variables
    private bool mIsActive = false;

    private List<GameObject> mDynamicWallPrefabs;

    private GameObject mSelectionPrefab;
    private PlaceableDynamicWall mSelection;
    private SpriteRenderer mSelectionSR;

    protected InputManager mIM;
    protected TileCursor mTC;
    protected EpochMap mEM;
    protected DynamicWallManager mDWM;
    protected TileFinder mTF;

    private Utilities.IntPair mOldCursor;
    private Vector3 mBase;
    private Vector3 mOffsetPos;

    private int mActivationDelay = 0;

    private bool mIsPlaceable = false;
	#endregion
	
	protected override void Awaken()
	{
        mDynamicWallPrefabs = new List<GameObject>();

        mDynamicWallPrefabs.Add(Resources.Load<GameObject>("Prefabs/DynamicWalls/SlidingWall"));
        mDynamicWallPrefabs.Add(Resources.Load<GameObject>("Prefabs/DynamicWalls/RotatingWall"));
	}

    protected override void Initialize()
    {
        mIM = InputManager.Get();
        mTC = TileCursor.Get();
        mEM = EpochMap.Get();
        mDWM = DynamicWallManager.Get();
        mTF = TileFinder.Get();

        mOffsetPos = Utilities.toVector3(mEM.getTileSize()) / 2;
    }
	
	protected void Update ()
    {
        if (mIsActive)
            updateSelection();
	}
	
	#region Interface Methods
    public void activate()
    {
        mIsActive = true;

        mActivationDelay = 0;

        mSelectionPrefab = mDynamicWallPrefabs[0];
        mSelection = (GameObject.Instantiate(mDynamicWallPrefabs[0]) as GameObject).GetComponent<PlaceableDynamicWall>();
        mSelectionSR = mSelection.GetComponent<SpriteRenderer>();

        mOldCursor = mTC.getLogicalCursor().floorToEven();

        mBase = mTC.toTilePos(mOldCursor);
        processSelection();

        mSelection.transform.position = mBase;
    }
    public void deactivate()
    {
        mIsActive = false;

        if (mSelection != null)
        {
            mSelectionPrefab = null;
            GameObject.Destroy(mSelection.gameObject);
            mSelection = null;
        }
    }
	#endregion

    #region Instance Methods
    private void updateSelection()
    {
        selectObject();
        updatePosition();

        render();

        placeSelection();
    }

    private void selectObject()
    {
        if (mActivationDelay < 5)
        {
            mActivationDelay++;
            return;
        }

        for (int i = 0; i < mDynamicWallPrefabs.Count && i < mIM.toolSelection.Count; i++)
        {
            if (mIM.toolSelection[i].getUp())
            {
                GameObject.Destroy(mSelection.gameObject);
                mSelectionPrefab = mDynamicWallPrefabs[i];
                mSelection = (GameObject.Instantiate(mDynamicWallPrefabs[i]) as GameObject).GetComponent<PlaceableDynamicWall>();
                mSelectionSR = mSelection.GetComponent<SpriteRenderer>();
                mSelectionSR.color = Color.red;
                break;
            }
        }
    }

    private void updatePosition()
    {
        Utilities.IntPair curCursor = mTC.getLogicalCursor().floorToEven();
        Utilities.IntPair cursorDelta = curCursor - mOldCursor;
        if (!cursorDelta.equals(0, 0))
        {
            mOldCursor.assign(curCursor);
            mBase = mTC.toTilePos(mOldCursor);
            processSelection();
        }

        mSelection.transform.position = mOffsetPos + mBase;
    }
    private void processSelection()
    {
        mIsPlaceable = false;

        TileData tile = mTC.getDetectedTile();
        if (tile == null || tile.isWall())
            return;

        TileData[] tiles = new TileData[4];
        tiles[0] = tile;
        tiles[1] = tiles[0].getTile(QuadMatrix<TileData>.Node.SIDE.RIGHT);
        tiles[2] = tiles[0].getTile(QuadMatrix<TileData>.Node.SIDE.TOP);

        if (tiles[2] != null)
            tiles[3] = tiles[2].getTile(QuadMatrix<TileData>.Node.SIDE.RIGHT);

        bool tilesFound = true;
        foreach (TileData t in tiles)
        {
            if (t == null || t.isWall() || t.hasAttached())
                tilesFound = false;
        }

        if (!tilesFound)
            return;

        mIsPlaceable = true;
    }

    private void render()
    {
        if (mIsPlaceable)
            mSelectionSR.color = Color.white;
        else
            mSelectionSR.color = Color.red;
    }

    private void placeSelection()
    {
        if (!mIM.primaryPlace.getUp() || !mIsPlaceable)
            return;

        mDWM.register(mSelection);

        TileData[] tiles = new TileData[4];
        tiles[0] = mTF.findTile(mOldCursor);
        tiles[1] = tiles[0].getTile(QuadMatrix<TileData>.Node.SIDE.RIGHT);
        tiles[2] = tiles[0].getTile(QuadMatrix<TileData>.Node.SIDE.TOP);

        if (tiles[2] != null)
            tiles[3] = tiles[2].getTile(QuadMatrix<TileData>.Node.SIDE.RIGHT);

        mSelection.attachBase(tiles[0]);
        mSelection.registerBase();

        mSelection = null;

        mSelection = (GameObject.Instantiate(mSelectionPrefab) as GameObject).GetComponent<PlaceableDynamicWall>();
        mSelectionSR = mSelection.GetComponent<SpriteRenderer>();
    }
	#endregion*/
}
