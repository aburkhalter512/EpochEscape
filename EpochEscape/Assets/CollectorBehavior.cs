using UnityEngine;
using System.Collections;

public class CollectorBehavior : LaserBehavior {
	public CollectorColors bottom;
	public CollectorColors left;
	public CollectorColors right;

	public bool b = false;
	public bool l = false;
	public bool r = false;

	public Color combinedColor = Color.black;

	// Use this for initialization
	void Start () {
	}
	void Update(){
		CollectBottom ();
		CollectLeft ();
		CollectRight ();
	}
	// Update is called once per frame
	void FixedUpdate () {
		if (b || l || r) {
			base.SetColor (combinedColor);
			base.bounces = 0;
			base.BuildLaser(start.position);
			base.DrawLaser ();
			base.positions.Clear ();
		}
		else{
			base.SetColor (combinedColor);
		}
		combinedColor = Color.black;
	}

	public void CollectBottom(){
		if (b) {
			combinedColor += bottom.color;
		}
		bottom.color = Color.black;
		b = false;
	}

	public void CollectLeft(){
		if (l) {
			combinedColor += left.color;
		}
		left.color = Color.black;
		l = false;
	}

	public void CollectRight(){
		if (r) {
			combinedColor += right.color;
		}
		right.color = Color.black;
		r = false;
	}
}
