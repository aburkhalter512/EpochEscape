using UnityEngine;
using System.Collections;

public class DetectionArea : MonoBehaviour
{
    public Player detectedPlayer = null;

    public void Start()
    {
        detectedPlayer = null;
    }

	public void OnTriggerStay2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{
			GameObject parent = transform.parent.gameObject;
			Player player = other.GetComponent<Player>();
			
			if(parent != null && player != null)
			{
                detectedPlayer = player;

                if (isPlayerInBounds(other))
                {
                    player.m_isDetected = true;

                    if (parent != null)
                    {
                        if (parent.tag == "Guard")
                        {
                            // Do something with the guard.
                        }
                        else if (parent.tag == "SecurityCamera")
                        {
                            SecurityCamera securityCamera = parent.GetComponent<SecurityCamera>();

                            if (securityCamera != null)
                            {
                                /*
                                securityCamera.m_previousState = securityCamera.m_currentState;
                                securityCamera.m_currentState = SecurityCamera.State.ALERT;
                                */
                            }
                        }
                    }
                }
                else
                    player.m_isDetected = false;
			}
		}
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{
			GameObject parent = transform.parent.gameObject;
			Player player = other.GetComponent<Player>();
			
			if(player != null)
			{
				player.m_isDetected = false;

				if(parent != null)
				{
					if(parent.tag == "Guard")
					{
						// Do something with the guard.
					}
					else if(parent.tag == "SecurityCamera")
					{
						SecurityCamera securityCamera = parent.GetComponent<SecurityCamera>();

						if(securityCamera != null)
						{
							/*
							securityCamera.m_previousState = securityCamera.m_currentState;
							securityCamera.m_currentState = SecurityCamera.State.ROTATE_LEFT;
							*/
						}
					}
				}
			}
		}
	}

	public bool isPlayerInBounds(Collider2D other)
	{
		if(other.tag == "Player")
		{
			Transform parentTransform = transform.parent;

			if(parentTransform != null)
			{
				float angleBound = Mathf.Atan(((parentTransform.localScale.x * transform.localScale.x) / 2f) / 
					(parentTransform.localScale.y * transform.localScale.y)) * Mathf.Rad2Deg;
				
				Vector3 toPlayer = other.transform.position - parentTransform.position;
				toPlayer.Normalize();
				
				float toPlayerAngle = Mathf.Acos(Vector3.Dot(toPlayer, parentTransform.up)) * Mathf.Rad2Deg;
				
				if(toPlayerAngle < angleBound)
					return true;
			}
		}

		return false;
	}
}
