using UnityEngine;
using System.Collections.Generic;

public class Map
{
	#region Interface Variables
    public static readonly Vector2 tileSize = new Vector2(.2f, .2f);
	#endregion
	
	#region Instance Variables
    private static Map _instance;

    private SortedList<Utilities.Vec2Int, Chunk> _chunks;
	#endregion

    #region Class constants
    private class Chunk
    {
        QuadNode<Tile>[,] _data;

        public Chunk()
        {
            _data = new QuadNode<Tile>[size.x, size.y];

            QuadNode<Tile> bottom = null;
            QuadNode<Tile> left = null;
            for (int i = 0, j = 0; i < size.x; i++)
            {
                for (j = 0; j < size.y; j++)
                {
                    _data[i, j] = new QuadNode<Tile>();

                    if (bottom != null)
                        bottom.attach(_data[i, j], Utilities.SIDE_4.TOP);

                    if (left != null)
                    {
                        left.attach(_data[i, j], Utilities.SIDE_4.RIGHT);
                        left = left.getSide(Utilities.SIDE_4.TOP);
                    }

                    bottom = _data[i, j];
                }

                left = _data[i, 0];
                bottom = null;
            }
        }

        public QuadNode<Tile> get(int x, int y)
        {
            if (x < 0 || x >= size.x)
                return null;
            else if (y < 0 || y >= size.y)
                return null;

            return _data[x, y];
        }
        public void set(QuadNode<Tile> tile, int x, int y)
        {
            if (tile == null || 
                x < 0 || x >= size.x || 
                y < 0 || y >= size.y)
                return;

            _data[x, y] = tile;
        }

        public void connectTo(Chunk chunk, Utilities.SIDE_4 side)
        {
            if (chunk == null)
                return;

            switch (side)
            {
                case Utilities.SIDE_4.RIGHT:
                    for (int j = 0; j < size.y; j++)
                        _data[size.x - 1, j].attach(chunk._data[0, j], Utilities.SIDE_4.RIGHT);
                    break;
                case Utilities.SIDE_4.LEFT:
                    for (int j = 0; j < size.y; j++)
                        _data[0, j].attach(chunk._data[size.x - 1, j], Utilities.SIDE_4.LEFT);
                    break;
                case Utilities.SIDE_4.TOP:
                    for (int i = 0; i < size.x; i++)
                        _data[i, size.y - 1].attach(chunk._data[i, 0], Utilities.SIDE_4.TOP);
                    break;
                case Utilities.SIDE_4.BOTTOM:
                    for (int i = 0; i < size.x; i++)
                        _data[i, 0].attach(chunk._data[i, size.y - 1], Utilities.SIDE_4.BOTTOM);
                    break;
            }
        }

        public static readonly Utilities.Vec2Int size = new Utilities.Vec2Int(64, 64);
    }
    #endregion

    private Map()
    {
        _chunks = new SortedList<Utilities.Vec2Int, Chunk>(Utilities.Vec2IntComparer.Get());
    }
	
	#region Interface Methods
    public static Map Get()
    {
        if (_instance == null)
            _instance = new Map();

        return _instance;
    }

