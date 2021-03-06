﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace MapEditor
{
    public class PlaceableSlidingWall : PlaceableDynamicWall
    {
        #region Interface Methods
        public static GameObject getPrefab()
        {
			return ResourceManager.ResourceManager.Get().prefab("Prefabs/MapEditor/Placeables/SlidingWall");
        }

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

        protected override GameObject loadPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/MapEditor/Placeables/SlidingWall");
        }
        #endregion
    }
}
