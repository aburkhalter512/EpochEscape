using UnityEngine;
using System.Collections;

public class PitFloor : InteractiveObject {
	public Sprite fill;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			Player p = other.gameObject.GetComponent<Player>();
			//p.m_currentState = Player.PlayerState.DEAD;
		}
	}
	
	public override void Interact() {
		GameObject g = GameObject.FindGameObjectWithTag("Player");
		Player p = g.GetComponent<Player>();
		if (p.m_isHoldingBox) {
			HoldableBox box = p.GetComponentInChildren<HoldableBox>();
			SpriteRenderer s = gameObject.GetComponent<SpriteRenderer>();
			s.sprite = fill;
			box.Die ();
			p.m_isHoldingBox = false;
			gameObject.GetComponent<Collider2D>().enabled = false;
		}
	}
}
