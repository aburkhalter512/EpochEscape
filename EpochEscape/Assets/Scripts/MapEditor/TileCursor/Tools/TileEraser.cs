using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileEraser : SquarePlacer<TileEraser>
{
    #region Instance Variables
    SortedList<Utilities.IntPair, Utilities.IntPair> mExisted;
    List<TileData> mToConnect;
    List<TileData> mToDelete;
    #endregion

    protected override void Awaken()
    {
 	    base.Awaken();

        registerCoroutine(attachTiles);
        registerCoroutine(deleteTiles);

        Utilities.IntPairComparer comparer = Utilities.IntPairComparer.Get();
        mExisted = new SortedList<Utilities.IntPair, Utilities.IntPair>(comparer);
        mToConnect = new List<TileData>();
        mToDelete = new List<TileData>();
    }

    #region Instance Methods
    protected override void initSelection()
    {
        mExisted.Clear();

        Utilities.IntPair pos = new Utilities.IntPair(mTileCursor.getLogicalCursor());
        TileData tile = mTF.findTile(pos);

        if (tile != null && !tile.isWall())
        {
            mTF.removeTile(pos);
            tile.quadNode.getData().clear();
            tile.quadNode.detachAll();

            mExisted.Add(pos, pos);
        }

        mBasePos = new Utilities.IntPair(pos);
        mOldCursor = new Utilities.IntPair(pos);
    }

    protected override void finalizeSelection()
    {
        if (mIM.cancelInput.get())
        {
            foreach (Utilities.IntPair nodePos in mExisted.Values)
            {
                QuadMatrix<TileData>.Node node = new QuadMatrix<TileData>.Node(nodePos.first, nodePos.second);

                TileCreator.createTile(node);
                mTF.attachSurroundingTiles(node.getData());
            }
        }

        mExisted.Clear();
    }

    protected override void selectionShrink(Utilities.IntPair tilePos)
    {
        TileData tile = mTF.findTile(tilePos);

        // Does the tile already exist?
        if (tile != null)
            return;

        if (!mExisted.Remove(tilePos))
            return;

        QuadMatrix<TileData>.Node node = new QuadMatrix<TileData>.Node(tilePos.first, tilePos.second);
        TileCreator.createTile(node);

        mToConnect.Add(node.getData());
    }

    protected override void selectionGrow(Utilities.IntPair tilePos)
    {
        TileData tile = mTF.findTile(tilePos);

        // Does the tile already exist?
        if (tile == null || tile.isWall())
            return;

        Utilities.IntPair searcher;

        if (mExisted.TryGetValue(tilePos, out searcher))
            return;

        mExisted.Add(tile.getPosition(), tile.getPosition());
        mToDelete.Add(tile);
    }

    protected IEnumerator attachTiles()
    {
        int counter = 0;

        foreach (TileData tile in mToConnect)
        {
            mTF.attachSurroundingTiles(tile, false);

            if (counter++ >= processCountYield)
            {
                counter = 0;
                yield return null;
            }
        }

        mToConnect.Clear();
    }

    protected IEnumerator deleteTiles()
    {
        int counter = 0;

        foreach (TileData tile in mToDelete)
        {
            mTF.removeTile(tile.getPosition());
            tile.clear();
            tile.quadNode.detachAll();

            if (counter++ >= processCountYield)
            {
                counter = 0;
                yield return null;
            }
        }

        mToDelete.Clear();
    }
	#endregion
}
