using UnityEngine;
using System.Collections.Generic;

public class TerminalSwitch : InteractiveObject {
	
	public GameObject[] actuators;
    public GameObject[] lockedDoors;
    public bool interact = false;

    private List<LockedDoorFrame> mLockedDoors;

	// Use this for initialization
	void Start () {
        mLockedDoors = new List<LockedDoorFrame>();
        LockedDoorFrame temp = null;

        foreach (GameObject GO in lockedDoors)
        {
            temp = GO.GetComponent<LockedDoorFrame>();

            if (temp != null)
                mLockedDoors.Add(temp);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (interact == true)
        {
            Interact();
            interact = false;
        }
	}
	
	public override void Interact() {
		foreach (GameObject actuator in actuators)
		{
			Debug.Log("Action performed!");
			actuator.SendMessage("Activate");
		}

        foreach (LockedDoorFrame actuator in mLockedDoors)
            actuator.toggleLock();
	}
}
