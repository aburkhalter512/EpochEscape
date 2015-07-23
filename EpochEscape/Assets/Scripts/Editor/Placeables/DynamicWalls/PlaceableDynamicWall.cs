using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Utilities;

namespace Editor
{
    public abstract class PlaceableDynamicWall : PlaceableObject, IPlaceable, ISerializable, IConnectable
    {
        #region Instance Variables
        private SortedList<Utilities.Vec2Int, Tile[]> mTiles;
        private Dictionary<SIDE_4, int> _armLengths;
        private static readonly int maxArmSize = 8;

        //private Tile mBase;

        private static readonly float SELECTION_DELAY = .33f;
        protected float mCurrentDelay = 0.0f;

        protected DynamicWallManager mDWM;

        protected Vector3 mBasePos;

        private bool mResized = false;

        private static Texture2D[] mWallTextures;
        #endregion

        protected override void Awake()
        {
            base.Awake();

            _sr = GetComponent<SpriteRenderer>();

            mTiles = new SortedList<Utilities.Vec2Int, Tile[]>(Utilities.Vec2IntComparer.Get());
            //mEndCaps = new Dictionary<SIDE_4, Tile>();

            _armLengths = new Dictionary<SIDE_4, int>();
            _armLengths[SIDE_4.RIGHT] = 0;
            _armLengths[SIDE_4.TOP] = 0;
            _armLengths[SIDE_4.LEFT] = 0;
            _armLengths[SIDE_4.BOTTOM] = 0;

            int areaWidth = maxArmSize * 4 + 2;
            // Keep size / 2 odd
            _area = new bool[areaWidth, areaWidth];
            for (int i = 0; i < _area.GetLength(0); i++)
                for (int j = 0; j < _area.GetLength(1); j++)
                    _area[i, j] = false;

            // Make only the center placeable
            int centerBase = maxArmSize * 2;
            _area[centerBase, centerBase] = true;
            _area[centerBase, centerBase + 1] = true;
            _area[centerBase + 1, centerBase] = true;
            _area[centerBase + 1, centerBase + 1] = true;

            loadTextures();
        }

        protected override void Start()
        {
            base.Start();

            mDWM = DynamicWallManager.Get();
        }

        #region Interface Methods
        public override IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
        {
            XmlElement dynamicwall = doc.CreateElement("dynamicwall");
            dynamicwall.SetAttribute("id", getID());
            dynamicwall.SetAttribute("type", getType());

            /*Utilities.Vec2Int basePos = mBase.position();
            int east = (mEndCaps[SIDE_4.RIGHT].position().x - basePos.x) / 2;*/
            dynamicwall.SetAttribute("east", _armLengths[SIDE_4.RIGHT].ToString());

            //int north = (mEndCaps[SIDE_4.TOP].position().y - basePos.y) / 2;
            dynamicwall.SetAttribute("north", _armLengths[SIDE_4.TOP].ToString());

            //int west = (basePos.x - mEndCaps[SIDE_4.LEFT].position().x) / 2;
            dynamicwall.SetAttribute("west", _armLengths[SIDE_4.LEFT].ToString());

            //int south = (basePos.y - mEndCaps[SIDE_4.BOTTOM].position().y) / 2;
            dynamicwall.SetAttribute("south", _armLengths[SIDE_4.BOTTOM].ToString());

            XmlElement boxCollider = doc.CreateElement("boxcollider2d");
            Vector2 colliderSize = Map.tileSize;
            colliderSize.x *= _armLengths[SIDE_4.RIGHT] + _armLengths[SIDE_4.LEFT] + 1;
            Vector2 colliderCenter = Vector2.zero;
            colliderCenter.y = (_armLengths[SIDE_4.TOP] - _armLengths[SIDE_4.BOTTOM]) * Map.tileSize.y;
            boxCollider.SetAttribute("size", colliderSize.ToString());
            boxCollider.SetAttribute("center", colliderCenter.ToString());
            dynamicwall.AppendChild(boxCollider);

            boxCollider = doc.CreateElement("boxcollider2d");
            colliderSize = Map.tileSize;
            colliderSize.y *= _armLengths[SIDE_4.TOP] + _armLengths[SIDE_4.BOTTOM] + 1;
            colliderCenter = Vector2.zero;
            colliderCenter.x = (_armLengths[SIDE_4.RIGHT] - _armLengths[SIDE_4.LEFT]) * Map.tileSize.x;
            boxCollider.SetAttribute("size", colliderSize.ToString());
            boxCollider.SetAttribute("center", colliderCenter.ToString());
            dynamicwall.AppendChild(boxCollider);

            dynamicwall.AppendChild(Serialization.toXML(transform, doc));

            callback(dynamicwall);

            return null;
        }

