using UnityEngine;
using System.Collections;

public class EarshotArea : MonoBehaviour
{
	private Player m_player;
	private Guard m_parent;

	public void Start()
	{
		m_player = null;
		m_parent = null;

		if(transform.parent != null)
		{
			if(transform.parent.tag == "Guard")
				m_parent = transform.parent.gameObject.GetComponent<Guard>();
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

	public void OnTriggerStay2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			if(!(m_player == null || m_parent == null) && !m_player.m_isHiding)
			{
				m_player.m_isWithinEarshot = true;

				if(m_parent.m_currentState != Guard.State.STUN)
					m_parent.m_currentState = Guard.State.ALERT;
			}
		}
	}
	
	public void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			if(!(m_player == null || m_parent == null))
				m_player.m_isWithinEarshot = false;
		}
	}
}
