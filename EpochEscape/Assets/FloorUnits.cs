using UnityEngine;
using System.Collections;

public class FloorUnits : MonoBehaviour
{
	public void Start()
	{
		SpriteRenderer sr = GetComponent<SpriteRenderer>();

		if(sr != null)
		{
			Debug.Log(sr.sprite.bounds.size.x);
		}
	}
}
