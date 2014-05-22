using UnityEngine;
using System.Collections;

public class PopupSwitchNoColl : MonoBehaviour {
	public float wait = 0f;
	public string instructions = "";
	private float startTime;
	private bool active = true;
	// Use this for initialization
	void Start () {
		startTime = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {
		if(active){
			if ((Time.realtimeSinceStartup - startTime) > wait) {
				GameManager.getInstance ().message = instructions;
				GameManager.getInstance ().ShowPopupMessage();
				GameManager.getInstance ().popup = true;
				active = false;
			}
		}
	}
}
