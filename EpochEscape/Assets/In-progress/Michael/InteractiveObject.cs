using UnityEngine;
using System.Collections;

public abstract class InteractiveObject : MonoBehaviour {

	// Use this for initialization
	protected void Start () {
		gameObject.tag = "InteractiveObject";
	}
	
	// Update is called once per frame
	protected void Update () {
	
	}

	public abstract void Interact();	
}
