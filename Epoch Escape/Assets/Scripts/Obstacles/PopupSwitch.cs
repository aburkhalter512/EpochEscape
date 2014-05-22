using UnityEngine;
using System.Collections;

public class PopupSwitch : MonoBehaviour {
	public string instructions = "";
	public bool active = true;

	void OnTriggerEnter2D(Collider2D other){
		if(active){
			if (other.gameObject.tag == "Player") {
				GameManager.getInstance ().message = instructions;
				GameManager.getInstance ().ShowPopupMessage();
				GameManager.getInstance ().popup = true;
				active = false;
			}
		}
	}
}
