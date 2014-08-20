using UnityEngine;
using System.Collections;

public class WallSwitch : InteractiveObject {
	
	public GameObject[] actuators;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public override void Interact() {
		foreach (GameObject actuator in actuators)
		{
			Debug.Log("Action performed!");
			actuator.SendMessage("Toggle");
		}
	}
}
