﻿using UnityEngine;
using System.Xml;
using System;
using System.Collections.Generic;

public class DoorFactory : Factory<DoorFrame>
{
	#region Instance Variables
    private List<Utilities.Pair<TeleporterDoorFrame, string>> mToConnect;
	#endregion
	
	#region Interface Methods
    public override DoorFrame create(XmlElement element)
    {
        if (element == null || element.Name != "door")
            return null;

        DoorFrame retVal = null;

        switch (element.GetAttribute("type"))
        {
            case "CheckpointDoorFrame":
            case "DirectionDoorFrame":
            case "EntranceDoorFrame":
            case "StandardDoorFrame":
                retVal = createBasicDoor(element);
                break;
            case "ExitDoorFrame":
            case "PowerCoreDoorFrame":
                retVal = createPowerCoreDoor(element);
                break;
            case "TeleporterDoorFrame":
                retVal = createTeleporterDoor(element);
                break;
            case "TimerDoorFrame":
                retVal = createTimerDoorFrame(element);
                break;
        }

        return retVal;
    }
	#endregion
	
	#region Instance Methods
    private DoorFrame createBasicDoor(XmlElement element)
    {
        UnityEngine.Object convertor = null; //Used for the funky generic conversions
        DoorFrame retVal = null;
        GameObject go = null;

        switch (element.GetAttribute("type"))
        {
            case "CheckpointDoorFrame":
                go = Resources.Load<GameObject>("Prefabs/Activatables/Doors/CheckpointDoor");
                convertor = go.GetComponent<CheckpointDoorFrame>();
                break;
            case "DirectionDoorFrame":
                go = Resources.Load<GameObject>("Prefabs/Activatables/Doors/DirectionDoor");
                convertor = go.GetComponent<DirectionalDoorFrame>();
                break;
            case "EntranceDoorFrame":
                go = Resources.Load<GameObject>("Prefabs/Activatables/Doors/EntranceDoor");
                convertor = go.GetComponent<EntranceDoorFrame>();
                break;
            case "StandardDoorFrame":
                go = Resources.Load<GameObject>("Prefabs/Activatables/Doors/StandardDoor");
                convertor = go.GetComponent<StandardDoorFrame>();
                break;
        }
        if (go == null)
            return null;

        go = GameObject.Instantiate(go) as GameObject; //Instantiate the prefab
        
        retVal = convertor as DoorFrame;

        if (convertor == null || retVal == null)
            return null;

        retVal.initialState = 
            Utilities.ParseEnum<DoorFrame.STATE>(element.GetAttribute("initialState"));

        deserializeComponents(go, element);

        return retVal;
    }

    private DoorFrame createPowerCoreDoor(XmlElement element)
    {
        UnityEngine.Object convertor = null; //Used for the funky generic conversions
        DoorFrame retVal = null;
        GameObject go = null;
        PowerCoreDoorFrame.CORES coreCount = PowerCoreDoorFrame.CORES.NONE;

        switch (element.GetAttribute("type"))
        {
            case "PowerCoreDoorFrame":
                coreCount = (PowerCoreDoorFrame.CORES)
                    Enum.Parse(typeof(PowerCoreDoorFrame.CORES), element.GetAttribute("cores"));

                switch (coreCount)
                {
                    case PowerCoreDoorFrame.CORES.NONE:
                        break;
                    case PowerCoreDoorFrame.CORES.ONE:
                        go = Resources.Load<GameObject>("Prefabs/Activatables/Doors/OneCoreDoor");
                        break;
                    case PowerCoreDoorFrame.CORES.TWO:
                        go = Resources.Load<GameObject>("Prefabs/Activatables/Doors/TwoCoreDoor");
                        break;
                    case PowerCoreDoorFrame.CORES.FULL:
                        go = Resources.Load<GameObject>("Prefabs/Activatables/Doors/FullCoreDoor");
                        break;
                }

                if (go == null)
                    return null;

                convertor = go.GetComponent<PowerCoreDoorFrame>();
                break;
            case "ExitDoorFrame":
                go = Resources.Load<GameObject>("Prefabs/Activatables/Doors/ExitDoor");
                convertor = go.GetComponent<ExitDoorFrame>();
                break;
        }
        if (go == null)
            return null;

        go = GameObject.Instantiate(go) as GameObject;

        retVal = convertor as DoorFrame;

        if (convertor == null || retVal == null)
            return null;

        {
            PowerCoreDoorFrame powerCoreDoor = convertor as PowerCoreDoorFrame;
            powerCoreDoor.powerCores = coreCount;
        }

        retVal.initialState =
            Utilities.ParseEnum<DoorFrame.STATE>(element.GetAttribute("initialState"));

        deserializeComponents(go, element);

        return retVal;
    }

