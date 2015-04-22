using UnityEngine;
using System.Collections.Generic;
using SIDE = QuadMatrix<TileData>.Node.SIDE;

public class DoorPlacer : Manager<DoorPlacer>, IActivatable
{
	#region Interface Variables
	#endregion
	
	#region Instance Variables
    private bool mIsActive = false;

    private SIDE mRotation = SIDE.RIGHT;
    private List<GameObject> mDoorPrefabs;

    private GameObject mSelectionPrefab;
    private PlaceableDoor mSelection;
    private SpriteRenderer mSelectionSR;
    private QuadMatrix<TileData>.Node[] mExisted;

    protected InputManager mIM;
    protected TileCursor mTileCursor;
    protected EpochMap mEM;
    protected DoorManager mDM;
    protected TileFinder mTF;
    protected StaticWallManager mSWM;

    private Vector3 mRadius;

    private Utilities.IntPair mOldCursor;
    private Vector3 mBase;

    private float mOffsetAngle;

    private int mActivationDelay = 0;

    private bool mOverWall = false;
	#endregion
	
	protected override void Awaken()
	{
        mExisted = new QuadMatrix<TileData>.Node[2];
        mExisted[0] = null;
        mExisted[1] = null;

        mDoorPrefabs = new List<GameObject>();

        mDoorPrefabs.Add(Resources.Load<GameObject>("Prefabs/Doors/StandardDoor"));
        mDoorPrefabs.Add(Resources.Load<GameObject>("Prefabs/Doors/DirectionalDoor"));
        mDoorPrefabs.Add(Resources.Load<GameObject>("Prefabs/Doors/EntranceDoor"));
        mDoorPrefabs.Add(Resources.Load<GameObject>("Prefabs/Doors/ExitDoor"));
        mDoorPrefabs.Add(Resources.Load<GameObject>("Prefabs/Doors/PowerCoreDoor"));
        mDoorPrefabs.Add(Resources.Load<GameObject>("Prefabs/Doors/CheckpointDoor"));
        mDoorPrefabs.Add(Resources.Load<GameObject>("Prefabs/Doors/TeleporterDoor"));
	}

    protected override void Initialize()
    {
        mIM = InputManager.Get();
        mTileCursor = TileCursor.Get();
        mEM = EpochMap.Get();
        mTF = TileFinder.Get();
        mDM = DoorManager.Get();
        mSWM = StaticWallManager.Get();

        Vector2 tileSize = mEM.getTileSize();
        mRadius.x = tileSize.x * 1.5f;
        mRadius.y = tileSize.y * .5f;
        mOffsetAngle = Mathf.Atan(mRadius.y / mRadius.x);
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

        mSelectionPrefab = mDoorPrefabs[0];
        mSelection = (GameObject.Instantiate(mDoorPrefabs[0]) as GameObject).GetComponent<PlaceableDoor>();
        mSelectionSR = mSelection.GetComponent<SpriteRenderer>();

        mBase = mTileCursor.getCursor();
        processSelection();

        Vector3 pos = new Vector3(1, 0, 0) * mRadius.magnitude;
        pos += mBase;

        mSelection.transform.position = pos;
        mSelection.transform.localEulerAngles = new Vector3(0, 0, 0);

        mOldCursor = mTileCursor.getLogicalCursor().floorToEven();
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
        selectDoor();
        rotateDoor();
        updatePosition();

        render();

        placeSelection();
    }

    private void selectDoor()
    {
        if (mActivationDelay < 5)
        {
            mActivationDelay++;
            return;
        }

        for (int i = 0; i < mDoorPrefabs.Count && i < mIM.toolSelection.Count; i++)
        {
            if (mIM.toolSelection[i].getUp())
            {
                GameObject.Destroy(mSelection.gameObject);
                mSelectionPrefab = mDoorPrefabs[i];
                mSelection = (GameObject.Instantiate(mDoorPrefabs[i]) as GameObject).GetComponent<PlaceableDoor>();
                mSelectionSR = mSelection.GetComponent<SpriteRenderer>();
                mSelectionSR.color = Color.red;
                break;
            }
        }
    }
    private void rotateDoor()
    {
        if (!mIM.alternateInput.getUp())
            return;

        switch (mRotation)
        {
            case SIDE.RIGHT:
                mRotation = SIDE.TOP;
                break;
            case SIDE.TOP:
                mRotation = SIDE.RIGHT;
                break;
        }
    }

