using UnityEngine;
using System.Xml;
using System;
using System.Collections.Generic;
using Utilities;

namespace Game
{
    public class DynamicWallFactory : IFactory<DynamicWall>
    {
        #region Instance Variables
        private List<Utilities.Pair<TeleporterDoorFrame, string>> mToConnect;

        private UnityEngine.Object mSlidingWallPrefab;
        private UnityEngine.Object mRotatingWallPrefab;
        #endregion

        #region Interface Methods
        public override DynamicWall create(XmlElement element)
        {
            if (element == null || element.Name != "dynamicwall")
                return null;

            DynamicWall retVal = null;

            switch (element.GetAttribute("type"))
            {
                case "RotatingWall":
                    retVal = createRotatingWall(element);
                    break;
                case "SlidingWall":
                    retVal = createSlidingWall(element);
                    break;
            }

            return retVal;
        }
        #endregion

        #region Instance Methods
        private DynamicWall createRotatingWall(XmlElement element)
        {
            if (mRotatingWallPrefab == null)
                mRotatingWallPrefab = Resources.Load<GameObject>("Prefabs/Activatables/Walls/RotatingWall");

            if (mRotatingWallPrefab == null)
                return null;

            GameObject go = GameObject.Instantiate(mRotatingWallPrefab) as GameObject;
            RotatingWall retVal = go.GetComponent<RotatingWall>();

            retVal.setID(element.GetAttribute("id"));

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
                    targets.Add(Serialization.ParseEnum<RotatingWall.DIRECTION>(component.GetAttribute("rotation")));
            }
            retVal.setRotationTargets(targets.ToArray());

            deserializeComponents(go, element);
            // Rotation point is set after the transform has been set
            retVal.setRotationPoint(
                Serialization.toVector3(element.GetAttribute("rotationpoint")));
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = DynamicWall.createWallSprite(
                Convert.ToInt32(element.GetAttribute("east")),
                Convert.ToInt32(element.GetAttribute("north")),
                Convert.ToInt32(element.GetAttribute("west")),
                Convert.ToInt32(element.GetAttribute("south")));

            return retVal;
        }

        private DynamicWall createSlidingWall(XmlElement element)
        {
            if (mSlidingWallPrefab == null)
                mSlidingWallPrefab = Resources.Load<GameObject>("Prefabs/Activatables/Walls/SlidingWall");

            if (mSlidingWallPrefab == null)
                return null;

            GameObject go = GameObject.Instantiate(mSlidingWallPrefab) as GameObject;
            SlidingWall retVal = go.GetComponent<SlidingWall>();

            XmlNode child = element.FirstChild;
            XmlElement component;

            List<Vector3> targets = new List<Vector3>();
            Vector3 target;
            while (child != null)
            {
                component = child as XmlElement;
                child = child.NextSibling;

                if (component == null)
                    continue;

                if (component.Name == "target")
                {
                    target = Serialization.toVector3(component.GetAttribute("position"));
                    targets.Add(target);
                }
            }

            retVal.setID(element.GetAttribute("id"));
            deserializeComponents(go, element);

            // Targets are set after the transform has been set.
            retVal.setSlidingTargets(targets.ToArray());
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = DynamicWall.createWallSprite(
                Convert.ToInt32(element.GetAttribute("east")),
                Convert.ToInt32(element.GetAttribute("north")),
                Convert.ToInt32(element.GetAttribute("west")),
                Convert.ToInt32(element.GetAttribute("south")));

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
                        case "spriterenderer":
                            Serialization.deserialize(go.GetComponent<SpriteRenderer>(), component);
                            break;
                        case "boxcollider2d":
                            BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
                            Serialization.deserialize(collider, component);
                            break;
                    }
                }

                child = child.NextSibling;
            }
        }
        #endregion
    }
}
