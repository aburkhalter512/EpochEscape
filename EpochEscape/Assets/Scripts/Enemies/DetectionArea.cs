using UnityEngine;
using System.Collections;

public class DetectionArea : MonoBehaviour
{
    public Player detectedPlayer = null;

	private GameObject m_parent;
	private Player m_player;

    public void Start()
    {
        detectedPlayer = null;

		if(transform.parent != null)
			m_parent = transform.parent.gameObject;
    }

	public void Update()
	{
		if(m_player == null)
			CheckForPlayer();
	}

	private void CheckForPlayer()
	{
		if(m_player != null) return;

		GameObject player = GameObject.FindWithTag("Player");

		if(player != null)
			m_player = player.GetComponent<Player>();
	}

	public void OnTriggerStay2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{	
			if(!(m_parent == null || m_player == null))
			{
				detectedPlayer = m_player;

                if(isPlayerInBounds(other))
                {
					m_player.m_isDetected = true;

					if(m_parent != null)
                    {
						if(m_parent.tag == "Guard")
                        {
                            // Do something with the guard.
                        }
						else if(m_parent.tag == "SecurityCamera")
                        {
							SecurityCamera securityCamera = m_parent.GetComponent<SecurityCamera>();

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
					m_player.m_isDetected = false;
			}
		}
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{
			if(m_player != null)
			{
				m_player.m_isDetected = false;

				if(m_parent != null)
				{
					if(m_parent.tag == "Guard")
					{
						// Do something with the guard.
					}
					else if(m_parent.tag == "SecurityCamera")
					{
						SecurityCamera securityCamera = m_parent.GetComponent<SecurityCamera>();

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
			if(m_parent != null)
			{
				float angleBound = Mathf.Atan(((m_parent.transform.localScale.x * transform.localScale.x) / 2f) / 
					(m_parent.transform.localScale.y * transform.localScale.y)) * Mathf.Rad2Deg;
				
				Vector3 toPlayer = other.transform.position - m_parent.transform.position;
				toPlayer.Normalize();
				
				float toPlayerAngle = Mathf.Acos(Vector3.Dot(toPlayer, m_parent.transform.up)) * Mathf.Rad2Deg;
				
				if(toPlayerAngle < angleBound)
					return true;
			}
		}

		return false;
	}
}
