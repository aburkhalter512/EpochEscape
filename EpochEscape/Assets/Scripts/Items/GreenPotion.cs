using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using G = GameManager;

public class GreenPotion : ActiveItem
{
    #region Inspector Variables
    public AudioSource m_flaskSmash;
    public AudioSource m_flaskStrike;
    #endregion

    #region Instance Variables
    Animator m_animator;
    bool mThrown = false;
    bool m_isBroken;

    Player mPlayer;
    #endregion

    #region Class Constants
    public const float SPEED = 2f;
    public const float ROTATION_SPEED = 20f; // degrees
    #endregion

    protected void Awake()
    {
        base.Awake();

        gameObject.tag = "EmptyFlask";

        m_animator = GetComponent<Animator>();
        m_isBroken = false;
    }
    
    public void Update ()
    {
        if(!G.getInstance ().paused)
        {
            if(mThrown)
            {
                Transform parent = transform.parent;

                if(parent != null)
                {
                    parent.transform.position += (SPEED * Time.smoothDeltaTime) * parent.transform.up;
                    
                    transform.RotateAround(parent.transform.position, -Vector3.forward, ROTATION_SPEED);
                }
            }

            UpdateAnimator();
        }
    }

    #region Interface Methods
    public override void PickUp(Player player)
    {
        if (player == null || mThrown)
            return;

        mPlayer = player;

        PickUpSound.Play();

        if (player.inventory.add(this))
        {
            mIsInventory = true;

            if (mSR == null)
                Debug.Log("fail!");
            mSR.enabled = false;
        }
    }
    
    public override void Activate ()
    {
        Throw();

        gameObject.renderer.enabled = true;
    }
    #endregion

    #region Instance Methods
    private void Throw()
    {
        if (mThrown || !mIsInventory)
            return;

        gameObject.tag = "ItemThrown";

        ActivateSound.Play ();
        GameObject flaskThrowPosition = GameObject.FindGameObjectWithTag("FlaskThrowPosition");

        Transform parent = transform.parent;

        if(parent != null)
        {
            if(flaskThrowPosition != null)
                parent.transform.position = flaskThrowPosition.transform.position;
                
            parent.transform.up = mPlayer.transform.up;
            transform.up = -mPlayer.transform.up;

            mThrown = true;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!mThrown)
            base.OnTriggerEnter2D (other);
        
        if (other.GetComponent<Guard>() != null && mThrown)
        {
            if(!m_isBroken && m_flaskStrike != null)
                AudioSource.PlayClipAtPoint(m_flaskStrike.clip, transform.position);

            m_isBroken = true;
            mThrown = false;

            Guard guard = other.gameObject.GetComponent<Guard>();

            guard.m_currentState = Guard.State.STUN;
            guard.transform.up = Vector3.up;

            other.enabled = false;
        }

        if((other.gameObject.tag == "Wall" || other.gameObject.tag == "OrganicCurtain")
            && gameObject.tag == "ItemThrown")
        {
            if(!m_isBroken)
                PlaySmashSound();

            m_isBroken = true;
            mThrown = false;
        }
    }

    private void UpdateAnimator()
    {
        if(m_animator != null)
        {
            m_animator.SetBool("isThrown", mThrown);
            m_animator.SetBool("isBroken", m_isBroken);
        }
    }

    private void PlaySmashSound()
    {
        if(m_flaskSmash != null)
            AudioSource.PlayClipAtPoint(m_flaskSmash.clip, transform.position);
    }

    private void Destroy()
    {
        Transform parent = transform.parent;
        
        if(parent != null)
            Destroy(parent.gameObject);
        else
            Destroy(gameObject);
    }
    #endregion
}
