using UnityEngine;
using System.Collections;

public class GrabbableObject : InteractiveObject {
	private enum direction {
		Left, Right, Up, Down, Static
	};

	public float k_speed = 0.05f;
	private bool m_isInUse = false;
	private direction m_direction;
	
	void Start () {
		m_direction = direction.Static;
	}

	void Update () {

	}

	public override void Interact() {
		if (!m_isInUse) {
			float left = renderer.bounds.min.x;
			float right = renderer.bounds.max.x;
			float top = renderer.bounds.max.y;
			float bottom = renderer.bounds.min.y;
			
			GameObject g = GameObject.FindGameObjectWithTag("Player");
			Vector3 playerPos = g.transform.position;
			
			if (playerPos.x <= left && playerPos.y < top && playerPos.y > bottom) { // player is to the left of box
				Debug.Log ("Left");
			} else if (playerPos.x >= right && playerPos.y < top && playerPos.y > bottom) { //player is to the right
				Debug.Log ("Right");
			} else if (playerPos.y >= top && playerPos.x > left && playerPos.x < right) { //player is above
				Debug.Log ("Top");
			} else {
				Debug.Log ("Bottom");
			}
			m_isInUse = true;
		} else {
			m_isInUse = false;
		}
	}
}
