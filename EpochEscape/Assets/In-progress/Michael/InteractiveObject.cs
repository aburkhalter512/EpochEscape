using UnityEngine;
using System.Collections;

public abstract class InteractiveObject : MonoBehaviour {

	// Use this for initialization
	protected virtual void Awake ()
    {
		gameObject.tag = "InteractiveObject";
	}

	public abstract void Interact();	
}
