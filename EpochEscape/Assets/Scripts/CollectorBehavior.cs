using UnityEngine;
using System.Collections;

public class CollectorBehavior : LaserBehavior {
	public EdgeCollider2D bottom;
	public EdgeCollider2D left;
	public EdgeCollider2D right;
	public SpriteRenderer bottomSprite;
	public SpriteRenderer leftSprite;
	public SpriteRenderer rightSprite;
	public SpriteRenderer emitter;
	public SpriteRenderer middle;
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
		if (c == bottomSprite.color) {
			bottomSprite.color = Color.white;
		}
		else if (c == leftSprite.color) {
			leftSprite.color = Color.white;
		}
		else if (c == rightSprite.color) {
			rightSprite.color = Color.white;
		}

		if (color.r == 0 && color.g == 0 && color.b == 0) {
			resetLast(null);
			hit = false;
			middle.color = Color.white;
			emitter.color = Color.white;
		}
		else{
			middle.color = color;
			emitter.color = color;	
		}
	}

	public void Activate(Color c, EdgeCollider2D side){
		color += c;
		hit = true;
		if (side == bottom) {
			bottomSprite.color = c;
		}
		else if(side == left){
			leftSprite.color = c;
		}
		else if(side == right){
			rightSprite.color = c;
		}
		middle.color = color;
		emitter.color = color;
	}
}
