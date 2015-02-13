using UnityEngine;
using System.Xml;
using System;
using System.Collections.Generic;

public class ItemFactory : Factory<MonoBehaviour>
{
	#region Interface Methods
    public override MonoBehaviour create(XmlElement element)
    {
        if (element == null || element.Name != "item")
            return null;
        MonoBehaviour retVal = null;

        switch (element.GetAttribute("type"))
        {
            case "PowerCore":
                retVal = createBasicPowerCore(element);
                break;
        }

        return retVal;
    }
	#endregion
	
	#region Instance Methods
    private MonoBehaviour createBasicPowerCore(XmlElement element)
    {
        GameObject go = null;
        switch (Convert.ToInt32(element.GetAttribute("core")))
        {
            case 1:
                go = Resources.Load<GameObject>("Prefabs/Game Environment/Items/PowerCore1");
                break;
            case 2:
                go = Resources.Load<GameObject>("Prefabs/Game Environment/Items/PowerCore2");
                break;
            case 3:
                go = Resources.Load<GameObject>("Prefabs/Game Environment/Items/PowerCore3");
                break;
        }

        if (go == null)
        {
            Debug.Log("go is null");
            return null;
        }

        go = GameObject.Instantiate(go) as GameObject;
        PowerCore retVal = go.GetComponent<PowerCore>();

        deserializeComponents(go, element);

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
