using UnityEngine;
using System.Collections;

public class CollectorBehavior : LaserBehavior {
	public CollectorColors bottom;
	public CollectorColors left;
	public CollectorColors right;

	public bool b = false;
	public bool l = false;
	public bool r = false;

	// Use this for initialization
	void Start () {

	}
	void Update(){
		//ResetCollecting();
		CollectBottom ();
		CollectLeft ();
		CollectRight ();
	}
	// Update is called once per frame
	void FixedUpdate () {
		base.SetColor (color);
		if (b || l || r) {
			base.bounces = 0;
			base.BuildLaser(start.position);
			base.DrawLaser ();
			base.positions.Clear ();
		}
		base.color = Color.black;
	}

	public void CollectBottom(){
		if (b) {
			base.color += bottom.color;
			//bottom.collecting = true;
		}
		bottom.color = Color.black;
		b = false;
	}

	public void CollectLeft(){
		if (l) {
			base.color += left.color;
			//left.collecting = true;
		}
		left.color = Color.black;
		l = false;
	}

	public void CollectRight(){
		if (r) {
			base.color += right.color;
			//right.collecting = true;
		}
		right.color = Color.black;
		r = false;
	}

//	public void ResetCollecting(){
//		bottom.collecting = false;
//		left.collecting = false;
//		right.collecting = false;
//	}
}
