using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class TeleportArea : MonoBehaviour {

	public TeleportArea destination;
	public bool isVertical;
	public float exitOffset = 0.2f;
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
			float top = renderer.bounds.max.y;
			
			Vector3 playerPos = other.transform.position;
			
			if (isVertical) {
				if (playerPos.x <= left) {
					other.transform.position = destination.exitZone2;
				} else {
					other.transform.position = destination.exitZone1;
				}
			} else {
				if (playerPos.y >= top) {
					other.transform.position = destination.exitZone2;
				} else {
					other.transform.position = destination.exitZone1;
				}
			}
		}
	}
}
