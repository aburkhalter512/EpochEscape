using UnityEngine;
using System.Collections;

public class Cowbell : MonoBehaviour
{
	public void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			CameraBehavior cameraBehavior = Camera.main.GetComponent<CameraBehavior>();

			if(cameraBehavior != null)
				cameraBehavior.m_displayCowbell = true;

			Destroy(gameObject);
		}
	}
}
