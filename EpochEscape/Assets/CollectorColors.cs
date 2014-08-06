using UnityEngine;
using System.Collections;

public class CollectorColors : MonoBehaviour {
	public Color color;
	public CollectorBehavior collector;
	public Side side;

	public enum Side{
		Bottom, Left, Right
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Collect(){
		switch (side) {
		case Side.Bottom:
			collector.b = true;
			break;
		case Side.Left:
			collector.l = true;
			break;
		case Side.Right:
			collector.r = true;
			break;
		default:
			break;
		}
	}
}
