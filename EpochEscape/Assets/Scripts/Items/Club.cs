using UnityEngine;
using System.Collections;

public class Club : Item {

	// Use this for initialization
	void Start () {
		gameObject.tag = "Special Item";
	}

	public override void PickUp(Player player)
	{
		/*PickUpSound.Play ();
		player.m_hasSpecialItem = true;
		player.m_specialItemCollectTime = Time.time;
		player.m_playSpecialItemAnim = true;*/
	}

	public override void Activate(){
		//ActivateSound.Play ();
		//Player player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
	}
}
