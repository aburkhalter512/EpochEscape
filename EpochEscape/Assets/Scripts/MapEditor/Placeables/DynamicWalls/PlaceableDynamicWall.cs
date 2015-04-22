using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using SIDE = QuadMatrix<TileData>.Node.SIDE;

public abstract class PlaceableDynamicWall : MonoBehaviour, IPlaceable, ISerializable, IConnectable
{
	#region Interface Variables
    public GameObject selectionHighlighter;
	#endregion

    #region Instance Variables
    private string mID = "";

    private SpriteRenderer mSelection;
    protected bool mIsSelected;

    private SortedList<Utilities.IntPair, TileData[]> mTiles;
    private Dictionary<SIDE, TileData> mEndCaps;

    private TileData mBase;

    private static readonly float SELECTION_DELAY = .33f;
    protected float mCurrentDelay = 0.0f;

    protected InputManager mIM;
    protected EpochMap mEM;
    protected ChunkManager mCM;
    protected DynamicWallManager mDWM;

    private SpriteRenderer mSpriteRenderer;

    protected Vector3 mBasePos;

    private bool mResized = false;

    private static Texture2D[] mWallTextures;
	#endregion

    protected void Awake()
    {
        mTiles = new SortedList<Utilities.IntPair, TileData[]>(Utilities.IntPairComparer.Get());
        mEndCaps = new Dictionary<SIDE, TileData>();

        loadTextures();
    }

	protected void Start()
	{
        mSelection = selectionHighlighter.GetComponent<SpriteRenderer>();
        mSelection.gameObject.SetActive(false);

        mIM = InputManager.Get();
        mEM = EpochMap.Get();
        mCM = ChunkManager.Get();
        mDWM = DynamicWallManager.Get();

        mSpriteRenderer = GetComponent<SpriteRenderer>();
	}

    protected void Update()
    {
        if (mIsSelected)
            selectUpdate();
    }

    #region Interface Methods
    public string getID()
    {
        if (mID == "")
            mID = Utilities.generateUUID(this);

        return mID;
    }
    public void setID(string id)
    {
        mID = id;
    }

    public virtual IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
    {
        XmlElement dynamicwall = doc.CreateElement("dynamicwall");
        dynamicwall.SetAttribute("id", getID());
        dynamicwall.SetAttribute("type", getType());

        Utilities.IntPair basePos = mBase.getPosition();
        int east = (mEndCaps[SIDE.RIGHT].getPosition().first - basePos.first) / 2;
        dynamicwall.SetAttribute("east", east.ToString());

        int north = (mEndCaps[SIDE.TOP].getPosition().second - basePos.second) / 2;
        dynamicwall.SetAttribute("north", north.ToString());

        int west = (basePos.first - mEndCaps[SIDE.LEFT].getPosition().first) / 2;
        dynamicwall.SetAttribute("west", west.ToString());

        int south = (basePos.second - mEndCaps[SIDE.BOTTOM].getPosition().second) / 2;
        dynamicwall.SetAttribute("south", south.ToString());

        XmlElement boxCollider = doc.CreateElement("boxcollider2d");
        Vector2 colliderSize = mEM.getTileSize();
        colliderSize.x *= east + west + 1;
        Vector2 colliderCenter = Vector2.zero;
        colliderCenter.y = (north - south) * mEM.getTileSize().y;
        boxCollider.SetAttribute("size", colliderSize.ToString());
        boxCollider.SetAttribute("center", colliderCenter.ToString());
        dynamicwall.AppendChild(boxCollider);

        boxCollider = doc.CreateElement("boxcollider2d");
        colliderSize = mEM.getTileSize();
        colliderSize.y *= north + south + 1;
        colliderCenter = Vector2.zero;
        colliderCenter.x = (east - west) * mEM.getTileSize().x;
        boxCollider.SetAttribute("size", colliderSize.ToString());
        boxCollider.SetAttribute("center", colliderCenter.ToString());
        dynamicwall.AppendChild(boxCollider);

        dynamicwall.AppendChild(ComponentSerializer.toXML(transform, doc));

        callback(dynamicwall);

        return null;
    }

