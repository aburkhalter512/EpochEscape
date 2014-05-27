using UnityEngine;
using System.Collections;
using G = GameManager;

public class EmptyFlask : Item
{
    #region Inspector Variables
	public float throwTime = 0.75f;
    public float deceleration = 2f;
    public Vector3 endScale = new Vector3(0.75f, 0.75f, 0.75f);
    #endregion

    #region Private Variables
    private Animator m_animator;
    private bool thrown = false;
	private bool m_isBroken;

    Vector3 mOrigin;
    Vector3 mDestination;
    float mCurrentSpeed;
    float startTime;
    Vector3 mScaleDelta;
    #endregion

    #region Class Variables
    public const int MAX_COUNT = 2;
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
		audio.Play ();
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
            Debug.Log((mDestination - mOrigin).magnitude);
		}
	}
	
	public override void Add(){
		if (count < MAX_COUNT)
			count++;
	}

	public override void OnTriggerEnter2D(Collider2D other){
		base.OnTriggerEnter2D (other);
		
        if (other.gameObject.tag == "Guard")
        {
			m_isBroken = true;
			thrown = false;

			Guard guardManager = other.gameObject.GetComponent<Guard>();

			if(guardManager != null)
				guardManager.m_currentState = Guard.State.STUN;
        }

		if(other.gameObject.tag == "Wall")
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
