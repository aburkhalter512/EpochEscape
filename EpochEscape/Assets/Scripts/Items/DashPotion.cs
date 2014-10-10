using UnityEngine;
using System.Collections;

public class Dash : ActiveItem {
	public float SPEED = 1.5f;
	// Use this for initialization
	void Start () {
		gameObject.tag = "Dash Potion";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void PickUp(Player player){
		PickUpSound.Play ();
	}
	public override void Activate(){
		Player player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
		player.Boost = 1.5f;
		player.BoostTime = Time.deltaTime;
	}
}
