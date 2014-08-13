using UnityEngine;
using System.Collections;

public class HoldableObject : InteractiveObject {
	private enum direction {
		Left, Right, Up, Down, Static
	};

	public float k_speed = 0.05f;
	private bool m_isInUse = false;
	private direction m_direction;
	private Player p;
	
	public void Start () {
		m_direction = direction.Static;
		GameObject g = GameObject.FindGameObjectWithTag("Player");
		p = g.GetComponent<Player>();
	}

	public void Update () {

	}

	public override void Interact() {
		if (!m_isInUse && !p.m_isHoldingBox) { 
			Renderer r  = gameObject.GetComponent<Renderer>();
			r.enabled = false;
			Collider2D c = gameObject.GetComponent<Collider2D>();
			c.enabled = false;
			m_isInUse = true;
			p.m_isHoldingBox = true;
		}
	}
	
	public void Place() { //places the box in front of the player
			
	}
}
