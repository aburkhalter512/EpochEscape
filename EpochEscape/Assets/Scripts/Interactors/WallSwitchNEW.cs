using UnityEngine;
using System.Collections.Generic;

public class WallSwitchNEW : InteractiveObject
{
    #region Interface Variables
    public GameObject[] actuators;
    #endregion

    #region InstanceVariables
    List<DynamicWall> wallActuators;
    //List<LockedDoor> doorActuators;
    #endregion

    // Use this for initialization
	void Start ()
    {
        MonoBehaviour script = null;

        wallActuators = new List<DynamicWall>();
        //doorActuators = new List<LockedDoor>();

        foreach (GameObject actuator in actuators)
        {
            #region Retrieving Dynamic Wall Actuators
            script = actuator.GetComponent<DynamicWall>();

            if (script != null)
                wallActuators.Add(script as DynamicWall);
            #endregion

            #region Retrieving Locked Door Actuators
            //script = actuator.GetComponent<LockedDoor>();
			/*
            if (script != null)
                doorActuators.Add(script as LockedDoor);*/
            #endregion
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public override void Interact()
    {
        foreach (DynamicWall actuator in wallActuators)
            actuator.currentState = DynamicWall.STATES.CHANGE;

		/*
        foreach (LockedDoor actuator in doorActuators)
            actuator.toggleLock();*/

		foreach (GameObject actuator in actuators)
		{
			Debug.Log("Action performed!");
			//actuator.SendMessage("Toggle");
		}
	}
}