    public Tile getTile(Vector3 worldPosition)
    {
        Vector3 snapDistance = Utilities.toVector3(tileSize);

        Utilities.Vec2Int globalPosition = new Utilities.Vec2Int(
            Mathf.RoundToInt(worldPosition.x / snapDistance.x),
            Mathf.RoundToInt(worldPosition.y / snapDistance.y));

        return getTile(globalPosition);
    }
    public Tile getTile(Utilities.Vec2Int globalPosition)
    {
        Utilities.Vec2Int chunkPos = new Utilities.Vec2Int(0, 0);
        Utilities.Vec2Int localPos = new Utilities.Vec2Int(0, 0);

        if (globalPosition.x >= 0)
        {
            chunkPos.x = globalPosition.x / Chunk.size.x;
            localPos.x = globalPosition.x % Chunk.size.x;
        }
        else
        {
            chunkPos.x = (globalPosition.x + 1) / Chunk.size.x - 1;
            localPos.x = Chunk.size.x - 1 + (globalPosition.x + 1) % Chunk.size.x;
        }

        if (globalPosition.y >= 0)
        {
            chunkPos.y = globalPosition.y / Chunk.size.y;
            localPos.y = globalPosition.y % Chunk.size.y;
        }
        else
        {
            chunkPos.y = (globalPosition.y + 1) / Chunk.size.y - 1;
            localPos.y = Chunk.size.y - 1 + (globalPosition.y + 1) % Chunk.size.y;
        }

        Chunk searcher;
        if (!_chunks.TryGetValue(chunkPos, out searcher))
        {
            searcher = new Chunk();
            _chunks.Add(chunkPos, searcher);

            Chunk connector;
            if (_chunks.TryGetValue(chunkPos.translate(-1, 0), out connector))
                connector.connectTo(searcher, Utilities.SIDE_4.RIGHT);
            if (_chunks.TryGetValue(chunkPos.translate(1, 1), out connector))
                connector.connectTo(searcher, Utilities.SIDE_4.BOTTOM);
            if (_chunks.TryGetValue(chunkPos.translate(1, -1), out connector))
                connector.connectTo(searcher, Utilities.SIDE_4.LEFT);
            if (_chunks.TryGetValue(chunkPos.translate(-1, -1), out connector))
                connector.connectTo(searcher, Utilities.SIDE_4.TOP);
        }

        return searcher.get(localPos.x, localPos.y).data();
    }

    public Tile getExistingTile(Vector3 worldPosition)
    {
        Vector3 snapDistance = Utilities.toVector3(tileSize);

        Utilities.Vec2Int globalPosition = new Utilities.Vec2Int(
            Mathf.RoundToInt(worldPosition.x / snapDistance.x),
            Mathf.RoundToInt(worldPosition.y / snapDistance.y));

        return getExistingTile(globalPosition);
    }
    public Tile getExistingTile(Utilities.Vec2Int globalPosition)
    {
        Utilities.Vec2Int chunkPos = new Utilities.Vec2Int(0, 0);
        Utilities.Vec2Int localPos = new Utilities.Vec2Int(0, 0);

        if (globalPosition.x >= 0)
        {
            chunkPos.x = globalPosition.x / Chunk.size.x;
            localPos.x = globalPosition.x % Chunk.size.x;
        }
        else
        {
            chunkPos.x = (globalPosition.x + 1) / Chunk.size.x - 1;
            localPos.x = Chunk.size.x - 1 + (globalPosition.x + 1) % Chunk.size.x;
        }

        if (globalPosition.y >= 0)
        {
            chunkPos.y = globalPosition.y / Chunk.size.y;
            localPos.y = globalPosition.y % Chunk.size.y;
        }
        else
        {
            chunkPos.y = (globalPosition.y + 1) / Chunk.size.y - 1;
            localPos.y = Chunk.size.y - 1 + (globalPosition.y + 1) % Chunk.size.y;
        }

        Chunk searcher;
        if (!_chunks.TryGetValue(chunkPos, out searcher))
            return null;

        return searcher.get(localPos.x, localPos.y).data();
    }

    public Vector3 toTilePos(Vector3 vec)
    {
        return toTilePos(toLogicalTilePos(vec));
    }
    public Vector3 toTilePos(Utilities.Vec2Int pos)
    {
        Vector3 retVal = Utilities.toVector3(pos);
        retVal.x *= tileSize.x;
        retVal.y *= tileSize.y;

        return retVal;
    }

    public Utilities.Vec2Int toLogicalTilePos(Vector3 vec)
    {
        return new Utilities.Vec2Int(
            Mathf.RoundToInt(vec.x / tileSize.x),
            Mathf.RoundToInt(vec.y / tileSize.y));
    }
	#endregion
	
	#region Instance Methods
	#endregion
}
