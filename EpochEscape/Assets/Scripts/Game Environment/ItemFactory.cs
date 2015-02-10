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
            case "powercore":
                retVal = createBasicPowerCore(element);
                break;
        }

        return retVal;
    }
	#endregion
	
	#region Instance Methods
    private MonoBehaviour createBasicPowerCore(XmlElement element)
    {
        GameObject go = Resources.Load<GameObject>("Prefabs/Game Environment/Items/PowerCore");
        if (go == null)
            return null;

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
                    case "spriterenderer":
                        ComponentSerializer.deserialize(go.GetComponent<SpriteRenderer>(), component);
                        break;
                }
            }

            child = child.NextSibling;
        }
    }
	#endregion
}
