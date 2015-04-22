using UnityEngine;
using System;
using System.Collections;
using System.Xml;

public abstract class PlaceableObject : MonoBehaviour, IPlaceable, IConnectable
{
	#region Interface Variables
    public GameObject selectionHighlighter;
	#endregion
	
	#region Instance Variables
    protected SpriteRenderer _sr;
    protected SpriteRenderer mSelection;

    protected bool mIsSelected = false;

    protected string mID = "";

    protected bool[,] _area;
    protected Utilities.SIDE_4 _orientation;

    protected Tile[] mAttached = null;

    protected Map _map;
    protected TileCursor mTC;
	#endregion
	
	protected virtual void Awake()
    {
	}
	
	protected virtual void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        mSelection = selectionHighlighter.GetComponent<SpriteRenderer>();
        mSelection.gameObject.SetActive(false);

        _map = Map.Get();
        mTC = TileCursor.Get();
	}

    #region Interface Methods
    public string getID()
    {
        if (mID == "")
            mID = Utilities.generateUUID(this);

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

        GameObject.Destroy(gameObject);
    }

    public virtual void rotate()
    {
        _orientation = Utilities.rotateLeft(_orientation);

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

        updateColor();
    }
    public virtual bool canPlace()
    {
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
        transform.position = _map.toTilePos(v) - Utilities.toVector3(Map.tileSize / 2);
    }
    public virtual bool place()
    {
        if (!canPlace())
            return false;

        detach();

        Tile[] tiles = new Tile[8];
        int tileCounter = 0;
        processTiles((int x, int y, Utilities.Vec2Int tilePos) =>
        {
            tiles[tileCounter++] = getTile(tilePos);
        });

        attach(tiles);

        return true;
    }
	#endregion

    #region Instance Methods
    protected Tile getTile(Utilities.Vec2Int v)
    {
        Utilities.Vec2Int basePos;

        if (_area.GetLength(1) % 2 == 0) //Is it even
            basePos = _map.toLogicalTilePos(transform.position + Utilities.toVector3(Map.tileSize / 2));
        else
            basePos = _map.toLogicalTilePos(transform.position);

        return _map.getExistingTile(basePos + v);
    }

    protected abstract bool isValidTile(int areaX, int areaY, Utilities.Vec2Int tilePos);

    protected void processTiles(Action<int, int, Utilities.Vec2Int> processor)
    {
        Utilities.Vec2Int tilePos = new Utilities.Vec2Int(0, 0);

        switch (_orientation)
        {
            case Utilities.SIDE_4.RIGHT:
                for (int i = 0; i < _area.GetLength(1); i++)
                {
                    tilePos.x = i - _area.GetLength(1) / 2;

                    for (int j = 0; j < _area.GetLength(2); j++)
                    {
                        tilePos.y = j - _area.GetLength(2) / 2;

                        processor(i, j, tilePos);
                    }
                }

                return;
            case Utilities.SIDE_4.TOP:
                for (int i = 0; i < _area.GetLength(1); i++)
                {
                    tilePos.y = i - _area.GetLength(1) / 2;

                    for (int j = 0; j < _area.GetLength(2); j++)
                    {
                        tilePos.x = j - _area.GetLength(2) / 2;

                        processor(i, j, tilePos);
                    }
                }

                return;
            case Utilities.SIDE_4.LEFT:
                for (int i = 0; i < _area.GetLength(1); i++)
                {
                    tilePos.x = _area.GetLength(1) / 2 - i - 1;

                    for (int j = 0; j < _area.GetLength(2); j++)
                    {
                        tilePos.y = _area.GetLength(2) / 2 - j - 1;

                        processor(i, j, tilePos);
                    }
                }

                return;
            case Utilities.SIDE_4.BOTTOM:
                for (int i = 0; i < _area.GetLength(1); i++)
                {
                    tilePos.y = i - _area.GetLength(1) / 2;

                    for (int j = 0; j < _area.GetLength(2); j++)
                    {
                        tilePos.x = j - _area.GetLength(2) / 2;

                        processor(i, j, tilePos);
                    }
                }

                return;
        }
    }

    protected void attach(Tile[] tiles)
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
        foreach (Tile tile in mAttached)
            tile.removeObject();

        mAttached = null;
    }

    protected void updateColor()
    {
        if (canPlace())
            _sr.color = Color.white;
        else
            _sr.color = Color.red;
    }
    #endregion
}
