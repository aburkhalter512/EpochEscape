using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Utilities;

namespace MapEditor
{
    public class ChunkManager : Manager<ChunkManager>, ISerializable
    {
        #region Instance Variables
        List<Pair<Utilities.Vec2Int, Texture2D>> mQueuedTiles;
        bool mIsAwake = false;

        SortedList<Vec2Int, Texture2D> mChunks;
        SortedList<Vec2Int, Texture2D> mChunksToUpdate;

        Vec2Int mChunkTexSize;
        Vector2 mChunkRealSize;

        Vec2Int mTilesPerChunk = new Vec2Int(16, 16);

        Vec2Int mTileTexSize = new Vec2Int(32, 32);

        Vector2 mTileSize;

        Color mClearColor = new Color(0, 0, 0, 1);
        Texture2D mClearTex;

        private string _exportDir = "";
        #endregion

        protected override void Awaken()
        {
        }

        protected override void Initialize()
        {
            mTileSize = Map.tileSize;

            mChunkTexSize = new Vec2Int(
                mTileTexSize.x * mTilesPerChunk.x,
                mTileTexSize.y * mTilesPerChunk.y);

            mChunkRealSize = new Vector2(
                mTileSize.x * mTilesPerChunk.x,
                mTileSize.y * mTilesPerChunk.y);

            mChunks = new SortedList<Vec2Int, Texture2D>(Vec2IntComparer.Get());
            mChunksToUpdate = new SortedList<Vec2Int, Texture2D>(Vec2IntComparer.Get());

            getChunk(new Vec2Int(0, 0));
            getChunk(new Vec2Int(0, -1));
            getChunk(new Vec2Int(-1, 0));
            getChunk(new Vec2Int(-1, -1));

            mIsAwake = true;

            if (mQueuedTiles == null)
                mQueuedTiles = new List<Pair<Vec2Int, Texture2D>>();

            foreach (Pair<Vec2Int, Texture2D> tile in mQueuedTiles)
                setTileTexture(tile.first, tile.second);
        }

        protected void Update()
        {
            updateChunks();
        }

        #region Interface Methods
        public void clearTileTexture(Vec2Int logicalPosition)
        {
            if (logicalPosition == null)
                return;

            if (mClearTex == null)
            {
                mClearTex = new Texture2D(mTileTexSize.x, mTileTexSize.y);

                Color[] pixels = mClearTex.GetPixels();
                for (int i = 0; i < pixels.Length; i++)
                    pixels[i] = mClearColor;

                mClearTex.SetPixels(pixels);
                mClearTex.Apply();
            }

            setTileTexture(logicalPosition, mClearTex);
        }

        public void setTileTexture(Vec2Int logicalPosition, Texture2D tileTex)
        {
            if (logicalPosition == null || tileTex == null)
                return;

            if (!mIsAwake)
            {
                if (mQueuedTiles == null)
                    mQueuedTiles = new List<Pair<Vec2Int, Texture2D>>();

                mQueuedTiles.Add(new Pair<Vec2Int, Texture2D>(new Vec2Int(logicalPosition), tileTex));
                return;
            }

            if (tileTex.width != mTileTexSize.x || tileTex.height != mTileTexSize.y)
            {
                Debug.Log("setTileTexture: tileTex size is not correct!");
                return;
            }

            Vec2Int chunkPos = new Vec2Int(0, 0);
            Vec2Int localTilePos = new Vec2Int(0, 0);

            toChunk(logicalPosition, out chunkPos, out localTilePos);
            Texture2D chunk = getChunk(chunkPos);

            chunk.SetPixels(
                localTilePos.x * mTileTexSize.x,
                localTilePos.y * mTileTexSize.y,
                mTileTexSize.x,
                mTileTexSize.y,
                tileTex.GetPixels());

            Texture2D toFind;
            if (!mChunksToUpdate.TryGetValue(chunkPos, out toFind))
                mChunksToUpdate.Add(chunkPos, chunk);
        }

        public Vec2Int getTileTexSize()
        {
            return mTileTexSize;
        }

