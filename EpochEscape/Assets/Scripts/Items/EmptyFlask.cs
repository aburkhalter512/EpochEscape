using UnityEngine;
using System.Collections;
using G = GameManager;

public class EmptyFlask : Item {
	public const int MAX_COUNT = 2;
	public const float Speed = 2f;
	private bool thrown = false;
	private Animator m_animator;

	private bool m_isBroken;
	
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
				transform.position += (Speed * Time.smoothDeltaTime) * transform.up;

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