        public new void select()
        {
            base.select();

            mCurrentDelay = 0;
        }

        public override void rotate()
        {
            // Dynamic walls don't need to rotate
        }

        public override void moveTo(Vector3 v)
        {
            if (!started())
                return;

            if (placed())
                remove();

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
        #endregion

        #region Instance Methods
        protected abstract string getType();
        protected override void selectUpdate()
        {
            resize();
            updateTexture();

            mResized = false;
        }

        protected override bool isValidTile(int areaX, int areaY, Utilities.Vec2Int tilePos)
        {
            if (areaX < 0 || areaX >= _area.GetLength(0))
                return false;
            else if (areaY < 0 || areaY >= _area.GetLength(1))
                return false;

            if (_area[areaX, areaY])
            {
                Tile tile = getTile(tilePos);

                if (tile == null || (tile.hasObject() && tile.getObject().getID() != getID()))
                    return false;
            }

            return true;
        }

        private void resize()
        {
            Vector2 inputDelta = _im.secondaryJoystick.get();

            if (inputDelta.sqrMagnitude < .25f)
            {
                mCurrentDelay = -1;
                return;
            }
            else if (Time.realtimeSinceStartup - mCurrentDelay < SELECTION_DELAY)
                return;

            mCurrentDelay = Time.realtimeSinceStartup;

            bool mIsAuxInput = false; // _im.alternateInput.get();

            SIDE_4 side = SIDE_4.TOP; // Just a dummy value

            bool resizedDir = false;
            if (inputDelta.x > .5f)
            {
                side = SIDE_4.RIGHT;
                resizedDir = true;
            }
            else if (inputDelta.x < -.5f)
            {
                side = SIDE_4.LEFT;
                resizedDir = true;
            }

            if (resizedDir)
            {
                Debug.Log("Resizing horizontally");

                if (mIsAuxInput && _armLengths[side] > 0)
                {
                    setArea(side, _armLengths[side], false);
                    _armLengths[side]--;
                }
                else if (_armLengths[side] < maxArmSize)
                {
                    _armLengths[side]++;
                    setArea(side, _armLengths[side], true);
                }

                if (!place())
                {
                    _armLengths[side]--;
                    setArea(side, _armLengths[side], false);

                    place();
                }
            }

            mResized = resizedDir;
            resizedDir = false;
            if (inputDelta.y > .5f)
            {
                side = SIDE_4.TOP;
                resizedDir = true;
            }
            else if (inputDelta.y < -.5f)
            {
                side = SIDE_4.BOTTOM;
                resizedDir = true;
            }

            if (resizedDir)
            {
                if (mIsAuxInput && _armLengths[side] > 0)
                {
                    setArea(side, _armLengths[side], false);
                    _armLengths[side]--;
                }
                else if (_armLengths[side] < maxArmSize)
                {
                    _armLengths[side]++;
                    setArea(side, _armLengths[side], true);
                }

                if (!place())
                {
                    Debug.Log("Reverting!");
                    _armLengths[side]--;
                    setArea(side, _armLengths[side], false);

                    place();
                }
                else
                    Debug.Log("Placed!");
            }

            mResized = mResized || resizedDir;
        }
        private void setArea(SIDE_4 side, int armLength, bool hasTiles)
        {
            if (armLength < 0 || armLength > 8)
                return;

            int realX = maxArmSize * 2;
            int realY = maxArmSize * 2;

            switch (side)
            {
                case SIDE_4.RIGHT:
                    realX += armLength * 2;
                    break;
                case SIDE_4.TOP:
                    realY += armLength * 2;
                    break;
                case SIDE_4.LEFT:
                    realX -= armLength * 2;
                    break;
                case SIDE_4.BOTTOM:
                    realY -= armLength * 2;
                    break;
                default:
                    return;
            }

            _area[realX, realY] = hasTiles;
            _area[realX, realY + 1] = hasTiles;
            _area[realX + 1, realY] = hasTiles;
            _area[realX + 1, realY + 1] = hasTiles;
        }

        private void updateTexture()
        {
            if (!mResized)
                return;

            _sr.sprite = createFullTex(
                _armLengths[SIDE_4.RIGHT],
                _armLengths[SIDE_4.TOP],
                _armLengths[SIDE_4.LEFT],
                _armLengths[SIDE_4.BOTTOM]);
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
        protected static Sprite createFullTex(int east, int north, int west, int south)
        {
            int centerSpriteIndex = 0;

            if (east <= 0)
                east = 0;
            else
                centerSpriteIndex = 0x1;
            if (north <= 0)
                north = 0;
            else
                centerSpriteIndex |= 0x2;
            if (west <= 0)
                west = 0;
            else
                centerSpriteIndex |= 0x4;
            if (south <= 0)
                south = 0;
            else
                centerSpriteIndex |= 0x8;

            Utilities.Pair<int, int> centerIndex =
                new Utilities.Pair<int, int>(west, south);

            Utilities.Pair<int, int> wallTexSize =
                new Utilities.Pair<int, int>(mWallTextures[0].width, mWallTextures[0].height);

            Utilities.Pair<int, int> dimensions =
                new Utilities.Pair<int, int>(west + east + 1, north + south + 1);


            Texture2D wallTexture =
                new Texture2D(dimensions.first * mWallTextures[0].width, dimensions.second * mWallTextures[0].height);
            wallTexture.alphaIsTransparency = true;

            // Clear out all of the pixels
            Color32[] wallTexColors = wallTexture.GetPixels32();
            Color32 clearColor32 = new Color32(0, 0, 0, 0);
            for (int i = 0; i < wallTexColors.Length; i++)
                wallTexColors[i] = clearColor32;
            wallTexture.SetPixels32(wallTexColors);

            // Set the center wall texture
            wallTexture.SetPixels(
                centerIndex.first * wallTexSize.first, centerIndex.second * wallTexSize.second,
                wallTexSize.first, wallTexSize.second,
                mWallTextures[centerSpriteIndex].GetPixels());

            if (west > 0)
            {
                wallTexture.SetPixels(
                    0, south * wallTexSize.second,
                    wallTexSize.first, wallTexSize.second,
                    mWallTextures[1].GetPixels()); //West end cap

                for (int i = 1; i < centerIndex.first; i++)
                    wallTexture.SetPixels(
                        i * wallTexSize.first, south * wallTexSize.second,
                        wallTexSize.first, wallTexSize.second,
                        mWallTextures[5].GetPixels()); //Horizontal Straight
            }
            if (east > 0)
            {
                wallTexture.SetPixels(
                    (west + east) * wallTexSize.first, south * wallTexSize.second,
                    wallTexSize.first, wallTexSize.second,
                    mWallTextures[4].GetPixels()); //East end cap

                for (int i = west + east - 1; i > centerIndex.first; i--)
                    wallTexture.SetPixels(
                        i * wallTexSize.first, south * wallTexSize.second,
                        wallTexSize.first, wallTexSize.second,
                        mWallTextures[5].GetPixels()); //Horizontal Straight
            }

            if (south > 0)
            {
                wallTexture.SetPixels(
                    west * wallTexSize.first, 0,
                    wallTexSize.first, wallTexSize.second,
                    mWallTextures[2].GetPixels()); //South end cap

                for (int i = 1; i < centerIndex.second; i++)
                    wallTexture.SetPixels(
                        west * wallTexSize.first, i * wallTexSize.second,
                        wallTexSize.first, wallTexSize.second,
                        mWallTextures[10].GetPixels()); //Horizontal Straight
            }
            if (north > 0)
            {
                wallTexture.SetPixels(
                    west * wallTexSize.first, (south + north) * wallTexSize.second,
                    wallTexSize.first, wallTexSize.second,
                    mWallTextures[8].GetPixels()); //North end cap

                for (int i = south + north - 1; i > centerIndex.second; i--)
                    wallTexture.SetPixels(
                        west * wallTexSize.first, i * wallTexSize.second,
                        wallTexSize.first, wallTexSize.second,
                        mWallTextures[10].GetPixels()); //Horizontal Straight
            }

            wallTexture.Apply();

            Vector2 pivot = new Vector2(0.5f, 0.5f);
            pivot.x += ((float)(west - east)) / (2 * (west + east + 1));
            pivot.y += ((float)(south - north)) / (2 * (south + north + 1));

            return Sprite.Create(
                wallTexture,
                new Rect(0, 0, wallTexture.width, wallTexture.height),
                pivot, 160);
        }
        #endregion
    }
}
