using UnityEngine;
using System.Collections;

public class PopupSwitch : MonoBehaviour {
	public string instructions;
	public bool active = true;
	public bool delay = false;

	private void message () {
		GameManager.getInstance ().message = instructions;
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
