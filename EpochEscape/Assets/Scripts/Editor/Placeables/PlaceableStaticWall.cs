using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Editor
{
    public class PlaceableStaticWall : PlaceableObject
    {
        #region Interface Variables
        public Tile bottomLeft;
        public Tile bottomRight;
        public Tile topLeft;
        public Tile topRight;
        #endregion

        #region Instance Variables
        PlaceableStaticWall[] mWallConnections = new PlaceableStaticWall[4];
        bool mIsUpdatingWall = false;

        bool mIsFirstUpdate = true;
        #endregion

        #region Class Variables
        protected static Texture2D[] mWallTextures;
        protected static ChunkManager mCM;
        protected static StaticWallManager mSWM;
        #endregion

        protected override void Awake()
        {
            base.Awake();

            _area = new bool[2, 2];
            _area[0, 0] = true;
            _area[0, 1] = true;
            _area[1, 0] = true;
            _area[1, 1] = true;
        }

        protected override void Start()
        {
            base.Start();

            if (mCM == null)
                mCM = ChunkManager.Get();
            if (mSWM == null)
                mSWM = StaticWallManager.Get();
        }

        #region Interface Methods
        public static void createStaticWall(Vector3 v)
        {
            GameObject _prefab = getPrefab();

            if (_prefab == null)
                return;

            PlaceableStaticWall staticWall =
                (GameObject.Instantiate(_prefab) as GameObject).GetComponent<PlaceableStaticWall>();

            staticWall.StartCoroutine(placeAtStart(staticWall, v));
        }
        public override void rotate()
        {
            // Static walls don't need to rotate...
        }
        public override void moveTo(Vector3 v)
        {
            if (!started())
                return;

            if (placed())
                remove();

            _sr.enabled = true;

            Vector3 offset = Utilities.Math.toVector3(Map.tileSize / 2);
            if (v.x > 0)
                v.x += offset.x;
            else
                v.x -= offset.x;

            if (v.y > 0)
                v.y += offset.y;
            else
                v.y -= offset.y;

            Utilities.Vec2Int tileCounter = new Utilities.Vec2Int(
                (int)(v.x / Map.tileSize.x),
                (int)(v.y / Map.tileSize.y));
            if (tileCounter.x % 2 == 0)
                tileCounter.x++;
            if (tileCounter.y % 2 == 0)
                tileCounter.y++;

            Vector3 snappedPosition = new Vector3(
                tileCounter.x * Map.tileSize.x,
                tileCounter.y * Map.tileSize.y);

            base.moveTo(snappedPosition);
        }
        public override bool place()
        {
            if (!started() || !base.place())
                return false;

            foreach (Tile tile in mAttached)
            {
                if (tile.position().x % 2 == 0)
                {
                    if (tile.position().y % 2 == 0)
                        bottomLeft = tile;
                    else
                        topLeft = tile;
                }
                else
                {
                    if (tile.position().y % 2 == 0)
                        bottomRight = tile;
                    else
                        topRight = tile;
                }
            }

            updateTexture();

            _sr.enabled = false;

            if (!_registered)
            {
                mSWM.add(this);
                _registered = true;
            }

            return true;
        }
        public override void remove()
        {
            if (placed())
            {
                if (bottomLeft != null)
                    bottomLeft.resetTexture();
                if (bottomRight != null)
                    bottomRight.resetTexture();
                if (topLeft != null)
                    topLeft.resetTexture();
                if (topRight != null)
                    topRight.resetTexture();
            }

            for (int i = 0; i < mWallConnections.Length; i++)
                mWallConnections[i] = null;

            if (_registered)
                mSWM.remove(bottomLeft.position());

            base.remove();

            _registered = false;
        }

        public static GameObject getPrefab()
        {
            GameObject retVal = Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/StaticWall");

            if (retVal == null)
                Debug.LogError("_prefab is null!");

            return retVal;
        }

        public PlaceableStaticWall getSide(Utilities.SIDE_4 side)
        {
            PlaceableStaticWall retVal = null;
            Tile adjTile = null;

            switch (side)
            {
                case Utilities.SIDE_4.RIGHT:
                    if (bottomRight != null)
                    {
                        adjTile = bottomRight.getSide(side);

                        if (adjTile != null)
                            retVal = adjTile.getObject() as PlaceableStaticWall;
                    }

                    break;
                case Utilities.SIDE_4.TOP:
                    if (topRight != null)
                    {
                        adjTile = topRight.getSide(side);

                        if (adjTile != null)
                            retVal = adjTile.getObject() as PlaceableStaticWall;
                    }

                    break;
                case Utilities.SIDE_4.LEFT:
                    if (bottomLeft != null)
                    {
                        adjTile = bottomLeft.getSide(side);

                        if (adjTile != null)
                            retVal = adjTile.getObject() as PlaceableStaticWall;
                    }

                    break;
                case Utilities.SIDE_4.BOTTOM:
                    if (bottomRight != null)
                    {
                        adjTile = bottomRight.getSide(side);

                        if (adjTile != null)
                            retVal = adjTile.getObject() as PlaceableStaticWall;
                    }

                    break;
                default:
                    return null;
            }

            return retVal;
        }

        public void updateTexture()
        {
            if (mIsUpdatingWall ||
                bottomLeft == null || bottomRight == null ||
                topLeft == null || topRight == null)
                return;

            mIsUpdatingWall = true;

            bool didUpdate = false;

            PlaceableStaticWall node = getSide(Utilities.SIDE_4.RIGHT);
            if (node != mWallConnections[0])
            {
                mWallConnections[0] = node;
                if (node != null)
                    node.updateTexture();

                didUpdate = true;
            }//*/

            node = getSide(Utilities.SIDE_4.TOP);
            if (node != mWallConnections[1])
            {
                mWallConnections[1] = node;
                if (node != null)
                    node.updateTexture();

                didUpdate = true;
            }//*/

            node = getSide(Utilities.SIDE_4.LEFT);
            if (node != mWallConnections[2])
            {
                mWallConnections[2] = node;
                if (node != null)
                    node.updateTexture();

                didUpdate = true;
            }//*/

            node = getSide(Utilities.SIDE_4.BOTTOM);
            if (node != mWallConnections[3])
            {
                mWallConnections[3] = node;
                if (node != null)
                    node.updateTexture();

                didUpdate = true;
            }//*/

            if (didUpdate || mIsFirstUpdate)
            {
                mIsFirstUpdate = false;

                Texture2D wallTex = getTexture(
                    mWallConnections[0] != null,
                    mWallConnections[1] != null,
                    mWallConnections[2] != null,
                    mWallConnections[3] != null);

                Utilities.Vec2Int texSize = mCM.getTileTexSize();
                Texture2D tempTex = Utilities.Graphics.subTexture(wallTex, 0, 0, texSize.x, texSize.y);
                bottomLeft.setTexture(tempTex);

                tempTex = Utilities.Graphics.subTexture(wallTex, texSize.x, 0, texSize.x, texSize.y);
                bottomRight.setTexture(tempTex);//*/

                tempTex = Utilities.Graphics.subTexture(wallTex, 0, texSize.y, texSize.x, texSize.y);
                topLeft.setTexture(tempTex);//*/

                tempTex = Utilities.Graphics.subTexture(wallTex, texSize.x, texSize.y, texSize.x, texSize.y);
                topRight.setTexture(tempTex);//*/
            }

            mIsUpdatingWall = false;
        }
        #endregion

        #region Instance Methods
        protected override bool isValidTile(int areaX, int areaY, Utilities.Vec2Int tilePos)
        {
            if (areaX < 0 || areaX >= _area.GetLength(0))
                return false;
            else if (areaY < 0 || areaY >= _area.GetLength(1))
                return false;

            if (_area[areaX, areaY])
            {
                Tile tile = getTile(tilePos);

                if (tile == null || tile.hasObject())
                    return false;
            }

            return true;
        }
        protected override void selectUpdate()
        { }

        protected override void detach()
        {
            PlaceableStaticWall rightWall = getSide(Utilities.SIDE_4.RIGHT);
            PlaceableStaticWall topWall = getSide(Utilities.SIDE_4.TOP);
            PlaceableStaticWall leftWall = getSide(Utilities.SIDE_4.LEFT);
            PlaceableStaticWall bottomWall = getSide(Utilities.SIDE_4.BOTTOM);


            base.detach();

            if (rightWall != null)
                rightWall.updateTexture();

            if (topWall != null)
                topWall.updateTexture();

            if (leftWall != null)
                leftWall.updateTexture();

            if (bottomWall != null)
                bottomWall.updateTexture();

            bottomLeft = null;
            bottomRight = null;
            topLeft = null;
            topRight = null;
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

        protected override GameObject loadPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/StaticWall");
        }
        #endregion
    }
}
