using UnityEngine;
using System.Collections;
using G = GameManager;

public class EmptyFlask : Item
{
    #region Inspector Variables
	public float throwTime = 0.75f;
    public float initialSpeed = 2f;
	public Vector3 endScale = new Vector3(0.75f, 0.75f, 0.75f);
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
            if (thrown)
            {
                if (Time.realtimeSinceStartup - startTime > throwTime)
                {
                    m_isBroken = true;
                    thrown = false;
                }
                else
                {
                    transform.position += mCurrentSpeed * Time.smoothDeltaTime * transform.up;
                    mCurrentSpeed -= deceleration * Time.smoothDeltaTime;
                    transform.localScale += mScaleDelta * Time.smoothDeltaTime;
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
	
	private void Throw(){
		gameObject.tag = "ItemThrown";

		GameObject player = GameObject.FindGameObjectWithTag("Player");

		if(player != null)
		{
			GameObject flaskThrowPosition = GameObject.FindGameObjectWithTag("FlaskThrowPosition");

			transform.position = player.transform.position;

			// This should not work, but it does. RIP in pepperoni relative positioning. No copy pasterino Al Pacino.
			if(flaskThrowPosition != null)
				transform.position = flaskThrowPosition.transform.position;

			transform.up = player.transform.up;
			thrown = true;

            startTime = Time.realtimeSinceStartup;
            mCurrentSpeed = initialSpeed;
			mOrigin = transform.position;
            mDestination = InputManager.getInstance().mouseInWorld;
            mDestination.z = mOrigin.z; //Avoid incorrect displacement
            mCurrentSpeed = (mDestination - mOrigin).magnitude;
            mCurrentSpeed += (deceleration * throwTime * throwTime) / 2;
            mCurrentSpeed /= throwTime;
            mScaleDelta = endScale - transform.localScale;
            mScaleDelta /= throwTime;

            //Debug.Log(mOrigin);
            //Debug.Log(mDestination);
            //Debug.Log((mDestination - mOrigin).magnitude);
		}
	}

	public override void OnTriggerEnter2D(Collider2D other){
		base.OnTriggerEnter2D (other);
		
        if (other.gameObject.tag == "Guard" && gameObject.tag == "ItemThrown")
        {
			m_isBroken = true;
			thrown = false;

			Guard guardManager = other.gameObject.GetComponent<Guard>();
            if (guardManager == null)
                other.gameObject.GetComponent<StationaryGuard>();

            if (guardManager != null)
            {
                guardManager.m_currentState = Guard.State.STUN;
                
				//Debug.Log("Stunned guard");
            }
        }

		if(other.gameObject.tag == "Wall" && gameObject.tag == "ItemThrown")
		{
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

	public void Destroy()
	{
		Destroy(gameObject);
	}
}
