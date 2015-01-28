using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PowerCore : MonoBehaviour, IResettable
{
	#region Inspector Variables
    public AudioSource m_powerCorePickup1 = null;
    public AudioSource m_powerCorePickup2 = null;
    public AudioSource m_powerCorePickup3 = null;
	#endregion

	#region Instance Variables
    private Animator m_animator = null;
    private Collider2D m_collider = null;
    private SpriteRenderer m_renderer = null;

    private Sprite m_initialSprite = null;

    private int m_isCollectedHash = 0;
	#endregion

	public void Start()
	{
		m_animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider2D>();
        m_renderer = GetComponent<SpriteRenderer>();

        if (m_animator != null)
            m_isCollectedHash = Animator.StringToHash("isCollected");

        if (m_renderer != null)
            m_initialSprite = m_renderer.sprite;
	}

	#region Instance Methods
    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            if(m_animator != null)
                m_animator.SetTrigger(m_isCollectedHash);

            if(m_collider != null)
                m_collider.enabled = false;

            AudioSource soundToPlay = null;

            Player.Get().addCore();

            switch (PlayerManager.GetCores())
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

            if (soundToPlay != null)
                AudioSource.PlayClipAtPoint(soundToPlay.clip, PlayerManager.GetPosition());
        }
    }

	public void Collect()
	{
        if (m_renderer != null)
            m_renderer.enabled = false;

        if (m_animator != null)
            m_animator.enabled = false;

        if (m_initialSprite != null)
            m_renderer.sprite = m_initialSprite;
	}
	#endregion

    public void Reset()
    {
        if(m_renderer != null)
            m_renderer.enabled = true;

        if (m_animator != null)
            m_animator.enabled = true;

        if (m_collider != null)
            m_collider.enabled = true;

        PlayerManager.RemoveCore();
    }
}
