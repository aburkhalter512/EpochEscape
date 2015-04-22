using UnityEngine;
using System.Collections;

public class TileData : QuadMatrix<TileData>.Node.NodeData
{
	#region Interface Variables
    #endregion

    #region Instance Variables
    PlaceableStaticWall mWall;

    ChunkManager mCM;

    Texture2D mDefaultFloorTex;

    IPlaceable mObject;
    #endregion

    #region Class Variables
    #endregion

    #region Interface Methods
    public void setFloor()
    {
        if (mDefaultFloorTex == null)
            mDefaultFloorTex = Resources.Load<Texture2D>("Textures/Floor Tiles/testTile");

        setFloor(mDefaultFloorTex);
    }
    public void setFloor(Texture2D floorTex)
    {
        mWall = null;

        setTexture(floorTex);
    }
    public bool isFloor()
    {
        return !isWall();
    }

    public void attachObject(IPlaceable obj)
    {
        if (obj == null || isWall())
            return;

        mObject = obj;
    }
    public void detachObject()
    {
        mObject = null;
    }
    public bool hasAttached()
    {
        return mObject != null;
    }
    public IPlaceable getAttached()
    {
        return mObject;
    }

    public void setWall(PlaceableStaticWall wall)
    {
        mWall = wall;

        mObject = null;
    }
    public PlaceableStaticWall getWall()
    {
        return mWall;
    }
    public bool isWall()
    {
        return mWall != null;
    }

    public void manualSetTexture(Texture2D tex)
    {
        setTexture(tex);
    }
    public void clear()
    {
        if (mCM == null)
            mCM = ChunkManager.Get();

        //mCM.clearTileTexture(quadNode.getPosition());
    }

    public Utilities.IntPair getPosition()
    {
        return quadNode.getPosition();
    }

    public TileData getTile(QuadMatrix<TileData>.Node.SIDE side)
    {
        if (quadNode.getNode(side) == null)
            return null;

        return quadNode.getNode(side).getData();
    }
    #endregion

    #region Instance Methods
    protected void setTexture(Texture2D tex)
    {
        if (tex == null)
            return;

        if (mCM == null)
            mCM = ChunkManager.Get();

        //mCM.setTileTexture(quadNode.getPosition(), tex);
    }
    #endregion
}
