using UnityEngine;
using System;
using System.Collections;
using System.Xml;

public class TeleporterDoor : PlaceableDoor, IConnector, IHighlighteable
{
	#region Interface Variables
	#endregion
	
	#region Instance Variables
    TeleporterDoor mDestination;
	#endregion
	
	#region Interface Methods
    public override void activate()
    {
        mFrontSide.activate();

        mIsActive = true;
    }
    public override void deactivate()
    {
        mBackSide.deactivate();

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

    public void connect(IConnectable door)
    {
        if (door == null)
            return;

        TeleporterDoor tDoor = door as TeleporterDoor;
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
	#endregion
	
	#region Instance Methods
    protected override string getType()
    {
        return "TeleporterDoorFrame";
    }
	#endregion
}
