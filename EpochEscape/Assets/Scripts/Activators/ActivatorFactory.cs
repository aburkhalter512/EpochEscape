using UnityEngine;
using System.Xml;
using System;
using System.Collections.Generic;

public class ActivatorFactory : Factory<Activator>
{
	#region Interface Variables
    public List<IActivatable> toConnect; //ASSUMED TO BE CREATED BY THE FACTORY USER
	#endregion

    #region Instance Variables
    GameObject mTogglePadPrefab;
    GameObject mSwitchPadPrefab;
    GameObject mTerminalPrefab;
    GameObject mOneTimeTerminalPrefab;
    GameObject mFloorActivatorPrefab;
    #endregion

    #region Interface Methods
    public override Activator create(XmlElement element)
    {
        if (element == null || element.Name != "activator")
            return null;

        Activator retVal = null;

        switch (element.GetAttribute("type"))
        {
            case "Terminal":
            case "OneTimeTerminal":
            case "PressurePlate":
            case "PressureSwitch":
            case "FloorActivator":
                retVal = createBasicActivator(element);
                break;
        }

        return retVal;
    }
	#endregion
	
	#region Instance Methods
    private Activator createBasicActivator(XmlElement element)
    {
        Activator retVal = null;
        GameObject go = null;

        switch (element.GetAttribute("type"))
        {
            case "PressurePlate":
                if (mTogglePadPrefab == null)
                    mTogglePadPrefab = Resources.Load<GameObject>("Prefabs/Activators/Pressure Pads/TogglePad");

                go = mTogglePadPrefab;
                break;
            case "PressureSwitch":
                if (mSwitchPadPrefab == null)
                    mSwitchPadPrefab = Resources.Load<GameObject>("Prefabs/Activators/Pressure Pads/SwitchPad");

                go = mSwitchPadPrefab;
                break;
            case "Terminal":
                if (mTerminalPrefab == null)
                    mTerminalPrefab = Resources.Load<GameObject>("Prefabs/Activators/Terminals/Terminal");

                go = mTerminalPrefab;
                break;
            case "OneTimeTerminal":
                if (mOneTimeTerminalPrefab == null)
                    mOneTimeTerminalPrefab = Resources.Load<GameObject>("Prefabs/Activators/Terminals/OneTimeTerminal");

                go = mOneTimeTerminalPrefab;
                break;
            case "FloorActivator":
                if (mFloorActivatorPrefab == null)
                    mFloorActivatorPrefab = Resources.Load<GameObject>("Prefabs/Activators/FloorActivator");

                go = mFloorActivatorPrefab;
                break;
        }
        if (go == null)
            return null;

        go = GameObject.Instantiate(go) as GameObject;
        retVal = go.GetComponent<Activator>();

        if (retVal == null)
            return null;

        deserializeComponents(go, element, retVal);

        return retVal;
    }

    private void deserializeComponents(GameObject go, XmlElement parent, Activator activator)
    {
        XmlNode child = parent.FirstChild;
        XmlElement component;

        IActivatable target = null;
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
                    case "activatable":
                        string targetID = component.GetAttribute("id");
                        foreach (IActivatable actuator in toConnect)
                        {
                            IIdentifiable identifier = actuator as IIdentifiable;

                            if (identifier != null && targetID == identifier.getID())
                            {
                                target = actuator;
                                break;
                            }
                        }

                        if (target != null)
                        {
                            activator.addActivatable(target);
                            target = null;
                        }
                        else
                            Debug.Log("target not found!");
                        break;
                }
            }

            child = child.NextSibling;
        }
    }
	#endregion
}
