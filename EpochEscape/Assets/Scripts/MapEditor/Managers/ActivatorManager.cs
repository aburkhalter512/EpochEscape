using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Utilities;

namespace MapEditor
{
    public class ActivatorManager : ISerializable
    {
        #region Instance Variables
        private static ActivatorManager mInstance;

        private SortedList<string, PlaceableActivator> mActivators;

        private readonly int PROCESS_COUNT = 80;
        #endregion

        private ActivatorManager()
        {
            mActivators = new SortedList<string, PlaceableActivator>(Utilities.StringComparer.Get());
        }

        #region Interface Methods
        public static ActivatorManager Get()
        {
            if (mInstance == null)
                mInstance = new ActivatorManager();

            return mInstance;
        }

        public void register(PlaceableActivator activator)
        {
            mActivators.Add(activator.getID(), activator);
        }
        public void unregister(string id)
        {
            mActivators.Remove(id);
        }

        public IEnumerator serialize(XmlDocument doc, Action<XmlElement> callback)
        {
            if (callback == null)
                yield break;

            XmlElement activators = doc.CreateElement("activators");

            int count = 0;
            foreach (PlaceableActivator activator in mActivators.Values)
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
