using UnityEngine;
using System.Collections.Generic;

public class StaticWallPlacer// : Manager<StaticWallPlacer>, IActivatable
{
    /*#region Interface Variables
    #endregion

    #region Instance Variables
    TileData mBaseTile = null;
    TileData mCurTile = null;

    SortedList<Utilities.IntPair, TileData> mOldPath;
    SortedList<Utilities.IntPair, TileData> mSelectionPath;

    Utilities.IntPair mPath;
    bool mIsAltPath;

    InputManager mIM;
    TileCursor mTC;

    bool mIsActive = false;
    bool mIsActivated = false;
    #endregion

    protected override void Awaken()
    {
        Utilities.IntPairComparer comparer = Utilities.IntPairComparer.Get();
        mOldPath = new SortedList<Utilities.IntPair, TileData>(comparer);
        mSelectionPath = new SortedList<Utilities.IntPair, TileData>(comparer);

        mPath = new Utilities.IntPair(0, 0);
    }

    protected override void Initialize()
    {
        mIM = InputManager.Get();
        mTC = TileCursor.Get();
    }

    protected void Update()
    {
        if (mIsActivated)
            updateStaticWallPlacement();
    }

    #region Interface Methods
    public void activate()
    {
        mIsActivated = true;
    }

    public void deactivate()
    {
        mIsActivated = false;

        if (mIsActive)
            placeSelection();
    }

    public static void addStaticWallToNode(QuadMatrix<TileData>.Node node)
    {
        if (node == null)
            return;

        Utilities.IntPair position = node.getPosition();
        if (position.first % 2 != 0)
            node = node.getNode(QuadMatrix<TileData>.Node.SIDE.LEFT);

        if (node == null)
            return;

        if (position.second % 2 != 0)
            node = node.getNode(QuadMatrix<TileData>.Node.SIDE.BOTTOM);

        if (node == null)
            return;

        position = node.getPosition();
        if (position.first % 2 != 0 || position.second % 2 != 0)
            return;

        TileData bottomLeft = node.getData();

        //Debug.Log("Bottom Left: " + bottomLeft.quadNode.getPosition());

        if (bottomLeft == null || bottomLeft.isWall())
            return;

        TileData bottomRight = bottomLeft.getTile(QuadMatrix<TileData>.Node.SIDE.RIGHT);
        if (bottomRight == null)
            return;

        TileData topLeft = bottomLeft.getTile(QuadMatrix<TileData>.Node.SIDE.TOP);
        TileData topRight = bottomRight.getTile(QuadMatrix<TileData>.Node.SIDE.TOP);

        if (topLeft == null || topRight == null)
            return;

        PlaceableStaticWall wall = new PlaceableStaticWall(bottomLeft, bottomRight, topLeft, topRight);
        wall.updateTexture();

        StaticWallManager.Get().add(wall);

        //Debug.Log("Bottom Left: " + bottomLeft.quadNode.getPosition());
    }

    public static void removeStaticWallFromNode(QuadMatrix<TileData>.Node node)
    {
        if (node == null || !node.getData().isWall())
            return;

        node.getData().getWall().detach();

        StaticWallManager.Get().remove(node.getPosition());
    }
    #endregion

    #region Instance Methods
    protected void updateStaticWallPlacement()
    {
        mCurTile = mTC.getDetectedTile();

        if (mCurTile == null)
            return;

        bool forceUpdate = false;

        if (mIM.alternateInput.getDown())
        {
            mIsAltPath = true;
            forceUpdate = true;
        }
        else if (mIM.alternateInput.getUp())
        {
            mIsAltPath = false;
            forceUpdate = true;
        }

        if (mIM.primaryPlace.getDown() && !mIsActive)
            initSelection(mCurTile);
        else if (mIM.primaryPlace.getUp() && mIsActive)
            placeSelection();
        else if (mIM.primaryPlace.get() && mIsActive)
            updateSelection(forceUpdate);
    }

    protected void placeSelection()
    {
        mIsActive = false;
        mBaseTile = null;
        mPath.assign(0, 0);

        mSelectionPath.Clear();
    }

    protected void initSelection(TileData baseTile)
    {
        mIsActive = true;
        mBaseTile = baseTile;
        addStaticWallToNode(mBaseTile.quadNode);
        mSelectionPath.Add(mBaseTile.getPosition(), mBaseTile);
        mOldPath.Add(mBaseTile.getPosition(), mBaseTile);
        mPath.assign(0, 0);

        mSelectionPath.Clear();
    }

    protected void updateSelection(bool forceUpdate)
    {
        Utilities.IntPair curPath = mCurTile.quadNode.getPosition() - mBaseTile.quadNode.getPosition();

        if (!forceUpdate)
        {
            if (curPath.equals(mPath))
                return;
        }

        mPath = curPath;

        //Debug.Log("Updating the selection");

        foreach (TileData tile in mSelectionPath.Values)
            if (tile.getWall() != null)
                tile.getWall().detach();

        mSelectionPath.Clear();

        int iMag = Mathf.Abs(curPath.first);
        int jMag = Mathf.Abs(curPath.second);

        QuadMatrix<TileData>.Node.SIDE iDir = (curPath.first > 0)
            ? QuadMatrix<TileData>.Node.SIDE.RIGHT : QuadMatrix<TileData>.Node.SIDE.LEFT;

        QuadMatrix<TileData>.Node.SIDE jDir = (curPath.second > 0)
            ? QuadMatrix<TileData>.Node.SIDE.TOP : QuadMatrix<TileData>.Node.SIDE.BOTTOM;

        //Debug.Log("iMag: " + iMag + ", jMag: " + jMag);

        TileData curTile = mBaseTile;

        if (mIsAltPath)
        {
            curTile = processVertical(curTile, jMag, jDir);
            curTile = processHorizontal(curTile, iMag, iDir);
        }
        else
        {
            curTile = processHorizontal(curTile, iMag, iDir);
            curTile = processVertical(curTile, jMag, jDir);
        }

        //processDifferences();

        mOldPath.Clear();
        foreach (KeyValuePair<Utilities.IntPair, TileData> pair in mSelectionPath)
            mOldPath.Add(new Utilities.IntPair(pair.Key), pair.Value);
    }

    private TileData processHorizontal(TileData baseTile, int iMag, QuadMatrix<TileData>.Node.SIDE iDir)
    {
        TileData curTile = baseTile;
        TileData it;
        for (int i = 1; i <= iMag; i++)
        {
            it = curTile.getTile(iDir);
            if (it == null)
                break;

            curTile = it;

            if (it.isWall())
                continue;

            mSelectionPath.Add(curTile.getPosition(), curTile);
            addStaticWallToNode(curTile.quadNode);
        }

        return curTile;
    }

    private TileData processVertical(TileData baseTile, int jMag, QuadMatrix<TileData>.Node.SIDE jDir)
    {
        TileData curTile = baseTile;
        TileData it;
        for (int j = 1; j <= jMag; j++)
        {
            it = curTile.getTile(jDir);
            if (it == null)
                break;

            curTile = it;

            if (it.isWall())
                continue;

            mSelectionPath.Add(curTile.getPosition(), curTile);
            addStaticWallToNode(curTile.quadNode);
        }

        return curTile;
    }

    private Utilities.IntPair toBottomLeft(Utilities.IntPair pos)
    {
        Utilities.IntPair retVal = new Utilities.IntPair(pos);

        if (Mathf.Abs(pos.first) % 2 == 1)
            retVal.first--;
        if (Mathf.Abs(pos.second) % 2 == 1)
            retVal.second--;
       
        return retVal;
    }
    #endregion*/
}
