using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace MapEditor
{
    public class DynamicWallManager : ISerializable
    {
        #region Instance Variables
        private static DynamicWallManager mInstance;

        private SortedList<string, PlaceableDynamicWall> mDynWall;

        private readonly int PROCESS_COUNT = 80;
        #endregion

        private DynamicWallManager()
        {
            mDynWall = new SortedList<string, PlaceableDynamicWall>(Utilities.StringComparer.Get());
        }

        #region Interface Methods
        public static DynamicWallManager Get()
        {
            if (mInstance == null)
                mInstance = new DynamicWallManager();

            return mInstance;
        }

        public void register(PlaceableDynamicWall activator)
        {
            mDynWall.Add(activator.getID(), activator);
        }
        public void unregister(string id)
        {
            mDynWall.Remove(id);
        }

        public IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
        {
            if (callback == null)
                yield break;

            XmlElement activators = doc.CreateElement("dynamicwalls");

            int count = 0;
            foreach (PlaceableDynamicWall activator in mDynWall.Values)
            {
                activator.serialize(doc, (XmlElement elem) =>
                {
                    activators.AppendChild(elem);
                });

                if (++count == PROCESS_COUNT)
                {
                    yield return null;
                    count = 0;
                }
            }

            callback(activators);
        }
        #endregion
    }
}