    private DoorFrame createTeleporterDoor(XmlElement element)
    {
        UnityEngine.Object convertor = null; //Used for the funky generic conversions
        DoorFrame retVal = null;
        GameObject go = null;

        switch (element.GetAttribute("type"))
        {
            case "TeleporterDoorFrame":
                go = Resources.Load<GameObject>("Prefabs/Activatables/Doors/TeleporterDoor");
                convertor = go.GetComponent<TeleporterDoorFrame>();
                break;
        }
        if (go == null)
            return null;

        go = GameObject.Instantiate(go) as GameObject;

        retVal = convertor as DoorFrame;

        if (convertor == null || retVal == null)
            return null;

        retVal.setID(element.GetAttribute("id"));

        retVal.initialState =
            Utilities.ParseEnum<DoorFrame.STATE>(element.GetAttribute("initialState"));

        deserializeComponents(go, element);

        // Create the connection list if not already created
        if (mToConnect == null)
            mToConnect = new List<Utilities.Pair<TeleporterDoorFrame, string>>();

        TeleporterDoorFrame teleporterDoor = convertor as TeleporterDoorFrame;
        string targetID = element.GetAttribute("targetID");

        Debug.Log("New run.");
        // Iterate over the list to perform the connecting process
        List<Utilities.Pair<TeleporterDoorFrame, string>> toRemove = new List<Utilities.Pair<TeleporterDoorFrame, string>>();
        Utilities.Pair<TeleporterDoorFrame, string> newConnector =
            new Utilities.Pair<TeleporterDoorFrame, string>(teleporterDoor, targetID);
        mToConnect.Add(newConnector);
        newConnector = null;

        foreach (Utilities.Pair<TeleporterDoorFrame, string> door in mToConnect)
        {
            Debug.Log("ID: " + door.first.getID() + ", Target: " + door.second);
            Debug.Log("[DUP] ID: " + door.first.getID() + ", Target: " + door.second);
            /*Debug.Log("Door.second target ID: " + door.second);
            Debug.Log("teleporterDoor ID:     " + teleporterDoor.getID());
            if (door.second == teleporterDoor.getID())
            {
                Debug.Log("old found new");
                door.first.setTarget(teleporterDoor);
                toRemove.Add(door);
            }
                    
            Debug.Log("targetID:      " + targetID);
            Debug.Log("Door.first id: " + door.first.getID());
            if (targetID == door.first.getID())
            {
                Debug.Log("new found old");
                teleporterDoor.setTarget(door.first);
                toRemove.Add(newConnector);
            }*/
        }

        // Remove any connected teleporter doors
        /*foreach (Utilities.Pair<TeleporterDoorFrame, string> door in toRemove)
            mToConnect.Remove(door);

        toRemove.Clear();*/

        return retVal;
    }

    private DoorFrame createTimerDoorFrame(XmlElement element)
    {
        UnityEngine.Object convertor = null; //Used for the funky generic conversions
        DoorFrame retVal = null;
        GameObject go = null;

        switch (element.GetAttribute("type"))
        {
            case "TimerDoorFrame":
                go = Resources.Load<GameObject>("Prefabs/Activatables/Doors/TimerDoor");
                convertor = go.GetComponent<TimerDoorFrame>();
                break;
        }
        if (go == null)
            return null;

        go = GameObject.Instantiate(go) as GameObject;

        retVal = convertor as DoorFrame;

        if (convertor == null || retVal == null)
            return null;

        retVal.initialState =
            Utilities.ParseEnum<DoorFrame.STATE>(element.GetAttribute("initialState"));

        deserializeComponents(go, element);

        {
            TimerDoorFrame timerDoor = convertor as TimerDoorFrame;
            timerDoor.time = Convert.ToInt32(element.GetAttribute("time"));
        }

        return retVal;
    }

    private static void deserializeComponents(GameObject go, XmlElement parent)
    {
        XmlNode child = parent.FirstChild;
        XmlElement component;

        while (child != null)
        {
            component = child as XmlElement;
            if (component != null)
            {
                switch (component.Name)
                {
                    case "transform":
                        ComponentSerializer.deserialize(go.transform, component);
                        break;
                }
            }

            child = child.NextSibling;
        }
    }
	#endregion
}