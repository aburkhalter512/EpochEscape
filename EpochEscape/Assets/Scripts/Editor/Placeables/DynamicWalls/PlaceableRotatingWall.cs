using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Editor
{
    public class PlaceableRotatingWall : PlaceableDynamicWall
    {
        #region Interface Methods
        public static GameObject getPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/RotatingWall");
        }

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

        protected override GameObject loadPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/RotatingWall");
        }
        #endregion
    }
}
