using UnityEngine;
using System.Xml;
using System;
using System.Collections.Generic;

public class DoorFactory : Factory<DoorFrame>
{
	#region Instance Variables
    private List<Utilities.Pair<TeleporterDoorFrame, string>> mToConnect;

    private GameObject mCheckpointPrefab;
    private GameObject mDirectionalPrefab;
    private GameObject mEntrancePrefab;
    private GameObject mStandardPrefab;
    private GameObject mExitPrefab;
    private GameObject mPowerCore1Prefab;
    private GameObject mPowerCore2Prefab;
    private GameObject mPowerCore3Prefab;
    private GameObject mTeleporterPrefab;
    private GameObject mTimerPrefab;
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
            case "DirectionalDoorFrame":
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
        DoorFrame retVal = null;
        GameObject go = null;

        switch (element.GetAttribute("type"))
        {
            case "CheckpointDoorFrame":
                if (mCheckpointPrefab == null)
                    mCheckpointPrefab = Resources.Load<GameObject>("Prefabs/Activatables/Doors/CheckpointDoor");

                go = mCheckpointPrefab;
                break;
            case "DirectionalDoorFrame":
                if (mDirectionalPrefab == null)
                    mDirectionalPrefab = Resources.Load<GameObject>("Prefabs/Activatables/Doors/DirectionalDoor");

                go = mDirectionalPrefab;
                break;
            case "EntranceDoorFrame":
                if (mEntrancePrefab == null)
                    mEntrancePrefab = Resources.Load<GameObject>("Prefabs/Activatables/Doors/EntranceDoor");

                go = mEntrancePrefab;
                break;
            case "StandardDoorFrame":
                if (mStandardPrefab == null)
                    mStandardPrefab = Resources.Load<GameObject>("Prefabs/Activatables/Doors/StandardDoor");

                go = mStandardPrefab;
                break;
        }
        if (go == null)
            return null;

        go = GameObject.Instantiate(go) as GameObject; //Instantiate the prefab
        retVal = go.GetComponent<DoorFrame>();

        if (retVal == null)
            return null;

        retVal.setID(element.GetAttribute("id"));
        retVal.initialState = 
            Utilities.ParseEnum<DoorFrame.STATE>(element.GetAttribute("initialState"));

        deserializeComponents(go, element);

        return retVal;
    }

    private DoorFrame createPowerCoreDoor(XmlElement element)
    {
        PowerCoreDoorFrame retVal = null;
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
                        if (mPowerCore1Prefab == null)
                            mPowerCore1Prefab = Resources.Load<GameObject>("Prefabs/Activatables/Doors/OneCoreDoor");

                        go = mPowerCore1Prefab;
                        break;
                    case PowerCoreDoorFrame.CORES.TWO:
                        if (mPowerCore2Prefab == null)
                            mPowerCore2Prefab = Resources.Load<GameObject>("Prefabs/Activatables/Doors/TwoCoreDoor");

                        go = mPowerCore2Prefab;
                        break;
                    case PowerCoreDoorFrame.CORES.FULL:
                        if (mPowerCore3Prefab == null)
                            mPowerCore3Prefab = Resources.Load<GameObject>("Prefabs/Activatables/Doors/FullCoreDoor");

                        go = mPowerCore3Prefab;
                        break;
                }

                if (go == null)
                    return null;
                break;
            case "ExitDoorFrame":
                if (mExitPrefab == null)
                    mExitPrefab = Resources.Load<GameObject>("Prefabs/Activatables/Doors/ExitDoor");

                go = mExitPrefab;
                break;
        }
        if (go == null)
            return null;

        go = GameObject.Instantiate(go) as GameObject;
        retVal = go.GetComponent<PowerCoreDoorFrame>();

        if (retVal == null)
            return null;

        retVal.powerCores = coreCount;

        retVal.setID(element.GetAttribute("id"));
        retVal.initialState =
            Utilities.ParseEnum<DoorFrame.STATE>(element.GetAttribute("initialState"));

        deserializeComponents(go, element);

        return retVal;
    }

    private DoorFrame createTeleporterDoor(XmlElement element)
    {
        TeleporterDoorFrame retVal = null;
        GameObject go = null;

        switch (element.GetAttribute("type"))
        {
            case "TeleporterDoorFrame":
                if (mTeleporterPrefab == null)
                    mTeleporterPrefab = Resources.Load<GameObject>("Prefabs/Activatables/Doors/TeleporterDoor");

                go = mTeleporterPrefab;
                break;
        }
        if (go == null)
            return null;

        go = GameObject.Instantiate(go) as GameObject;
        retVal = go.GetComponent<TeleporterDoorFrame>();

        if (retVal == null)
            return null;

        retVal.setID(element.GetAttribute("id"));
        retVal.initialState =
            Utilities.ParseEnum<DoorFrame.STATE>(element.GetAttribute("initialState"));

        deserializeComponents(go, element);

        // Create the connection list if not already created
        if (mToConnect == null)
            mToConnect = new List<Utilities.Pair<TeleporterDoorFrame, string>>();

        string targetID = element.GetAttribute("targetID");

        // Iterate over the list to perform the connecting process
        List<Utilities.Pair<TeleporterDoorFrame, string>> toRemove = new List<Utilities.Pair<TeleporterDoorFrame, string>>();
        Utilities.Pair<TeleporterDoorFrame, string> newConnector =
            new Utilities.Pair<TeleporterDoorFrame, string>(retVal, targetID);
        mToConnect.Add(newConnector);
        newConnector = null;

        foreach (Utilities.Pair<TeleporterDoorFrame, string> door in mToConnect)
        {
            if (door.second == retVal.getID())
            {
                door.first.setTarget(retVal);
                toRemove.Add(door);
            }
                    
            if (targetID == door.first.getID())
            {
                retVal.setTarget(door.first);
                toRemove.Add(newConnector);
            }
        }

        // Remove any connected teleporter doors
        foreach (Utilities.Pair<TeleporterDoorFrame, string> door in toRemove)
            mToConnect.Remove(door);

        toRemove.Clear();

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
                if (mTimerPrefab == null)
                    mTimerPrefab = Resources.Load<GameObject>("Prefabs/Activatables/Doors/TimerDoor");

                go = mTimerPrefab;
                break;
        }
        if (go == null)
            return null;

        go = GameObject.Instantiate(go) as GameObject;
        convertor = go.GetComponent<TimerDoorFrame>();

        retVal = convertor as DoorFrame;

        if (convertor == null || retVal == null)
            return null;

        retVal.setID(element.GetAttribute("id"));
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
