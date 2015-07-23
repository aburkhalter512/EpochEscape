using UnityEngine;
using System;
using System.Collections;
using System.Xml;
using Utilities;

namespace Editor
{
    public abstract class PlaceableDoor : PlaceableObject,
        IActivatable, IToggleable, ISerializable, IPlaceable, IConnectable
    {
        #region Interface Variables
        public GameObject frontSide;
        public GameObject backSide;
        #endregion

        #region Instance Variables
        protected bool mIsActive;

        protected PlaceableDoorSide mFrontSide;
        protected PlaceableDoorSide mBackSide;

        private static DoorManager mDM;

        protected float _angle;
        #endregion

        protected override void Awake()
        {
            base.Awake();

            _area = new bool[4, 2];
            _area[0, 0] = true;
            _area[0, 1] = true;
            _area[1, 0] = true;
            _area[1, 1] = true;
            _area[2, 0] = true;
            _area[2, 1] = true;
            _area[3, 0] = true;
            _area[3, 1] = true;

            mIsActive = true;

            mFrontSide = frontSide.GetComponent<PlaceableDoorSide>();
            mBackSide = backSide.GetComponent<PlaceableDoorSide>();

            mAttached = new Tile[8];
            for (int i = 0; i < mAttached.Length; i++)
                mAttached[i] = null;

            _angle = 0.0f;

            _prefab = prefab();
        }

        protected override void Start()
        {
            base.Start();

            if (mDM == null)
                mDM = DoorManager.Get();
        }

        #region Interface Methods
        public abstract void activate();
        public abstract void deactivate();

        public void toggle()
        {
            if (mIsActive)
                deactivate();
            else
                activate();
        }

        public override IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
        {
            XmlElement door = doc.CreateElement("door");
            door.SetAttribute("id", getID());
            door.SetAttribute("type", getType());
            door.SetAttribute("initialState", getState());

            door.AppendChild(Serialization.toXML(transform, doc));

            callback(door);

            return null;
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
            switch (_orientation)
            {
                case Utilities.SIDE_4.RIGHT:
                case Utilities.SIDE_4.LEFT:
                    if (tileCounter.x % 2 != 0)
                        tileCounter.x++;
                    if (tileCounter.y % 2 == 0)
                        tileCounter.y++;

                    break;
                case Utilities.SIDE_4.TOP:
                case Utilities.SIDE_4.BOTTOM:
                    if (tileCounter.x % 2 == 0)
                        tileCounter.x++;
                    if (tileCounter.y % 2 != 0)
                        tileCounter.y++;

                    break;
            }

            Vector3 snappedPosition = new Vector3(
                tileCounter.x * Map.tileSize.x,
                tileCounter.y * Map.tileSize.y);

            base.moveTo(snappedPosition);
        }
        public override bool place()
        {
            if (!started() || !base.place())
                return false;

            if (!_registered)
            {
                mDM.register(this);
                _registered = true;
            }

            return true;
        }
        public override void stateUpdate()
        {
            if (!started())
                return;

            if (_im.toggleActivate.getUp())
            {
                Debug.Log("Toggling");
                toggle();
            }
        }
        public override void remove()
        {
            if (placed())
            {
                Vec2Int pos1 = mAttached[0].position();
                Vec2Int.floorTo(pos1, 2);
                Vec2Int pos2 = null;

                for (int i = 0; i < mAttached.Length; i++)
                {
                    pos2 = mAttached[i].position();
                    Vec2Int.floorTo(pos2, 2);

                    if (!pos2.Equals(pos1))
                        break;
                }

                /*Debug.Log("pos1: " + pos1.ToString() + ", pos2: " + pos2.ToString());
                Debug.Log("pos1: " + (_map.toTilePos(pos1) + Utilities.toVector3(Map.tileSize) / 2));
                Debug.Log("pos2: " + (_map.toTilePos(pos2) + Utilities.toVector3(Map.tileSize) / 2));*/

                PlaceableStaticWall.createStaticWall(_map.toTilePos(pos1) + Utilities.Math.toVector3(Map.tileSize) / 2);
                PlaceableStaticWall.createStaticWall(_map.toTilePos(pos2) + Utilities.Math.toVector3(Map.tileSize) / 2);
            }

            if (_registered)
            {
                mDM.unregister(getID());
                _registered = false;
            }

            base.remove();
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

                if (tile == null || !tile.hasObject() || !(tile.getObject() is PlaceableStaticWall))
                    return false;
            }

            return true;
        }
        protected override void selectUpdate()
        {
            if (_im.toggleActivate.getUp())
            {
                Debug.Log("Toggling");
                toggle();
            }
        }

        protected override void attach(Tile[] tiles)
        {
            PlaceableStaticWall sw1 = tiles[0].getObject() as PlaceableStaticWall;
            PlaceableStaticWall sw2 = null;

            for (int i = 1; i < tiles.Length; i++)
            {
                sw2 = tiles[i].getObject() as PlaceableStaticWall;
                if (!sw1.bottomLeft.position().Equals(sw2.bottomLeft.position()))
                    break;
            }

            sw1.remove();
            GameObject.Destroy(sw1.gameObject);

            sw2.remove();
            GameObject.Destroy(sw2.gameObject);

            base.attach(tiles);
        }

        protected abstract string getType();

        private string getState()
        {
            if (mIsActive)
                return "ACTIVE";
            else
                return "INACTIVE";
        }
        #endregion
    }
}
