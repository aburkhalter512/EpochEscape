using UnityEngine;
using Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MapEditor
{
    public class Map
    {
        #region Interface Variables
        public static readonly Vector2 tileSize = new Vector2(.2f, .2f);
        #endregion

        #region Instance Variables
        private static Map _instance;

        private SortedList<Vec2Int, Chunk> _chunks;
        private bool _processing;
        #endregion

        #region Class constants
        private class Chunk
        {
            QuadNode<Tile>[,] _data;

            public Chunk()
            {
                _data = new QuadNode<Tile>[size.x, size.y];

                for (int i = 0, j = 0; i < size.x; i++)
                {
                    for (j = 0; j < size.y; j++)
                    {
                        _data[i, j] = new QuadNode<Tile>();

                        if (j > 0)
                            _data[i, j - 1].attach(_data[i, j], SIDE_4.TOP);

                        if (i > 0)
                            _data[i - 1, j].attach(_data[i, j], SIDE_4.RIGHT);
                    }
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

            public void connectTo(Chunk chunk, SIDE_4 side)
            {
                if (chunk == null)
                    return;

                switch (side)
                {
                    case SIDE_4.RIGHT:
                        for (int j = 0; j < size.y; j++)
                            _data[size.x - 1, j].attach(chunk._data[0, j], SIDE_4.RIGHT);
                        break;
                    case SIDE_4.LEFT:
                        for (int j = 0; j < size.y; j++)
                            _data[0, j].attach(chunk._data[size.x - 1, j], SIDE_4.LEFT);
                        break;
                    case SIDE_4.TOP:
                        for (int i = 0; i < size.x; i++)
                            _data[i, size.y - 1].attach(chunk._data[i, 0], SIDE_4.TOP);
                        break;
                    case SIDE_4.BOTTOM:
                        for (int i = 0; i < size.x; i++)
                            _data[i, 0].attach(chunk._data[i, size.y - 1], SIDE_4.BOTTOM);
                        break;
                }
            }

            public static readonly Vec2Int size = new Vec2Int(8, 8);
        }
        #endregion

        private Map()
        {
            _chunks = new SortedList<Vec2Int, Chunk>(Vec2IntComparer.Get());
            getChunk(new Vec2Int(0, 0), true);
            getChunk(new Vec2Int(0, -1), true);
            getChunk(new Vec2Int(-1, 0), true);
            getChunk(new Vec2Int(-1, -1), true);

            _processing = false;
        }

        #region Interface Methods
        public static Map Get()
        {
            if (_instance == null)
                _instance = new Map();

            return _instance;
        }

        public void processAllTiles(Action<Tile, Vec2Int> processor)
        {
            processAllNodes((QuadNode<Tile> node, Vec2Int v) =>
                {
                    if (node.data() != null)
                        processor(node.data(), v);
                });
        }

        public void processAllNodes(Action<QuadNode<Tile>, Vec2Int> processor)
        {
            if (_processing)
                return;

            _processing = true;

            CoroutineManager.Get().StartCoroutine(_processAllNodes(processor));
        }

        public bool doneProcessing()
        {
            return !_processing;
        }

        public Tile[,] getArea(Tile baseTile, Vec2Int size)
        {
            if (size.x < 0 || size.y < 0)
                return null;

            Tile[,] area = new Tile[size.x, size.y];

            Vec2Int basePos = baseTile.position();
            Vec2Int offset = new Vec2Int(0, 0);
            for (int i = 0; i < size.x; i++)
            {
                offset.x = i;
                for (int j = 0; j < size.y; j++)
                {
                    offset.y = j;
                    area[i, j] = getExistingTile(basePos + offset);
                }
            }

            return area;
        }

        public void setTile(Vec2Int globalPosition, Tile tile)
        {
            getQuadNode(globalPosition).data(tile);
        }

        public Tile getTile(Vector3 worldPosition)
        {
            Vector3 snapDistance = Utilities.Math.toVector3(tileSize);

            Vec2Int globalPosition = new Vec2Int(
                Mathf.RoundToInt(worldPosition.x / snapDistance.x),
                Mathf.RoundToInt(worldPosition.y / snapDistance.y));

            return getTile(globalPosition);
        }
        public Tile getTile(Vec2Int globalPosition)
        {
            QuadNode<Tile> node = getQuadNode(globalPosition);
            if (node.data() == null)
                node.data(new Tile(node, null, globalPosition));

            return node.data();
        }

        public Tile getExistingTile(Vector3 worldPosition)
        {
            Vector3 snapDistance = Utilities.Math.toVector3(tileSize);

            Vec2Int globalPosition = new Vec2Int(
                Mathf.RoundToInt(worldPosition.x / snapDistance.x),
                Mathf.RoundToInt(worldPosition.y / snapDistance.y));

            return getExistingTile(globalPosition);
        }
        public Tile getExistingTile(Vec2Int globalPosition)
        {
            QuadNode<Tile> node = getQuadNode(globalPosition, true);

            if (node == null)
                return null;

            return node.data();
        }

        public Vector3 toTilePos(Vector3 vec)
        {
            return toTilePos(toLogicalTilePos(vec));
        }
        public Vector3 toTilePos(Vec2Int pos)
        {
            Vector3 retVal = Utilities.Math.toVector3(pos);
            retVal.x *= tileSize.x;
            retVal.y *= tileSize.y;

            return retVal;
        }

        public Vec2Int toLogicalTilePos(Vector3 vec)
        {
            return new Vec2Int(
                Mathf.RoundToInt(vec.x / tileSize.x),
                Mathf.RoundToInt(vec.y / tileSize.y));
        }

        public void deleteTile(Vector3 worldPosition)
        {
            deleteTile(toLogicalTilePos(worldPosition));
        }
        public void deleteTile(Vec2Int globalPosition)
        {
            QuadNode<Tile> node = getQuadNode(globalPosition, true);
            if (node == null)
                return;

            node.data().clear();
            node.data(null);
        }
        #endregion

        #region Instance Methods
        private IEnumerator _processAllNodes(Action<QuadNode<Tile>, Vec2Int> processor)
        {
            Vec2Int pos = new Vec2Int(0, 0);
            Vec2Int chunkOffset = new Vec2Int(0, 0);

            int processCounter = 0;
            foreach (KeyValuePair<Vec2Int, Chunk> pair in _chunks)
            {
                chunkOffset = new Vec2Int(pair.Key);
                chunkOffset.x *= Chunk.size.x;
                chunkOffset.y *= Chunk.size.y;

                for (int i = 0; i < Chunk.size.x; i++)
                {
                    pos.x = i;
                    for (int j = 0; j < Chunk.size.y; j++)
                    {
                        pos.y = j;

                        processor(pair.Value.get(i, j), pos + chunkOffset);

                        if (processCounter++ >= CoroutineManager.yieldCount)
                        {
                            processCounter = 0;
                            yield return null;
                        }
                    }
                }
            }

            _processing = false;

            yield break;
        }

        private QuadNode<Tile> getQuadNode(Vec2Int globalPosition, bool existingOnly = false)
        {
            Vec2Int chunkPos = new Vec2Int(0, 0);
            Vec2Int localPos = new Vec2Int(0, 0);

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

            Chunk searcher = getChunk(chunkPos, !existingOnly);

            if (searcher == null)
                return null;

            return searcher.get(localPos.x, localPos.y);
        }

        // Can be used for creating chunks
        private Chunk getChunk(Vec2Int pos, bool create = false)
        {
            Chunk searcher = null;
            if (!_chunks.TryGetValue(pos, out searcher) && create)
            {
                searcher = new Chunk();
                _chunks.Add(pos, searcher);

                pos = new Vec2Int(pos);

                Chunk connector;
                if (_chunks.TryGetValue(pos + new Vec2Int(-1, 0), out connector))
                    connector.connectTo(searcher, SIDE_4.RIGHT);
                if (_chunks.TryGetValue(pos + new Vec2Int(1, 0), out connector))
                    connector.connectTo(searcher, SIDE_4.BOTTOM);
                if (_chunks.TryGetValue(pos + new Vec2Int(0, -1), out connector))
                    connector.connectTo(searcher, SIDE_4.LEFT);
                if (_chunks.TryGetValue(pos + new Vec2Int(0, 1), out connector))
                    connector.connectTo(searcher, SIDE_4.TOP);
            }

            return searcher;
        }
        #endregion
    }
}
