using UnityEngine;
using System.Xml;
using System;
using System.Collections.Generic;

public class DynamicWallFactory : Factory<DynamicWall>
{
	#region Instance Variables
    private List<Utilities.Pair<TeleporterDoorFrame, string>> mToConnect;
	#endregion
	
	#region Interface Methods
    public override DynamicWall create(XmlElement element)
    {
        if (element == null || element.Name != "dynamicwall")
            return null;

        DynamicWall retVal = null;

        switch (element.GetAttribute("type"))
        {
            case "rotatingwall":
                retVal = createRotatingWall(element);
                break;
            case "slidingwall":
                retVal = createSlidingWall(element);
                break;
        }

        return retVal;
    }
	#endregion
	
	#region Instance Methods
    private DynamicWall createRotatingWall(XmlElement element)
    {
        GameObject go = Resources.Load<GameObject>("Prefabs/Activatables/Walls/Rotating Wall");
        if (go == null)
            return null;

        go = GameObject.Instantiate(go) as GameObject;
        RotatingWall retVal = go.GetComponent<RotatingWall>();

        retVal.rotationPoint = new GameObject();
        retVal.rotationPoint.transform.position = 
            Utilities.StringToVector3(element.GetAttribute("rotationpoint"));

        XmlNode child = element.FirstChild;
        XmlElement component;

        List<RotatingWall.DIRECTION> targets = new List<RotatingWall.DIRECTION>();
        while (child != null)
        {
            component = child as XmlElement;
            child = child.NextSibling;

            if (component == null)
                continue;

            if (component.Name == "target")
                targets.Add(Utilities.ParseEnum<RotatingWall.DIRECTION>(component.GetAttribute("rotation")));
        }
        retVal.rotationTargets = targets.ToArray();

        deserializeComponents(go, element);

        return retVal;
    }

    private DynamicWall createSlidingWall(XmlElement element)
    {
        GameObject go = Resources.Load<GameObject>("Prefabs/Activatables/Walls/Sliding Wall");
        if (go == null)
            return null;

        go = GameObject.Instantiate(go) as GameObject;
        SlidingWall retVal = go.GetComponent<SlidingWall>();

        XmlNode child = element.FirstChild;
        XmlElement component;

        List<GameObject> targets = new List<GameObject>();
        GameObject target;
        while (child != null)
        {
            component = child as XmlElement;
            child = child.NextSibling;

            if (component == null)
                continue;

            if (component.Name == "target")
            {
                target = new GameObject();
                target.transform.position = Utilities.StringToVector3(component.GetAttribute("position"));
                targets.Add(target);
            }
        }
        retVal.targets = targets.ToArray();

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
                    case "boxcollider2d":
                        BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
                        ComponentSerializer.deserialize(collider, component);
                        break;
                }
            }

            child = child.NextSibling;
        }
    }
	#endregion
}