    private void updatePosition()
    {
        Utilities.IntPair curCursor = mTileCursor.getLogicalCursor().floorToEven();
        Utilities.IntPair cursorDelta = curCursor - mOldCursor;
        if (!cursorDelta.equals(0, 0))
        {
            mOldCursor.assign(curCursor);
            mBase = mTileCursor.toTilePos(mOldCursor);
            processSelection();
        }

        float angle = 0;
        float rotAngle = 0;
        switch (mRotation)
        {
            case SIDE.RIGHT:
                angle = 0;
                rotAngle = mOffsetAngle;
                break;
            case SIDE.TOP:
                angle = 90;
                rotAngle = Mathf.PI / 2 - mOffsetAngle;
                break;
        }

        Vector3 pos = new Vector3(Mathf.Cos(rotAngle), Mathf.Sin(rotAngle), 0) * mRadius.magnitude;
        //Debug.Log(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * mRadius.magnitude);
        pos += mBase;

        mSelection.transform.position = pos;
        mSelection.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
    private void processSelection()
    {
        for (int i = 0; i < mExisted.Length; i++)
        {
            StaticWallPlacer.addStaticWallToNode(mExisted[i]);
            mExisted[i] = null;
        }

        TileData tile = mTileCursor.getDetectedTile();
        if (tile == null || !tile.isWall())
        {
            mOverWall = false;
            return;
        }

        PlaceableStaticWall baseWall = tile.getWall();
        PlaceableStaticWall adjWall = baseWall.getStaticWall(mRotation);

        if (adjWall == null)
        {
            mOverWall = false;
            return;
        }

        mExisted[0] = baseWall.bottomLeft.quadNode;
        mExisted[1] = adjWall.bottomLeft.quadNode;

        foreach (QuadMatrix<TileData>.Node node in mExisted)
            StaticWallPlacer.removeStaticWallFromNode(node);

        mOverWall = true;
    }

    private void render()
    {
        if (mOverWall)
            mSelectionSR.color = Color.white;
        else
            mSelectionSR.color = Color.red;
    }

    private void placeSelection()
    {
        if (!mIM.primaryPlace.getUp() || !mOverWall)
            return;

        mDM.register(mSelection);
        TileData[] tiles = new TileData[8];
        switch (mRotation)
        {
            case SIDE.RIGHT:
                tiles[0] = mTF.findTile(mOldCursor);
                tiles[1] = tiles[0].getTile(SIDE.RIGHT);
                tiles[2] = tiles[1].getTile(SIDE.RIGHT);
                tiles[3] = tiles[2].getTile(SIDE.RIGHT);
                tiles[4] = tiles[0].getTile(SIDE.TOP);
                tiles[5] = tiles[4].getTile(SIDE.RIGHT);
                tiles[6] = tiles[5].getTile(SIDE.RIGHT);
                tiles[7] = tiles[6].getTile(SIDE.RIGHT);
                break;
            case SIDE.TOP:
                tiles[0] = mTF.findTile(mOldCursor);
                tiles[1] = tiles[0].getTile(SIDE.TOP);
                tiles[2] = tiles[1].getTile(SIDE.TOP);
                tiles[3] = tiles[2].getTile(SIDE.TOP);
                tiles[4] = tiles[0].getTile(SIDE.RIGHT);
                tiles[5] = tiles[4].getTile(SIDE.TOP);
                tiles[6] = tiles[5].getTile(SIDE.TOP);
                tiles[7] = tiles[6].getTile(SIDE.TOP);
                break;
        }
        //mSelection.attachTiles(tiles);
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = null;

        mSelection = null;

        for (int i = 0; i < mExisted.Length; i++)
        {
            mSWM.remove(mExisted[i].getPosition());
            mExisted[i] = null;
        }

        mSelection = (GameObject.Instantiate(mSelectionPrefab) as GameObject).GetComponent<PlaceableDoor>();
        mSelectionSR = mSelection.GetComponent<SpriteRenderer>();
    }
	#endregion
}
