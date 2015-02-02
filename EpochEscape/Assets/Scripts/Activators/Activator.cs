using UnityEngine;
using System.Collections.Generic;

public abstract class Activator: MonoBehaviour
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
