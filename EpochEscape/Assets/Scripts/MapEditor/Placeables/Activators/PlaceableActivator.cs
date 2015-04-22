using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class PlaceableActivator : PlaceableObject, IPlaceable, ISerializable, IConnector
{
	#region Interface Variables
    public enum TYPE
    {
        PressurePlate,
        PressureSwitch,
        OneTimeTerminal,
        Terminal
    }

    public TYPE type;
	#endregion
	
	#region Instance Variables
    private SortedList<string, IConnectable> mConnections;

    protected ActivatorManager mAM;
	#endregion

    protected override void Awake()
    {
        base.Awake();

        mConnections = new SortedList<string, IConnectable>();

        _area = new bool[2, 2];
        _area[0, 0] = true;
        _area[0, 1] = true;
        _area[1, 0] = true;
        _area[1, 1] = true;
    }

	protected override void Start()
	{
        base.Start();

        mAM = ActivatorManager.Get();
	}
	
	#region Interface Methods
    public virtual IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
    {
        XmlElement activator = doc.CreateElement("activator");
        activator.SetAttribute("id", getID());
        activator.SetAttribute("type", getType());

        activator.AppendChild(ComponentSerializer.toXML(transform, doc));

        foreach (IConnectable connection in mConnections.Values)
        {
            XmlElement activatable = doc.CreateElement("activatable");
            activatable.SetAttribute("id", connection.getID());

            activator.AppendChild(activatable);
        }

        callback(activator);

        return null;
    }

    public virtual void select()
    {
        base.select();

        foreach (IConnectable connection in mConnections.Values)
            connection.highlight(Color.yellow);
    }
    public virtual void deselect()
    {
        base.deselect();

        foreach (IConnectable connection in mConnections.Values)
            connection.unlight();
    }

    public void connect(IConnectable connection)
    {
        if (connection == null)
            return;

        IConnectable searcher;
        if (mConnections.TryGetValue(connection.getID(), out searcher))
        {
            disconnect(connection.getID());
        }
        else
        {
            mConnections.Add(connection.getID(), connection);

            if (mIsSelected)
                connection.highlight(Color.yellow);
        }
    }
    public void disconnect(string id)
    {
        IConnectable searcher;
        if (mConnections.TryGetValue(id, out searcher))
        {
            mConnections.Remove(id);

            if (mIsSelected)
                searcher.unlight();
        }
    }

    public override bool place()
    {
        if (!base.place())
            return false;

        mAM.register(this);

        return true;
    }
    public override void remove()
    {
        mAM.unregister(getID());

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

            if (tile == null || tile.hasObject())
                return false;
        }

        return true;
    }

    private string getType()
    {
        return type.ToString();
    }
	#endregion
}
