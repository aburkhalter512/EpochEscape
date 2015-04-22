using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class PlaceableRotatingWall : PlaceableDynamicWall
{
	#region Interface Variables
	#endregion
	
	#region Instance Variables
	#endregion
	
	#region Interface Methods
    public override IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
    {
        base.serialize(doc, (XmlElement elem) =>
            {
                elem.SetAttribute("rotationpoint", mBasePos.ToString());

                callback(elem);
            });

        return null;
    }
	#endregion
	
	#region Instance Methods
    protected override string getType()
    {
        return "RotatingWall";
    }
	#endregion
}
