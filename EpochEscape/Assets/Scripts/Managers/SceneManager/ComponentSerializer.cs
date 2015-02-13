using UnityEngine;
using UnityEditor;
using System;
using System.Xml;

public class ComponentSerializer
{	
	#region Interface Methods
    public static XmlElement toXML(Transform transform, XmlDocument document)
    {
        XmlElement element = document.CreateElement("transform");
        element.SetAttribute("position", transform.position.ToString());
        element.SetAttribute("rotation", transform.eulerAngles.ToString());
        element.SetAttribute("localscale", transform.localScale.ToString());

        return element;
    }
    public static void deserialize(Transform transform, XmlElement element)
    {
        if (element.Name != "transform" || transform == null)
            return;

        transform.position = Utilities.StringToVector3(element.GetAttribute("position"));
        transform.eulerAngles = Utilities.StringToVector3(element.GetAttribute("rotation"));
        transform.localScale = Utilities.StringToVector3(element.GetAttribute("localscale"));
    }

    public static XmlElement toXML(SpriteRenderer sr, XmlDocument document)
    {
        XmlElement element = document.CreateElement("spriterenderer");
        element.SetAttribute("sprite", AssetDatabase.GetAssetPath(sr.sprite).Remove(0, 17)); //Removes Asset/Resources/
        element.SetAttribute("sortingOrder", sr.sortingOrder.ToString());

        return element;
    }
    public static void deserialize(SpriteRenderer sr, XmlElement element)
    {
        if (element.Name != "spriterenderer" || sr == null)
            return;

        sr.sprite = Resources.Load<Sprite>(element.GetAttribute("sprite")); 
        sr.sortingOrder = Convert.ToInt32(element.GetAttribute("sortingOrder"));
    }

    public static XmlElement toXML(BoxCollider2D collider, XmlDocument document)
    {
        XmlElement element = document.CreateElement("boxcollider2d");
        element.SetAttribute("size", collider.size.ToString());
        element.SetAttribute("center", collider.center.ToString());

        return element;
    }
    public static void deserialize(BoxCollider2D collider, XmlElement element)
    {
        if (element.Name != "boxcollider2d" || collider == null)
            return;

        collider.size = Utilities.StringToVector2(element.GetAttribute("size"));
        collider.center = Utilities.StringToVector2(element.GetAttribute("center"));
    }
	#endregion
}
