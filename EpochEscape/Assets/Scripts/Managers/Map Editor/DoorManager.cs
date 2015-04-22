using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class DoorManager : ISerializable
{
	#region Instance Variables
    private static DoorManager mInstance;

    private SortedList<string, PlaceableDoor> mDoors;

    private readonly int PROCESS_COUNT = 80;
	#endregion

    private DoorManager()
    {
        mDoors = new SortedList<string, PlaceableDoor>(Utilities.StringComparer.Get());
    }
	
	#region Interface Methods
    public static DoorManager Get()
    {
        if (mInstance == null)
            mInstance = new DoorManager();

        return mInstance;
    }

    public void register(PlaceableDoor door)
    {
        mDoors.Add(door.getID(), door);
    }
    public void unregister(string id)
    {
        mDoors.Remove(id);
    }

    public IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
    {
        if (callback == null)
            yield break;

        XmlElement doors = doc.CreateElement("doors");

        int count = 0;
        foreach (PlaceableDoor door in mDoors.Values)
        {
            door.serialize(doc, (XmlElement elem) =>
                {
                    doors.AppendChild(elem);
                });

            if (++count == PROCESS_COUNT)
            {
                yield return null;
                count = 0;
            }
        }

        callback(doors);
    }
	#endregion
}
