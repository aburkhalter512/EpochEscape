using UnityEngine;
using System.Collections;

public class TestCam : MonoBehaviour {
	private Player p;

	// Use this for initialization
	void Start () {
		GameObject g = GameObject.FindGameObjectWithTag ("Player");
		p = g.GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (p.transform.position.x, p.transform.position.y, -10f);
	}
}
