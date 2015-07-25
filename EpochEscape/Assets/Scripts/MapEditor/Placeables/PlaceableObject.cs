using UnityEngine;
using System;
using System.Collections;
using System.Xml;
using Utilities;

namespace MapEditor
{
    public abstract class PlaceableObject : MonoBehaviour, IPlaceable, IConnectable
    {
        #region Interface Variables
        public GameObject selectionHighlighter;
        #endregion

        #region Instance Variables
        protected SpriteRenderer _sr;
        protected SpriteRenderer mSelection;

        protected GameObject _prefab;

        protected bool mIsSelected = false;

        protected string mID = "";

        protected bool[,] _area;
        protected SIDE_4 _orientation;

        protected Tile[] mAttached = null;

        protected Map _map;
        protected InputManager _im;

        private bool _started;
        private bool _placed;

        protected bool _registered;
        #endregion

        protected virtual void Awake()
        {
            _started = false;
            _placed = false;
        }

        protected virtual void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
            mSelection = selectionHighlighter.GetComponent<SpriteRenderer>();
            mSelection.gameObject.SetActive(false);

            _map = Map.Get();
            _im = InputManager.Get();

            _started = true;

            if (_prefab != null)
                _prefab = prefab();
        }

        protected virtual void Update()
        {
            if (mIsSelected)
                selectUpdate();
        }

        #region Interface Methods
        public static IEnumerator placeAtStart(PlaceableObject obj, Vector3 v)
        {
            while (!obj.started())
            {
                yield return null;
            }

            obj.moveTo(v);
            if (!obj.place())
                GameObject.Destroy(obj.gameObject);
        }
        public bool started()
        {
            return _started;
        }
        public bool placed()
        {
            return _placed;
        }

        public string getID()
        {
            if (mID == "")
                mID = Serialization.generateUUID(this);

            return mID;
        }
        public void setID(string id)
        {
            mID = id;
        }

        public virtual IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
        {
            return null;
        }

        public virtual void select()
        {
            mIsSelected = true;

            highlight(Color.cyan);
        }
        public virtual void deselect()
        {
            mIsSelected = false;

            unlight();
        }

        public void highlight(Color color)
        {
            mSelection.color = color;
            mSelection.gameObject.SetActive(true);
            selectionHighlighter.SetActive(true);
        }
        public void unlight()
        {
            mSelection.gameObject.SetActive(false);
        }

        public virtual void remove()
        {
            detach();

            _placed = false;
        }

        public virtual void rotate()
        {
            bool wasPlaced = placed();
            if (wasPlaced)
                remove();

            _orientation = Side.rotateLeft(_orientation);

            switch (_orientation)
            {
                case Utilities.SIDE_4.RIGHT:
                    transform.localEulerAngles = new Vector3(0, 0, 0.0f);
                    break;
                case Utilities.SIDE_4.TOP:
                    transform.localEulerAngles = new Vector3(0, 0, 90.0f);
                    break;
                case Utilities.SIDE_4.LEFT:
                    transform.localEulerAngles = new Vector3(0, 0, 180.0f);
                    break;
                case Utilities.SIDE_4.BOTTOM:
                    transform.localEulerAngles = new Vector3(0, 0, 270.0f);
                    break;
            }

            rotateArea();

            if (wasPlaced)
                place();

            updateColor();
        }
        public virtual bool canPlace()
        {
            if (!started())
                return false;

            bool validTiles = true;

            processTiles((int x, int y, Utilities.Vec2Int tilePos) =>
                {
                    if (!isValidTile(x, y, tilePos))
                        validTiles = false;
                });

            return validTiles;
        }
        public virtual void moveTo(Vector3 v)
        {
            if (!started())
                return;

            transform.position = _map.toTilePos(v);

            // If the area is even, then we need to offset the position by half a tile
            if (_area.GetLength(0) % 2 == 0) //Is it even
                transform.position -= Utilities.Math.toVector3(Map.tileSize / 2);

            updateColor();
        }
        public virtual bool place()
        {
            if (!canPlace())
                return false;

            _placed = true;

            detach();

            Tile[] tiles = getTiles();

            attach(tiles);

            return true;
        }
        public virtual void stateUpdate()
        { }

        public GameObject prefab()
        {
            if (_prefab == null)
                _prefab = loadPrefab();

            return _prefab;
        }
        #endregion

        #region Instance Methods
        protected Tile[] getTiles()
        {
            int tileCount = 0;
            for (int i = 0; i < _area.GetLength(0); i++)
                for (int j = 0; j < _area.GetLength(1); j++)
                    if (_area[i, j])
                        tileCount++;

            if (tileCount == 0)
                return null;

            Tile[] tiles = new Tile[tileCount];
            int tileCounter = 0;
            processTiles((int x, int y, Utilities.Vec2Int tilePos) =>
            {
                if (_area[x, y])
                    tiles[tileCounter++] = getTile(tilePos);
            });

            return tiles;
        }
        protected Tile getTile(Utilities.Vec2Int v)
        {
            Utilities.Vec2Int basePos;

            if (_area.GetLength(0) % 2 == 0) //Is it even
                basePos = _map.toLogicalTilePos(transform.position + Utilities.Math.toVector3(Map.tileSize / 2));
            else
                basePos = _map.toLogicalTilePos(transform.position);

            return _map.getExistingTile(basePos + v);
        }

        protected abstract bool isValidTile(int areaX, int areaY, Utilities.Vec2Int tilePos);

        protected void processTiles(Action<int, int, Utilities.Vec2Int> processor)
        {
            Utilities.Vec2Int tilePos = new Utilities.Vec2Int(0, 0);

            for (int i = 0; i < _area.GetLength(0); i++)
            {
                tilePos.x = i - _area.GetLength(0) / 2;

                for (int j = 0; j < _area.GetLength(1); j++)
                {
                    tilePos.y = j - _area.GetLength(1) / 2;

                    processor(i, j, tilePos);
                }
            }
        }

        protected virtual void attach(Tile[] tiles)
        {
            mAttached = new Tile[tiles.Length];

            for (int i = 0; i < tiles.Length; i++)
            {
                mAttached[i] = tiles[i];
                mAttached[i].attachObject(this);
            }
        }
        protected virtual void detach()
        {
            if (mAttached == null)
                return;

            foreach (Tile tile in mAttached)
                if (tile != null)
                    tile.removeObject();

            mAttached = null;
        }

        protected abstract void selectUpdate();

        protected void updateColor()
        {
            if (canPlace())
                _sr.color = Color.white;
            else
                _sr.color = Color.red;
        }

        protected abstract GameObject loadPrefab();

        protected virtual void rotateArea(bool direction = true /*true == rotate left*/)
        {
            if (_area == null)
                return;

            bool[,] newArea = new bool[_area.GetLength(1), _area.GetLength(0)];

            if (direction)
            {
                for (int i = 0; i < _area.GetLength(0); i++)
                    for (int j = 0; j < _area.GetLength(1); j++)
                        newArea[j, i] = _area[i, j];
            }
            else
            {
                for (int i = 0; i < _area.GetLength(0); i++)
                    for (int j = 0; j < _area.GetLength(1); j++)
                        newArea[_area.GetLength(1) - j, _area.GetLength(0) - i] = _area[i, j];
            }

            _area = newArea;
        }
        #endregion
    }
}
