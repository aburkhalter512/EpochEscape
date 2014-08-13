using UnityEngine;
using System.Collections;

public class HoldableBox : InteractiveObject {
	protected enum direction {
		Left, Right, Up, Down, Static
	};

	//public float k_speed = 0.05f;
	protected bool m_isInUse = false;
	protected direction m_direction;
	protected Player p;
	
	public void Start () {
		m_direction = direction.Static;
		GameObject g = GameObject.FindGameObjectWithTag("Player");
		p = g.GetComponent<Player>();
	}

	public void Update () {

	}

	public override void Interact() {
		Debug.Log ("Box Interact");
		renderer.enabled = false;
		collider2D.enabled = false;
		m_isInUse = true;
		p.m_isHoldingBox = true;
		transform.parent = p.transform;
	}
	
	public void Place() { //places the box in front of the player
		
	}
	
	public void Die() {
		Destroy(gameObject);
	}
	
}
