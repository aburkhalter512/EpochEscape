using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopupSwitch : MonoBehaviour {
	public List<string> instructions = new List<string>();
	public bool active = true;
	public bool delay = false;

	private void message () {
		GameManager.getInstance ().messages = instructions;
		GameManager.getInstance ().ShowPopupMessage();
		GameManager.getInstance ().popup = true;
		active = false;
	}

	void OnTriggerEnter2D(Collider2D other){
		if(active && other.gameObject.tag == "Player"){
			if (!delay) {
				Player p = other.gameObject.GetComponent<Player>();
				p.audio.Stop ();
				message ();
			} else {
				Player p = other.gameObject.GetComponent<Player>();
				p.audio.Stop ();
				Invoke ("message",1);
			}
		}
	}
}
