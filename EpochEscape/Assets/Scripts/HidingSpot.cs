using UnityEngine;
using System.Collections;

public class HidingSpot : MonoBehaviour
{
	private Player m_player;

	public void Start()
	{
		m_player = null;
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

    public void OnTriggerStay2D(Collider2D collidee)
    {
        if (collidee.tag == "Player")
        {
            if (m_player != null)
                m_player.m_isHiding = m_player.IsActive() ? false : true;
        }
    }

	public void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			if(m_player != null)
				m_player.m_isHiding = false;
		}
	}
}
