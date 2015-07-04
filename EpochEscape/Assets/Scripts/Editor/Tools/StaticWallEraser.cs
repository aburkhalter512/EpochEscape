using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticWallEraser// : SquarePlacer<StaticWallEraser>
{
	/*#region Instance Variables
    SortedList<Utilities.IntPair, PlaceableStaticWall> mExisted;

    SortedDictionary<Utilities.IntPair, Utilities.IntPair> mProcessed;

    protected StaticWallManager mSM;
	#endregion

    protected override void Awaken()
    {
        base.Awaken();

        registerCoroutine(resetProcessed);

        Utilities.IntPairComparer comparer = Utilities.IntPairComparer.Get();
        mExisted = new SortedList<Utilities.IntPair, PlaceableStaticWall>(comparer);
        mProcessed = new SortedDictionary<Utilities.IntPair, Utilities.IntPair>(comparer);
    }

    #region Instance Methods
    protected override void initSelection()
    {
        mSM = StaticWallManager.Get();

        mExisted.Clear();

        TileData tile = mTileCursor.getDetectedTile();
        Utilities.IntPair pos = tile.getPosition();

        if (pos.first % 2 == 1)
            tile = tile.getTile(QuadMatrix<TileData>.Node.SIDE.LEFT);
        if (pos.second % 2 == 1)
            tile = tile.getTile(QuadMatrix<TileData>.Node.SIDE.BOTTOM);

        mBasePos = new Utilities.IntPair(tile.getPosition());
        mOldCursor = new Utilities.IntPair(tile.getPosition());

        if (tile == null)
        {
            finalizeSelection();
            return;
        }

        if (tile.isWall())
        {
            mExisted.Add(tile.getPosition(), tile.getWall());
            StaticWallPlacer.removeStaticWallFromNode(tile.quadNode);
        }
    }

    protected override void finalizeSelection()
    {
        if (mIM.cancelInput.get())
        {
            foreach (PlaceableStaticWall node in mExisted.Values)
                node.detach();
        }

        mExisted.Clear();
    }

    protected override void selectionShrink(Utilities.IntPair tilePos)
    {
        if (tilePos.first % 2 != 0)
            tilePos.first--;
        if (tilePos.second % 2 != 0)
            tilePos.second--;

        Utilities.IntPair searcher;
        if (mProcessed.TryGetValue(tilePos, out searcher))
            return;

        mProcessed.Add(new Utilities.IntPair(tilePos), new Utilities.IntPair(tilePos));

        TileData tile = mTF.findTile(tilePos);

        // Does the tile already exist?
        if (tile == null)
            return;

        if (mExisted.Remove(tilePos))
        {
            StaticWallPlacer.addStaticWallToNode(tile.quadNode);
            return;
        }
    }

    protected override void selectionGrow(Utilities.IntPair tilePos)
    {
        if (tilePos.first % 2 != 0)
            tilePos.first--;
        if (tilePos.second % 2 != 0)
            tilePos.second--;

        Utilities.IntPair searcher;
        if (mProcessed.TryGetValue(tilePos, out searcher))
            return;

        mProcessed.Add(new Utilities.IntPair(tilePos), new Utilities.IntPair(tilePos));

        TileData tile = mTF.findTile(tilePos);

        // Does the tile already exist?
        if (tile == null || !tile.isWall())
            return;

        mExisted.Add(tile.getPosition(), tile.getWall());
        mSM.remove(tile.getWall().bottomLeft.getPosition());
        StaticWallPlacer.removeStaticWallFromNode(tile.quadNode);
    }

    private IEnumerator resetProcessed()
    {
        mProcessed.Clear();

        yield break;
    }
	#endregion*/
}
