using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class PlaceableSlidingWall : PlaceableDynamicWall
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

            callback(elem);
        });

        return null;
    }
	#endregion

    #region Instance Methods
    protected override string getType()
    {
        return "SlidingWall";
    }

    protected override void selectUpdate()
    {
        base.selectUpdate();
    }
	#endregion
}
