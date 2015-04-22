using UnityEngine;
using System;
using System.Collections;
using System.Xml;

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

    private DoorManager mDM;

    protected float _angle;
	#endregion

    protected void Awake()
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
	}

    protected void Start()
    {
        base.Start();

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

    public virtual IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
    {
        XmlElement door = doc.CreateElement("door");
        door.SetAttribute("id", getID());
        door.SetAttribute("type", getType());
        door.SetAttribute("initialState", getState());

        door.AppendChild(ComponentSerializer.toXML(transform, doc));

        callback(door);

        return null;
    }

    public override bool place()
    {
        if (!base.place())
            return false;

        mDM.register(this);

        return true;
    }
    public void remove()
    {
        Utilities.Vec2Int pos1 = mAttached[0].position();
        Utilities.Vec2Int pos2 = new Utilities.Vec2Int(0, 0);
        Utilities.Vec2Int basePos;

        for (int i = 1; i < mAttached.Length; i++)
        {
            basePos = mAttached[i].position() / 2 * 2;

            if (!basePos.Equals(pos1))
                pos2 = basePos;
        }

        PlaceableStaticWall.createStaticWall(_map.toTilePos(pos1) + Utilities.toVector3(Map.tileSize));
        PlaceableStaticWall.createStaticWall(_map.toTilePos(pos2) + Utilities.toVector3(Map.tileSize));

        mDM.unregister(getID());

        base.remove();
    }
    #endregion

    #region Instance Methods
    protected override bool isValidTile(int areaX, int areaY, Utilities.Vec2Int tilePos)
    {
        if (areaX < 0 || areaX >= _area.GetLength(1))
            return false;
        else if (areaY < 0 || areaY >= _area.GetLength(2))
            return false;

        if (_area[areaX, areaY])
        {
            Tile tile = getTile(tilePos);

            if (tile == null || !tile.hasObject() || !(tile.getObject() is PlaceableStaticWall))
                return false;
        }

        return true;
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
