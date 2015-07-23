using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;
using Utilities;

/*
 * This script represents a changing wall, and is thus abstract. This class
 * allows for more different type of changing walls to be added easily.
 */
namespace Game
{
    public abstract class DynamicWall : MonoBehaviour, IActivatable, ISerializable, IIdentifiable
    {
        #region Instance Variables
        protected int mCurrentIndex = 0;
        protected float mCurrentChangeTime = 0.0f;

        protected STATE mState;

        private string mID = "";

        private SpriteRenderer mSR;

        private static Texture2D[] mWallTextures = null;
        #endregion

        #region Class Constants
        public static readonly float CHANGE_TIME = 1.0f;

        public enum STATE
        {
            STATIONARY = 0,
            TO_CHANGE,
            CHANGE
        };
        #endregion

        /*
     * Initializes the Dynamic Wall
     */
        protected void Awake()
        {
            mState = STATE.STATIONARY;
            mSR = GetComponent<SpriteRenderer>();

            getID();
        }

        /*
         * Updates the Dynamic Wall.
         */
        protected void Update()
        {
            switch (mState)
            {
                case STATE.STATIONARY:
                    break;
                case STATE.TO_CHANGE:
                    toChange();
                    break;
                case STATE.CHANGE:
                    change();
                    break;
            }
        }

        #region Interface Methods
        /*
     * Only activate() does changes the dynamic wall. Both deactivate and toggle are empty methods
     */
        public void activate() { }
        public void deactivate() { }
        public void toggle()
        {
            if (mState != STATE.CHANGE)
                mState = STATE.TO_CHANGE;
        }

        public virtual IEnumerator serialize(XmlDocument document, System.Action<XmlElement> callback)
        {
            XmlElement wallTag = document.CreateElement("dynamicwall");
            wallTag.SetAttribute("id", getID());
            wallTag.SetAttribute("type", GetType().ToString());

            //Transform Component
            wallTag.AppendChild(Serialization.toXML(transform, document));

            //Sprite Renderer Component
            wallTag.AppendChild(Serialization.toXML(mSR, document));

            //All Box Collider 2D Components
            BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D collider in colliders)
                wallTag.AppendChild(Serialization.toXML(collider, document));

            callback(wallTag);

            return null;
        }

        public virtual string getID()
        {
            if (mID == "")
                mID = Serialization.generateUUID(this);

            return mID;
        }

        public virtual void setID(string id)
        {
            mID = id;
        }

        public static Sprite createWallSprite(int east, int north, int west, int south)
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
                    if (mWallTextures[i] == null)
                        Debug.Log("mWallTextures[" + i + "] is null");
            }

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

            return Sprite.Create(
                wallTexture,
                new Rect(0, 0, wallTexture.width, wallTexture.height),
                new Vector2(0.5f, 0.5f), 160);
        }
        #endregion

        #region Instance Methods
        protected abstract void toChange();
        protected abstract void change();
        #endregion
    }
}
