using UnityEngine;
using System.Collections;

public class CollectorBehavior : LaserBehavior {
	public EdgeCollider2D bottom;
	public EdgeCollider2D left;
	public EdgeCollider2D right;
	public bool hit;

	// Use this for initialization
	void Start () {
	}
	void Update(){
		if (hit) {
			BuildLaser (start.position);
			DrawLaser ();
			positions.Clear ();
		}
	}

	public void resetActivate (Color c){
		color -= c;

		if (color.r == 0 && color.g == 0 && color.b == 0) {
			resetLast (null);
			hit = false;
		}
	}

	public void Activate(Color c){
		color += c;
		hit = true;
	}



//	public void ResetCollecting(){
//		bottom.collecting = false;
//		left.collecting = false;
//		right.collecting = false;
//	}
}
