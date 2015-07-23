using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Utilities;

namespace Game
{
    public abstract class Activator : MonoBehaviour, ISerializable
    {
        #region Interface Variables
        public GameObject[] activatables;
        #endregion

        #region Instance Variables
        protected List<IToggleable> mActivatables;
        #endregion

        #region Interface Methods
        public void trigger()
        {
            StartCoroutine(triggerCoroutine());
        }

        public virtual void addActivatable(IToggleable activatable)
        {
            if (activatable == null)
                return;

            if (mActivatables == null)
                mActivatables = new List<IToggleable>();

            mActivatables.Add(activatable);
        }

        public virtual IEnumerator serialize(XmlDocument document, System.Action<XmlElement> callback)
        {
            XmlElement actuatorTag = document.CreateElement("activator");
            actuatorTag.SetAttribute("type", GetType().ToString());

            actuatorTag.AppendChild(Serialization.toXML(transform, document));

            XmlElement child;
            foreach (IActivatable actuator in mActivatables)
            {
                IIdentifiable identifiable = actuator as IIdentifiable;

                if (identifiable == null)
                    continue;

                child = document.CreateElement("activatable");
                child.SetAttribute("id", identifiable.getID());

                actuatorTag.AppendChild(child);
            }

            callback(actuatorTag);

            return null;
        }
        #endregion

        #region Instance Methods
        protected virtual void populateActivatables()
        {
            if (mActivatables == null)
                mActivatables = new List<IToggleable>();

            foreach (GameObject activatable in activatables)
            {
                if (activatable == null)
                    continue;

                MonoBehaviour[] actuatorComponents = activatable.GetComponents<MonoBehaviour>();

                foreach (MonoBehaviour actuator in actuatorComponents)
                {
                    if ((actuator as IToggleable) != null)
                        mActivatables.Add(actuator as IToggleable);
                }
            }
        }

        private IEnumerator triggerCoroutine()
        {
            foreach (IToggleable actuator in mActivatables)
            {
                if (actuator != null)
                    actuator.toggle();

                yield return null;
            }
        }
        #endregion
    }
}
