using UnityEngine;
using System;
using System.Collections.Generic;

public class TileFinder : Manager<TileFinder>
{
    #region Interface Variables
    #endregion

    #region Instance Variables
    EpochMap mEM;

    TileData mDetectedTile;

    SortedList<Utilities.IntPair, TileData> mTileSearcher;
    #endregion

    protected override void Awaken()
    {
        mTileSearcher = new SortedList<Utilities.IntPair, TileData>(new Utilities.IntPairComparer());

        return;
    }

    protected override void Initialize()
    {
        mEM = EpochMap.Get();

        BoxCollider2D collider = this.gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = mEM.getTileSize() / 2;

        Rigidbody2D rigidbody = this.gameObject.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0;
    }

    #region Interface Methods
    public void registerTile(TileData tile)
    {
        if (tile == null)
            return;

        mTileSearcher.Add(tile.quadNode.getPosition(), tile);
    }

    public TileData removeTile(Utilities.IntPair logicalPosition)
    {
        TileData retVal;

        if (!mTileSearcher.TryGetValue(logicalPosition, out retVal))
            return null;

        mTileSearcher.Remove(logicalPosition);

        return retVal;
    }

    public TileData findTile(Utilities.IntPair logicalPosition)
    {
        TileData retVal = null;

        if (!mTileSearcher.TryGetValue(logicalPosition, out retVal))
            retVal = null;

        return retVal;
    }

    public void attachSurroundingTiles(TileData tile, bool debug = false)
    {
        if (tile == null)
            return;

        if (debug)
        {
            Debug.Log("attaching for tile: " + tile.quadNode.getPosition());
            /*Debug.Log("right tile: " + Utilities.IntPair.translate(tile.quadNode.getPosition(), 1, 0));
            Debug.Log("up tile: " + Utilities.IntPair.translate(tile.quadNode.getPosition(), 0, 1));
            Debug.Log("left tile: " + Utilities.IntPair.translate(tile.quadNode.getPosition(), -1, 0));
            Debug.Log("down tile: " + Utilities.IntPair.translate(tile.quadNode.getPosition(), 0, -1));*/
        }

        Utilities.IntPair tilePos = tile.quadNode.getPosition();
        TileData searchTile;
        mTileSearcher.TryGetValue(Utilities.IntPair.translate(tilePos, 1, 0), out searchTile);
        if (searchTile != null)
        {
            if (debug)
                Debug.Log("found right tile");

            tile.quadNode.attachNode(searchTile.quadNode, QuadMatrix<TileData>.Node.SIDE.RIGHT);
        }
        else if (debug)
            Debug.Log("did not find right tile");

        mTileSearcher.TryGetValue(Utilities.IntPair.translate(tilePos, 0, 1), out searchTile);
        if (searchTile != null)
        {
            if (debug)
                Debug.Log("found top tile: " + searchTile.quadNode.getPosition());

            tile.quadNode.attachNode(searchTile.quadNode, QuadMatrix<TileData>.Node.SIDE.TOP);
        }
        else if (debug)
            Debug.Log("did not find top tile");

        mTileSearcher.TryGetValue(Utilities.IntPair.translate(tilePos, -1, 0), out searchTile);
        if (searchTile != null)
        {
            if (debug)
                Debug.Log("found left tile");

            tile.quadNode.attachNode(searchTile.quadNode, QuadMatrix<TileData>.Node.SIDE.LEFT);
        }
        else if (debug)
            Debug.Log("did not find left tile");

        mTileSearcher.TryGetValue(Utilities.IntPair.translate(tilePos, 0, -1), out searchTile);
        if (searchTile != null)
        {
            if (debug)
                Debug.Log("found bottom tile");

            tile.quadNode.attachNode(searchTile.quadNode, QuadMatrix<TileData>.Node.SIDE.BOTTOM);
        }
        else if (debug)
            Debug.Log("did not find bottom tile");
    }
    #endregion
}
