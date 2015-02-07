using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public abstract class Activator : MonoBehaviour, ISerializable
{
	#region Interface Variables
    public GameObject[] activatables;
	#endregion
	
	#region Instance Variables
    protected List<IActivatable> mActivatables;
	#endregion
	
	#region Interface Methods
    public void trigger()
    {
        foreach (IActivatable actuator in mActivatables)
        {
            if (actuator != null)
                actuator.toggle();
        }
    }

    public virtual void addActivatable(IActivatable activatable)
    {
        if (activatable == null)
            return;

        if (mActivatables == null)
            mActivatables = new List<IActivatable>();

        mActivatables.Add(activatable);
    }

    public virtual XmlElement Serialize(XmlDocument document)
    {
        XmlElement actuatorTag = document.CreateElement("activator");
        actuatorTag.SetAttribute("type", GetType().ToString());

        actuatorTag.AppendChild(ComponentSerializer.toXML(transform, document));

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

        return actuatorTag;
    }
	#endregion
	
	#region Instance Methods
    protected virtual void populateActivatables()
    {
        if (mActivatables == null)
            mActivatables = new List<IActivatable>();
        else
            mActivatables.Clear();

        foreach (GameObject activatable in activatables)
        {
            if (activatable == null)
                continue;

            MonoBehaviour[] actuatorComponents = activatable.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour actuator in actuatorComponents)
            {
                if ((actuator as IActivatable) != null)
                    mActivatables.Add(actuator as IActivatable);
            }
        }
    }
	#endregion
}
