using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectorBehavior : LaserBehavior
{
	public EdgeCollider2D bottom;
	public EdgeCollider2D left;
	public EdgeCollider2D right;
	public SpriteRenderer bottomSprite;
	public SpriteRenderer leftSprite;
	public SpriteRenderer rightSprite;
	public SpriteRenderer emitter;
	public SpriteRenderer middle;
	public List<Color> colors;
	public bool hit;

	// Use this for initialization
	void Start () {
		colors = new List<Color> ();
	}
	void Update(){
		if (hit) {
			BuildLaser (start.position);
			DrawLaser ();
			positions.Clear ();
		}
	}

	public void resetActivate (Color c)
    {
		/*color = Utilities.subtractColors (c, color);
		if (Utilities.areEqualColors (c, bottomSprite.color)) {
			bottomSprite.color = Color.white;
		}
		else if (Utilities.areEqualColors (c, leftSprite.color)){
			leftSprite.color = Color.white;
		}
		else if (Utilities.areEqualColors (c, rightSprite.color)) {
			rightSprite.color = Color.white;
		}

		foreach(Color co in colors){
			if(Utilities.areEqualColors(c, co)){
				colors.Remove (c);
				break;
			}
		}
		SetColor (combineColors ());
		if (colors.Count == 0) {
			resetLast(null);
			hit = false;
			middle.color = Color.white;
			emitter.color = Color.white;
		}
		else{
			middle.color = color;
			emitter.color = color;	
		}
		if(lastObject != null && lastObject.tag == "Laser Switch"){
			LaserSwitchBehavior l = lastObject.GetComponent<LaserSwitchBehavior>();
			l.resetActivate ();
			l.Activate (color);
		}*/
	}

	public void Activate(Color c, EdgeCollider2D side){
		colors.Add (c);
		hit = true;
		if (side.Equals(bottom)) {
			bottomSprite.color = c;
		}
		else if(side.Equals(left)){
			leftSprite.color = c;
		}
		else if(side.Equals(right)){
			rightSprite.color = c;
		}
		SetColor (combineColors ());
		middle.color = color;
		emitter.color = color;
		if(lastObject != null && lastObject.tag == "Laser Switch"){
			LaserSwitchBehavior l = lastObject.GetComponent<LaserSwitchBehavior>();
			l.resetActivate ();
			l.Activate (color);
		}
	}

	private Color combineColors()
    {
		Color combined = new Color(0,0,0,255);
		/*foreach (Color c in colors)
			combined = Utilities.addColors (combined, c);*/

		return combined;
	}
}
