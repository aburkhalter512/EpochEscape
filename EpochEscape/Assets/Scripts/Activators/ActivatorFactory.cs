using UnityEngine;
using System.Xml;
using System;
using System.Collections.Generic;

public class ActivatorFactory : Factory<Activator>
{
	#region Interface Variables
    public List<IActivatable> toConnect; //ASSUMED TO BE CREATED BY THE FACTORY USER
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
                go = Resources.Load<GameObject>("Prefabs/Activators/Pressure Pads/TogglePad");
                retVal = go.GetComponent<PressurePlate>();
                break;
            case "PressurePad":
                go = Resources.Load<GameObject>("Prefabs/Activators/Pressure Pads/SwitchPad");
                retVal = go.GetComponent<PressureSwitch>();
                break;
            case "Terminal":
                go = Resources.Load<GameObject>("Prefabs/Activators/Terminals/Terminal");
                retVal = go.GetComponent<Terminal>();
                break;
            case "OneTimeTerminal":
                go = Resources.Load<GameObject>("Prefabs/Activators/Terminals/OneTimeTerminal");
                retVal = go.GetComponent<OneTimeTerminal>();
                break;
            case "FloorActivator":
                go = Resources.Load<GameObject>("Prefabs/Activators/Floor Activator");
                retVal = go.GetComponent<FloorActivator>();
                break;
        }
        if (go == null)
            return null;

        go = GameObject.Instantiate(go) as GameObject;

        if (retVal == null)
            return null;

        deserializeComponents(go, element);

        return retVal;
    }

    private void deserializeComponents(GameObject go, XmlElement parent)
    {
        XmlNode child = parent.FirstChild;
        XmlElement component;

        List<IActivatable> targets = new List<IActivatable>();
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
                            targets.Add(target);
                            target = null;
                        }
                        break;
                }
            }

            child = child.NextSibling;
        }
    }
	#endregion
}
