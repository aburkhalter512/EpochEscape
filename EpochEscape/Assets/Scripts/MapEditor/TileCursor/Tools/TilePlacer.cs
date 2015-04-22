using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TilePlacer : SquarePlacer<TilePlacer>, IActivatable
{
    #region Instance Variables
    List<QuadMatrix<TileData>.Node> mSelection;
    private List<QuadMatrix<TileData>.Node> mTilesToConnect;
    private List<QuadMatrix<TileData>.Node> mTilesToDelete;
    #endregion

    protected override void Awaken()
    {
        base.Awaken();

        registerCoroutine(attachTiles);
        registerCoroutine(deleteTiles);

        mSelection = new List<QuadMatrix<TileData>.Node>();
        mTilesToConnect = new List<QuadMatrix<TileData>.Node>();
        mTilesToDelete = new List<QuadMatrix<TileData>.Node>();
    }

    #region Instance Methods
    protected override void initSelection()
    {
        mSelection.Clear();

        Utilities.IntPair pos = mTileCursor.getLogicalCursor();

        mBasePos = new Utilities.IntPair(pos);
        mOldCursor = new Utilities.IntPair(pos);

        TileData tile = mTF.findTile(pos);

        // Does the tile already exist?
        if (tile != null)
            return;

        QuadMatrix<TileData>.Node node = new QuadMatrix<TileData>.Node(pos.first, pos.second);
        mSelection.Add(node);

        TileCreator.createTile(node);

        mTF.attachSurroundingTiles(node.getData(), false);
    }

    protected override void finalizeSelection()
    {
        if (mIM.cancelInput.get())
        {
            foreach (QuadMatrix<TileData>.Node node in mSelection)
            {
                node.getData().clear();
                node.detachAll();
            }
        }
        mSelection.Clear();
    }

    protected override void selectionGrow(Utilities.IntPair tilePos)
    {
        TileData tile = mTF.findTile(tilePos);

        // Does the tile already exist?
        if (tile != null)
            return;

        QuadMatrix<TileData>.Node node = new QuadMatrix<TileData>.Node(tilePos.first, tilePos.second);
        mSelection.Add(node);

        TileCreator.createTile(node);

        mTilesToConnect.Add(node);
    }

    protected override void selectionShrink(Utilities.IntPair tilePos)
    {
        TileData tile = mTF.findTile(tilePos);

        // Does the tile already exist?
        if (tile == null)
            return;

        if (mSelection.Remove(tile.quadNode))
            mTilesToDelete.Add(tile.quadNode);
    }

    protected IEnumerator attachTiles()
    {
        int counter = 0;

        foreach (QuadMatrix<TileData>.Node node in mTilesToConnect)
        {
            mTF.attachSurroundingTiles(node.getData(), false);

            if (counter++ >= processCountYield)
            {
                counter = 0;
                yield return null;
            }
        }

        mTilesToConnect.Clear();
    }

    protected IEnumerator deleteTiles()
    {
        int counter = 0;

        foreach (QuadMatrix<TileData>.Node node in mTilesToDelete)
        {
            mTF.removeTile(node.getPosition());
            node.getData().clear();
            node.detachAll();

            if (counter++ >= processCountYield)
            {
                counter = 0;
                yield return null;
            }
        }

        mTilesToDelete.Clear();
    }
    #endregion
}