        // Assumes destDir exists
        public void exportDir(string dir)
        {
            _exportDir = dir;
        }

        public IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
        {
            if (callback == null)
                yield break;

            string chunkDir = _exportDir + "/Chunks";

            if (!Directory.Exists(chunkDir))
                Directory.CreateDirectory(chunkDir);

            XmlElement parent = doc.CreateElement("chunks");
            parent.SetAttribute("chunkSize", mChunkTexSize.ToString());
            parent.SetAttribute("chunkRealSize", mChunkRealSize.ToString());
            BinaryWriter writer;
            FileStream fstream;

            foreach (KeyValuePair<Vec2Int, Texture2D> it in mChunks)
            {
                string filename = "texture_" + it.Key.x + "_" + it.Key.y + ".png";

                XmlElement child = doc.CreateElement("chunk");
                child.SetAttribute("filename", filename);
                child.SetAttribute("logicalPosition", it.Key.ToString());

                parent.AppendChild(child);

                fstream = new FileStream(chunkDir + "/" + filename, FileMode.Create);
                writer = new BinaryWriter(fstream);

                byte[] chunkData = it.Value.EncodeToJPG();
                writer.Write(chunkData);

                writer.Close();

                yield return null;
            }

            callback(parent);
        }
        #endregion

        #region Instance Methods
        protected Texture2D getChunk(Vec2Int logicalChunkPos)
        {
            Texture2D retVal;

            if (!mChunks.TryGetValue(logicalChunkPos, out retVal))
            {
                retVal = _createChunk(logicalChunkPos);
                mChunks.Add(new Vec2Int(logicalChunkPos), retVal);
            }

            return retVal;
        }

        // NOT INTENDED TO BE CALLED DIRECTLY
        protected Texture2D _createChunk(Vec2Int logicalChunkPos)
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(
                logicalChunkPos.x * mChunkRealSize.x + mChunkRealSize.x / 2 - mTileSize.x / 2,
                logicalChunkPos.y * mChunkRealSize.y + mChunkRealSize.y / 2 - mTileSize.y / 2,
                0);

            Mesh chunkMesh = Utilities.Graphics.makeQuadMesh(mChunkRealSize);

            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = chunkMesh;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.material.shader = Shader.Find("Self-Illumin/Diffuse");

            Texture2D retVal = new Texture2D(mChunkTexSize.x, mChunkTexSize.y);
            Color[] pixels = retVal.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = mClearColor;

            retVal.SetPixels(pixels);
            retVal.Apply();
            mr.material.mainTexture = retVal;

            return retVal;
        }

        protected void toChunk(
            Vec2Int tilePos,
            out Vec2Int chunkPos,
            out Vec2Int localTilePos)
        {
            if (tilePos == null)
            {
                chunkPos = null;
                localTilePos = null;

                return;
            }

            chunkPos = new Vec2Int(0, 0);
            localTilePos = new Vec2Int(0, 0);

            if (tilePos.x >= 0)
            {
                chunkPos.x = tilePos.x / mTilesPerChunk.x;
                localTilePos.x = tilePos.x % mTilesPerChunk.x;
            }
            else
            {
                chunkPos.x = (tilePos.x + 1) / mTilesPerChunk.x - 1;
                localTilePos.x = mTilesPerChunk.x - 1 + (tilePos.x + 1) % mTilesPerChunk.x;
            }

            if (tilePos.y >= 0)
            {
                chunkPos.y = tilePos.y / mTilesPerChunk.y;
                localTilePos.y = tilePos.y % mTilesPerChunk.y;
            }
            else
            {
                chunkPos.y = (tilePos.y + 1) / mTilesPerChunk.y - 1;
                localTilePos.y = mTilesPerChunk.y - 1 + (tilePos.y + 1) % mTilesPerChunk.y;
            }
        }

        protected void updateChunks()
        {
            if (mChunksToUpdate.Count == 0)
                return;

            foreach (Texture2D tex in mChunksToUpdate.Values)
                tex.Apply();

            mChunksToUpdate.Clear();
        }
        #endregion
    }
}
