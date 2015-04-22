using UnityEngine;
using System.Collections.Generic;

public class ObjectPlacer : Manager<ObjectPlacer>, IActivatable
{
	#region Interface Variables
	#endregion

    #region Instance Variables
    private bool mIsActive = false;

    private GameObject mSelectionPrefab;
    private PlaceableObject mSelection;
    private SpriteRenderer mSelectionSR;

    protected InputManager mIM;
    protected TileCursor mTileCursor;
    protected TileFinder mTF;
    protected EpochMap mEM;

    private Vector2 mTileSize;

    private Utilities.IntPair mOldCursor;

    private int mActivationDelay = 0;

    private bool mIsPlaceable = false;
	#endregion
	
	protected override void Awaken()
	{
        mSelectionPrefab = Resources.Load<GameObject>("Prefabs/Tools/PlaceableObject");
	}

    protected override void Initialize()
    {
        mIM = InputManager.Get();
        mTileCursor = TileCursor.Get();
        mTF = TileFinder.Get();
        mEM = EpochMap.Get();

        mTileSize = mEM.getTileSize();
    }

    protected void Update()
    {
        if (mIsActive)
            updateSelection();
    }

    #region Interface Methods
    public void activate()
    {
        mIsActive = true;

        mActivationDelay = 0;

        mSelection = (GameObject.Instantiate(mSelectionPrefab) as GameObject).GetComponent<PlaceableObject>();
        mSelectionSR = mSelection.GetComponent<SpriteRenderer>();

        mOldCursor = mTileCursor.getLogicalCursor(true);

        mSelection.transform.position = mTileCursor.getCursor(true);
    }
    public void deactivate()
    {
        mIsActive = false;

        if (mSelection != null)
        {
            GameObject.Destroy(mSelection.gameObject);
            mSelection = null;
        }
    }
	#endregion

    #region Instance Methods
    private void updateSelection()
    {
        updatePosition();

        render();

        placeSelection();
    }
    private void updatePosition()
    {
        Utilities.IntPair curCursor = mTileCursor.getLogicalCursor(true);
        Utilities.IntPair cursorDelta = curCursor - mOldCursor;
        if (!cursorDelta.equals(0, 0))
        {
            mOldCursor.assign(curCursor);
            processSelection();
        }

        mSelection.transform.position = mTileCursor.getCursor(true);
    }
    private void processSelection()
    {
        TileData[] tiles = mTileCursor.getDetectedTiles();

        mIsPlaceable = true;

        foreach (TileData tile in tiles)
        {
            if (tile == null || tile.isWall() || tile.hasAttached())
            {
                mIsPlaceable = false;
                break;
            }
        }
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
        
        //mSelection.attach(mTileCursor.getDetectedTiles());
        mSelection = null;

        mSelection = (GameObject.Instantiate(mSelectionPrefab) as GameObject).GetComponent<PlaceableObject>();
        mSelectionSR = mSelection.GetComponent<SpriteRenderer>();
    }
	#endregion
}
