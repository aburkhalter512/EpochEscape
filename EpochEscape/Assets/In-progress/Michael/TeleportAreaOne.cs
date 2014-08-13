using UnityEngine;
using System.Collections;

public class TeleportAreaOne : MonoBehaviour {

	public TeleportAreaOne destination;
	public bool isVertical;
	public bool topLeftActive; // true if the active side is either the top or the left;
	public float exitOffset = 0.02f;
	private Vector3 exitZone1; // left or top
	private Vector3 exitZone2; // right or bottom
	
	// Use this for initialization
	void Start () {
		Vector3 colliderExtents = GetComponent<BoxCollider2D>().size * 0.5f;
		
		if (isVertical) { // zone is aligned top to bottom
			exitZone1 = transform.position - new Vector3(colliderExtents.x,0,0);
			exitZone1.x = exitZone1.x - exitOffset;
			exitZone2 = transform.position + new Vector3(colliderExtents.x,0,0);
			exitZone2.x = exitZone2.x - exitOffset;
		} else { // zone is flat
			exitZone1 = transform.position + new Vector3(0,colliderExtents.y,0);
			exitZone1.y = exitZone1.y + exitOffset;
			exitZone2 = transform.position - new Vector3(0,colliderExtents.y,0);
			exitZone2.y = exitZone2.y - exitOffset;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	// Teleport the player when entered
	public void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			float left = renderer.bounds.min.x;
			float right = renderer.bounds.max.x;
			float top = renderer.bounds.max.y;
			float bottom = renderer.bounds.min.y;
			
			Vector3 playerPos = other.transform.position;
			Player p = other.GetComponent<Player>();
			
			if (isVertical) {
				if (topLeftActive) { // door is vertial and the left side is active
					if (p.m_isMovingRight) { // player can only teleport if moving down through zone
						other.transform.position = destination.exitZone2;
					}
				} else { // right side is active
					if (p.m_isMovingLeft) {
						other.transform.position = destination.exitZone1;
					}
				}
			} else { // door is horizontal
				if (topLeftActive) { // if top side is active
					if  (p.m_isMovingDown) {
						other.transform.position = destination.exitZone2;
					}
				} else { // bottom side is active
					if (p.m_isMovingForward) {
						other.transform.position = destination.exitZone1;
					}
				}
			}
		}
	}
	
	public void Toggle() {
		TeleportAreaOne script = GetComponent<TeleportAreaOne>();
		script.enabled = false;
		collider2D.enabled = false;
	}
}
