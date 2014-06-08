﻿using UnityEngine;
using System.Collections;

public class Club : Item {

	// Use this for initialization
	void Start () {
		gameObject.tag = "Special Item";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void PickUp(Player player)
	{
		PickUpSound.Play ();
		player.m_hasSpecialItem = true;
	}

	public override void Activate(){
		//ActivateSound.Play ();
		//Player player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
	}
}