    public virtual void select()
    {
        mIsSelected = true;
        mCurrentDelay = Time.smoothDeltaTime;

        highlight(Color.cyan);
    }
    public virtual void deselect()
    {
        mIsSelected = false;

        unlight();
    }

    public void highlight(Color color)
    {
        mSelection.color = color;
        mSelection.gameObject.SetActive(true);
    }
    public void unlight()
    {
        mSelection.gameObject.SetActive(false);
    }

    public void remove()
    {
        foreach (TileData[] tiles in mTiles.Values)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].detachObject();
                tiles[i] = null;
            }
        }

        mDWM.unregister(getID());
        
        mTiles.Clear();
        GameObject.Destroy(this.gameObject);
    }

    public void attachTile(TileData baseTile)
    {
        TileData[] baseTiles = new TileData[4];
        baseTiles[0] = baseTile;
        baseTiles[1] = baseTile.getTile(SIDE.RIGHT);
        baseTiles[2] = baseTile.getTile(SIDE.TOP);
        baseTiles[3] = baseTiles[2].getTile(SIDE.RIGHT);

        foreach (TileData tile in baseTiles)
            tile.attachObject(this);

        mTiles.Add(baseTile.getPosition(), baseTiles);
    }
    public void detachTile(Utilities.IntPair tilePos)
    {
        TileData[] searcher;
        if (!mTiles.TryGetValue(tilePos, out searcher))
            return;

        foreach (TileData tile in searcher)
            tile.detachObject();

        mTiles.Remove(tilePos);
    }
    public void attachBase(TileData baseTile)
    {
        mBase = baseTile;
        mEndCaps.Add(SIDE.RIGHT, mBase);
        mEndCaps.Add(SIDE.TOP, mBase);
        mEndCaps.Add(SIDE.LEFT, mBase);
        mEndCaps.Add(SIDE.BOTTOM, mBase);

        attachTile(baseTile);
    }

    public void registerBase()
    {
        mBasePos = transform.position;
    }
	#endregion
	
	#region Instance Methods
    protected abstract string getType();
    protected virtual void selectUpdate()
    {
        resize();
        updateTexture();
        updatePosition();

        mResized = false;
    }

    private void resize()
    {
        Vector2 inputDelta = mIM.primaryJoystick.get();

        if (inputDelta.sqrMagnitude < .25f)
        {
            mCurrentDelay = -1;
            return;
        }
        else if (Time.realtimeSinceStartup - mCurrentDelay < SELECTION_DELAY)
            return;

        mCurrentDelay = Time.realtimeSinceStartup;

        bool mIsAuxInput = false;
        if (mIM.auxillaryInput.get())
            mIsAuxInput = true;

        if (inputDelta.x > .5f)
        {
            mResized = true;

            if (mIsAuxInput)
                shrinkSide(SIDE.RIGHT);
            else
                growSide(SIDE.RIGHT);
        }
        else if (inputDelta.x < -.5f)
        {
            mResized = true;

            if (mIsAuxInput)
                shrinkSide(SIDE.LEFT);
            else
                growSide(SIDE.LEFT);
        }

        if (inputDelta.y > .5f)
        {
            mResized = true;

            if (mIsAuxInput)
                shrinkSide(SIDE.TOP);
            else
                growSide(SIDE.TOP);
        }
        else if (inputDelta.y < -.5f)
        {
            mResized = true;

            if (mIsAuxInput)
                shrinkSide(SIDE.BOTTOM);
            else
                growSide(SIDE.BOTTOM);
        }
    }
    private void growSide(SIDE side)
    {
        TileData endCap = mEndCaps[side];

        TileData newEnd = endCap.getTile(side);
        if (newEnd == null || newEnd.isWall())
            return;

        newEnd = newEnd.getTile(side);
        if (newEnd == null || newEnd.isWall())
            return;

        attachTile(newEnd);
        mEndCaps[side] = newEnd;
    }
    private void shrinkSide(SIDE side)
    {
        TileData endCap = mEndCaps[side];
        if (endCap.getPosition().equals(mBase.getPosition()))
            return;

        SIDE shrinkDir = flipSide(side);

        TileData newEnd = endCap.getTile(shrinkDir);
        if (newEnd == null)
            return;

        newEnd = newEnd.getTile(shrinkDir);
        if (newEnd == null)
            return;

        detachTile(endCap.getPosition());
        mEndCaps[side] = newEnd;
    }

    private void updatePosition()
    {
        if (!mResized)
            return;

        Utilities.IntPair basePos = mBase.getPosition();

        Utilities.IntPair netOffset = new Utilities.IntPair();
        netOffset.first = (mEndCaps[SIDE.RIGHT].getPosition().first - basePos.first) - 
            (basePos.first - mEndCaps[SIDE.LEFT].getPosition().first);
        netOffset.first /= 2;
        netOffset.second = (mEndCaps[SIDE.TOP].getPosition().second - basePos.second) - 
            (basePos.second - mEndCaps[SIDE.BOTTOM].getPosition().second);
        netOffset.second /= 2;

        Vector3 posOffset = mEM.getTileSize();
        posOffset.x *= netOffset.first;
        posOffset.y *= netOffset.second;

        transform.position = mBasePos + posOffset;
        mSelection.gameObject.transform.position = mBasePos;
    }

    private void updateTexture()
    {
        if (!mResized)
            return;

        Utilities.IntPair basePos = mBase.getPosition();
        int east = (mEndCaps[SIDE.RIGHT].getPosition().first - basePos.first) / 2;
        int north = (mEndCaps[SIDE.TOP].getPosition().second - basePos.second) / 2;

        int west = (basePos.first - mEndCaps[SIDE.LEFT].getPosition().first) / 2;
        int south = (basePos.second - mEndCaps[SIDE.BOTTOM].getPosition().second) / 2;

        mSpriteRenderer.sprite = createFullTex(east, north, west, south);
    }

    private static SIDE flipSide(SIDE side)
    {
        switch (side)
        {
            case SIDE.RIGHT:
                return SIDE.LEFT;
            case SIDE.TOP:
                return SIDE.BOTTOM;
            case SIDE.LEFT:
                return SIDE.RIGHT;
            case SIDE.BOTTOM:
                return SIDE.TOP;
        }

        return SIDE.RIGHT;
    }

    protected static Texture2D getTexture(bool isEastWall, bool isNorthWall, bool isWestWall, bool isSouthWall)
    {
        loadTextures();

        int index = 0;
        if (isEastWall)
            index |= 0x1;
        if (isNorthWall)
            index |= 0x2;
        if (isWestWall)
            index |= 0x4;
        if (isSouthWall)
            index |= 0x8;

        return mWallTextures[index];
    }
    protected static void loadTextures()
    {
        if (mWallTextures == null)
        {
            mWallTextures = new Texture2D[16];
            mWallTextures[0] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/SingleUnit");
            mWallTextures[1] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/WestEndCap");
            mWallTextures[2] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/SouthEndCap");
            mWallTextures[3] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/BottomLeftCorner");
            mWallTextures[4] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/EastEndCap");
            mWallTextures[5] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/HorizontalStraight");
            mWallTextures[6] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/BottomRightCorner");
            mWallTextures[7] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/EastNorthWestT");
            mWallTextures[8] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/NorthEndCap");
            mWallTextures[9] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/TopLeftCorner");
            mWallTextures[10] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/VerticalStraight");
            mWallTextures[11] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/EastNorthSouthT");
            mWallTextures[12] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/TopRightCorner");
            mWallTextures[13] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/EastWestSouthT");
            mWallTextures[14] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/NorthWestSouthT");
            mWallTextures[15] = Resources.Load<Texture2D>("Textures/Game Environment/Walls/Intersection");

            for (int i = 0; i < mWallTextures.Length; i++)
            {
                if (mWallTextures[i] == null)
                    Debug.Log("mWallTextures[" + i + "] is null");
            }
        }
    }
    protected static Sprite createFullTex(int east, int north, int west, int south)
    {
        int centerSpriteIndex = 0;

        if (east <= 0)
            east = 0;
        else
            centerSpriteIndex = 0x1;
        if (north <= 0)
            north = 0;
        else
            centerSpriteIndex |= 0x2;
        if (west <= 0)
            west = 0;
        else
            centerSpriteIndex |= 0x4;
        if (south <= 0)
            south = 0;
        else
            centerSpriteIndex |= 0x8;

        Utilities.Pair<int, int> centerIndex =
            new Utilities.Pair<int, int>(west, south);

        Utilities.Pair<int, int> wallTexSize =
            new Utilities.Pair<int, int>(mWallTextures[0].width, mWallTextures[0].height);

        Utilities.Pair<int, int> dimensions =
            new Utilities.Pair<int, int>(west + east + 1, north + south + 1);


        Texture2D wallTexture =
            new Texture2D(dimensions.first * mWallTextures[0].width, dimensions.second * mWallTextures[0].height);
        wallTexture.alphaIsTransparency = true;

        // Clear out all of the pixels
        Color32[] wallTexColors = wallTexture.GetPixels32();
        Color32 clearColor32 = new Color32(0, 0, 0, 0);
        for (int i = 0; i < wallTexColors.Length; i++)
            wallTexColors[i] = clearColor32;
        wallTexture.SetPixels32(wallTexColors);

        // Set the center wall texture
        wallTexture.SetPixels(
            centerIndex.first * wallTexSize.first, centerIndex.second * wallTexSize.second,
            wallTexSize.first, wallTexSize.second,
            mWallTextures[centerSpriteIndex].GetPixels());

        if (west > 0)
        {
            wallTexture.SetPixels(
                0, south * wallTexSize.second,
                wallTexSize.first, wallTexSize.second,
                mWallTextures[1].GetPixels()); //West end cap

            for (int i = 1; i < centerIndex.first; i++)
                wallTexture.SetPixels(
                    i * wallTexSize.first, south * wallTexSize.second,
                    wallTexSize.first, wallTexSize.second,
                    mWallTextures[5].GetPixels()); //Horizontal Straight
        }
        if (east > 0)
        {
            wallTexture.SetPixels(
                (west + east) * wallTexSize.first, south * wallTexSize.second,
                wallTexSize.first, wallTexSize.second,
                mWallTextures[4].GetPixels()); //East end cap

            for (int i = west + east - 1; i > centerIndex.first; i--)
                wallTexture.SetPixels(
                    i * wallTexSize.first, south * wallTexSize.second,
                    wallTexSize.first, wallTexSize.second,
                    mWallTextures[5].GetPixels()); //Horizontal Straight
        }

        if (south > 0)
        {
            wallTexture.SetPixels(
                west * wallTexSize.first, 0,
                wallTexSize.first, wallTexSize.second,
                mWallTextures[2].GetPixels()); //South end cap

            for (int i = 1; i < centerIndex.second; i++)
                wallTexture.SetPixels(
                    west * wallTexSize.first, i * wallTexSize.second,
                    wallTexSize.first, wallTexSize.second,
                    mWallTextures[10].GetPixels()); //Horizontal Straight
        }
        if (north > 0)
        {
            wallTexture.SetPixels(
                west * wallTexSize.first, (south + north) * wallTexSize.second,
                wallTexSize.first, wallTexSize.second,
                mWallTextures[8].GetPixels()); //North end cap

            for (int i = south + north - 1; i > centerIndex.second; i--)
                wallTexture.SetPixels(
                    west * wallTexSize.first, i * wallTexSize.second,
                    wallTexSize.first, wallTexSize.second,
                    mWallTextures[10].GetPixels()); //Horizontal Straight
        }

        wallTexture.Apply();

        return Sprite.Create(
            wallTexture,
            new Rect(0, 0, wallTexture.width, wallTexture.height),
            new Vector2(0.5f, 0.5f), 160);
    }
	#endregion
}
