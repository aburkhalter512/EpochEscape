using UnityEngine;
using System.Collections;

public class PowerCore : MonoBehaviour
{
	#region Inspector Variables
	public AudioSource m_powerCorePickup1;
	public AudioSource m_powerCorePickup2;
	public AudioSource m_powerCorePickup3;
	#endregion

	#region Instance Variables
	private bool m_isCollected = false;
	private Animator m_animator;
	#endregion

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	public void Start()
	{
		m_animator = GetComponent<Animator>();
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	public void Update()
	{
		UpdateAnimator();
	}

	private void UpdateAnimator()
	{
		if(m_animator != null)
		{
			m_animator.SetBool("isCollected", m_isCollected);
		}
	}

	#region Update Methods
    protected void OnTriggerEnter2D(Collider2D collidee)
    {
        if (collidee.tag == "Player" && !m_isCollected)
        {
			m_isCollected = true;

			GameObject player = GameObject.FindWithTag("Player");

			if(player != null)
			{
				Player playerScript = player.GetComponent<Player>();

				if(playerScript != null)
				{
					AudioSource soundToPlay = null;

					playerScript.addPowerCore();

					switch(playerScript.CurrentCores)
					{
					case 1:
						soundToPlay = m_powerCorePickup1;
						break;
					
					case 2:
						soundToPlay = m_powerCorePickup2;
						break;
					
					case 3:
						soundToPlay = m_powerCorePickup3;
						break;
					}

					if(soundToPlay != null)
						AudioSource.PlayClipAtPoint(soundToPlay.clip, player.transform.position);
				}
			}
        }
    }

	public void Destroy()
	{
		Destroy(gameObject);
	}
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
