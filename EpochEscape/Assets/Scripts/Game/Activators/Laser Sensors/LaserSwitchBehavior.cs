using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
    public class LaserSwitchBehavior : MonoBehaviour
    {
        #region Interface Variables
        public GameObject[] activatables;

        public List<DoorFrame> door;
        public List<bool> toActivate;
        public List<Color> colorMatch;
        public Color toMatch;
        #endregion

        #region Instance Variables
        public List<IActivatable> mActivatables;
        #endregion

        void Start()
        {
            /*foreach (Color c in colorMatch)
                toMatch = Utilities.addColors (toMatch, c);

            mActivatables = new List<IActivatable>();
            foreach (GameObject activatable in activatables)
            {
                IActivatable actuator = activatable.GetComponent<MonoBehaviour>() as IActivatable;

                if (actuator != null)
                    mActivatables.Add(actuator);
            }*/
        }

        public void resetActivate()
        {
            //loop the doors that this switch is responsible for
            for (int i = 0; i < door.Count; i++)
            {
                //check if activating or deactivating
                if (toActivate[i])
                    door[i].deactivate();
                else
                    door[i].activate();
            }
        }

        public void Activate(Color color)
        {
            /*//loop through doors this switch is responsible for
            if (Utilities.areEqualColors (color, toMatch)) {
                for (int i = 0; i < door.Count; i++) {
                    //check if activating or deactivating
                    if (toActivate[i])
                        door[i].deactivate();
                    else
                        door[i].activate();
                }
            }*/
        }
    }
}
	