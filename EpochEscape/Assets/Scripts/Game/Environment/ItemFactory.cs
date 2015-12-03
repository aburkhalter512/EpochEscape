using UnityEngine;
using System.Xml;
using System;
using System.Collections.Generic;
using Utilities;

namespace Game
{
    public class ItemFactory : IFactory<MonoBehaviour>
    {
        #region Instance Variables
        private GameObject mPowerCore1Prefab;
        private GameObject mPowerCore2Prefab;
        private GameObject mPowerCore3Prefab;
        #endregion

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
                    if (mPowerCore1Prefab == null)
                        mPowerCore1Prefab = Resources.Load<GameObject>("Prefabs/Game Environment/Items/PowerCore1");

                    go = mPowerCore1Prefab;
                    break;
                case 2:
                    if (mPowerCore2Prefab == null)
                        mPowerCore2Prefab = Resources.Load<GameObject>("Prefabs/Game Environment/Items/PowerCore2");

                    go = mPowerCore2Prefab;
                    break;
                case 3:
                    if (mPowerCore3Prefab == null)
                        mPowerCore3Prefab = Resources.Load<GameObject>("Prefabs/Game Environment/Items/PowerCore3");

                    go = mPowerCore3Prefab;
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
                            Serialization.deserialize(go.transform, component);
                            break;
                    }
                }

                child = child.NextSibling;
            }
        }
        #endregion
    }
}
