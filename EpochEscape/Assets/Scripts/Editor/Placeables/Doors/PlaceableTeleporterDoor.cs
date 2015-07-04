using UnityEngine;
using System;
using System.Collections;
using System.Xml;

public class PlaceableTeleporterDoor : PlaceableDoor, IConnector
{
	#region Instance Variables
    PlaceableTeleporterDoor mDestination;
	#endregion

    protected override void Start()
    {
        base.Start();

        mFrontSide.activate();
        mBackSide.deactivate();
    }

	#region Interface Methods
    public override void activate()
    {
        mFrontSide.activate();

        mIsActive = true;
    }
    public override void deactivate()
    {
        mFrontSide.deactivate();

        mIsActive = false;
    }

    public override void select()
    {
        base.select();

        if (mDestination != null)
            mDestination.highlight(Color.yellow);
    }
    public override void deselect()
    {
        base.deselect();

        if (mDestination != null)
            mDestination.unlight();
    }
    protected override void selectUpdate()
    {
        base.selectUpdate();

        if (_im.secondarySelect.getUp())
        {
            Tile tile = _map.getTile(_im.mouse.inWorld());
            if (tile != null && tile.hasObject())
                connect(tile.getObject());
        }
    }

    public void connect(IConnectable door)
    {
        if (door == null)
            return;

        PlaceableTeleporterDoor tDoor = door as PlaceableTeleporterDoor;
        if (tDoor == null)
            return;

        if (mDestination != null)
            disconnect(mDestination.getID());

        if (mIsSelected)
            tDoor.highlight(Color.yellow);
        mDestination = tDoor;
    }
    public void disconnect(string id)
    {
        if (mDestination == null)
            return;

        if (id == mDestination.getID())
        {
            if (mIsSelected)
                mDestination.unlight();
            mDestination = null;
        }
    }
    public override IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
    {
        base.serialize(doc, (XmlElement door) =>
        {
            string targetID = "";
            if (mDestination != null)
                targetID = mDestination.getID();

            door.SetAttribute("targetID", targetID);

            callback(door);
        });

        return null;
    }

    public static GameObject getPrefab()
    {
        GameObject retVal = Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/TeleporterDoor");

        return retVal;
    }
	#endregion
	
	#region Instance Methods
    protected override string getType()
    {
        return "TeleporterDoorFrame";
    }

    protected override GameObject loadPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/TeleporterDoor");
    }
	#endregion
}
