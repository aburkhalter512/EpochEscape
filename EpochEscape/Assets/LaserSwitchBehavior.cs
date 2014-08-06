using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserSwitchBehavior : MonoBehaviour {
	public List<Color> colors;
	public Color colorMatch;
	// Use this for initialization
	void Start () {
		if(colors != null || colors.Count != 0){
			foreach(Color c in colors){
				colorMatch += c;
			}
		}
		gameObject.GetComponent<SpriteRenderer> ().color = colorMatch;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Activate(){
		Debug.Log ("Activating!");
	}
}
