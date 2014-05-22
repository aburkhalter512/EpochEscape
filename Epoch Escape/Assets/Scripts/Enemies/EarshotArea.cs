using UnityEngine;
using System.Collections;

public class EarshotArea : MonoBehaviour
{
	public void OnTriggerStay2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{
			Player player = other.GetComponent<Player>();
			Guard guard = transform.parent.gameObject.GetComponent<Guard>();
			
			if(player != null && guard != null)
			{
				player.m_isWithinEarshot = true;

				if(guard.m_currentState != Guard.State.STUN)
					guard.m_currentState = Guard.State.ALERT;
			}
		}
	}
	
	public void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{
			Player player = other.GetComponent<Player>();
			Guard guard = transform.parent.gameObject.GetComponent<Guard>();
			
			if(player != null && guard != null)
				player.m_isWithinEarshot = false;
		}
	}
}
