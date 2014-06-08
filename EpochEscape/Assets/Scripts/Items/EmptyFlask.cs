using UnityEngine;
using System.Collections;
using G = GameManager;

public class EmptyFlask : Item
{
    #region Inspector Variables
	public float throwTime = 0.75f;
    public float initialSpeed = 2f;
	public Vector3 endScale = new Vector3(0.75f, 0.75f, 0.75f);

	public const float SPEED = 2f;
	public const float ROTATION_SPEED = 20f; // degrees

	public AudioSource m_flaskSmash;
	public AudioSource m_flaskStrike;
    #endregion

    #region Private Variables
    private Animator m_animator;
    private bool thrown = false;
	private bool m_isBroken;

    Vector3 mOrigin;
    Vector3 mDestination;
    float mCurrentSpeed;
    float deceleration;
    float startTime;
	Vector3 mScaleDelta;
    #endregion

    #region Class Variables
    #endregion

    public void Start()
	{
		gameObject.tag = "EmptyFlask";

		m_animator = GetComponent<Animator>();
		m_isBroken = false;
	}
	
	public void Update ()
	{
		if(!G.getInstance ().paused)
		{
			if(thrown)
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
	
	public override void PickUp(Player player)
	{
		PickUpSound.Play ();
	}
	
	public override void Activate ()
	{
		Throw ();
		gameObject.renderer.enabled = true;
	}

	private void Throw()
	{
		gameObject.tag = "ItemThrown";
		
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		
		if(player != null)
		{
			ActivateSound.Play ();
			GameObject flaskThrowPosition = GameObject.FindGameObjectWithTag("FlaskThrowPosition");

			Transform parent = transform.parent;

			if(parent != null)
			{
				parent.transform.position = player.transform.position;
				
				// This should not work, but it does.
				if(flaskThrowPosition != null)
					parent.transform.position = flaskThrowPosition.transform.position;
				
				parent.transform.up = player.transform.up;
				transform.up = -player.transform.up;

				thrown = true;
			}
		}
	}

	public override void OnTriggerEnter2D(Collider2D other){
		base.OnTriggerEnter2D (other);
		
        if (other.gameObject.tag == "Guard" && gameObject.tag == "ItemThrown")
        {
			if(!m_isBroken && m_flaskStrike != null)
				AudioSource.PlayClipAtPoint(m_flaskStrike.clip, transform.position);

			m_isBroken = true;
			thrown = false;

			Guard guardManager = other.gameObject.GetComponent<Guard>();
            if (guardManager == null)
                other.gameObject.GetComponent<StationaryGuard>();

            if (guardManager != null)
			{
                guardManager.m_currentState = Guard.State.STUN;
				guardManager.transform.up = Vector3.up;

				other.enabled = false;
			}
        }

		if(other.gameObject.tag == "Wall" && gameObject.tag == "ItemThrown")
		{
			if(!m_isBroken)
				PlaySmashSound();

			m_isBroken = true;
			thrown = false;
		}
	}

	private void UpdateAnimator()
	{
		if(m_animator != null)
		{
			m_animator.SetBool("isThrown", thrown);
			m_animator.SetBool("isBroken", m_isBroken);
		}
	}

	private void PlaySmashSound()
	{
		if(m_flaskSmash != null)
			AudioSource.PlayClipAtPoint(m_flaskSmash.clip, transform.position);
	}

	public void Destroy()
	{
		Destroy(gameObject);
	}
}
