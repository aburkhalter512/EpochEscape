using UnityEngine;
using System.Collections;

public class SmokeBomb : Item {

	// Use this for initialization
	void Start () {
		gameObject.tag = "Special Item";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public override void PickUp(Player player){
		
	}
	public override void Activate(){
		ActivateSound.Play ();
	}
}
