using UnityEngine;
using System.Collections;

public class DetectionArea : MonoBehaviour
{
	public Player detectedPlayer = null;
	
	private Player m_player;
	private GameObject m_parent;
	private SpriteRenderer m_parentRenderer;
	
	public bool audioIsPlaying = false;
	
	public static Color BLUE = new Color(0.33f, 0.5f, 0.78f, 0.33f);
	public static Color RED = new Color(1f, 0f, 0.12f, 0.45f);
	
	public enum ColorStatus
	{
		NORMAL,
		CAUTION
	};
	
	public void Start()
	{
		detectedPlayer = null;
		
		m_player = null;
		m_parent = null;
		
		if(transform.parent != null)
		{
			m_parent = transform.parent.gameObject;
			m_parentRenderer = GetComponent<SpriteRenderer>();
			
			if(m_parentRenderer != null)
				m_parentRenderer.color = BLUE;
		}
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
	
	private void ChangeColor(ColorStatus colorStatus)
	{
		int parentNumChildren = m_parent.transform.childCount;
		
		if(parentNumChildren == 0) return;
		
		Transform currentChild = null;
		SpriteRenderer currentRenderer = null;
		
		for(int i = 0; i < parentNumChildren; i++)
		{
			currentChild = m_parent.transform.GetChild(i);
			currentRenderer = currentChild.GetComponent<SpriteRenderer>();
			
			if(currentRenderer != null)
			{
				if(colorStatus == ColorStatus.NORMAL)
					currentRenderer.color = BLUE;
				else
					currentRenderer.color = RED;
			}
		}
	}
	
	public void OnTriggerStay2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{	
			if(!(m_parent == null || m_player == null))
			{
				detectedPlayer = m_player;
				
				if(!m_player.m_isHiding)
				{
					if(!(m_parentRenderer == null || m_player.m_isDetected))
						ChangeColor(ColorStatus.CAUTION);
					
					m_player.m_isDetected = true;
					
					if(m_parent != null)
					{
						if(m_parent.tag == "Guard")
						{
							// Do something with the guard.
						}
						else if(m_parent.tag == "SecurityCamera")
						{
							if (!audioIsPlaying) {
								audio.Play ();
								audioIsPlaying = true;
							}
							
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
				{
					m_player.m_isDetected = false;

					ChangeColor(ColorStatus.NORMAL);
				}
			}
		}
	}
	
	public void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			if(m_player != null)
			{
				ChangeColor(ColorStatus.NORMAL);
				
				m_player.m_isDetected = false;
				
				if(m_parent != null)
				{
					if(m_parent.tag == "Guard")
					{
						// Do something with the guard.
					}
					else if(m_parent.tag == "SecurityCamera")
					{
						audio.Stop();
						audioIsPlaying = false;
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
}
