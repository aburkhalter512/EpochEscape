using UnityEngine;
using System.Collections;

public class GrateFloor : MonoBehaviour {
	
	public void OnTriggerStay2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			Player p = col.gameObject.GetComponent<Player>();
			p.m_floorType = 1;
		}
	}

	public void OnTriggerExit2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			Player p = col.gameObject.GetComponent<Player>();
			p.m_floorType = 0;
		}
	}
}
