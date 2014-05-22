using UnityEngine;
using System.Collections;
using G = GameManager;

public class RedPotion : Item {
	public const int MAX_COUNT = 2;
	public const float Speed = 2f;
	private bool thrown = false;
	private bool m_isBroken = false;

	private Animator m_animator;

	public void Start()
	{
		gameObject.tag = "Item";

		m_animator = GetComponent<Animator>();
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
        player.m_detectionLevel -= 25f;

        if (player.m_detectionLevel < 0)
            player.m_detectionLevel = 0;
	}

	public override void Activate ()
	{
		SpriteRenderer r = GetComponent<SpriteRenderer>();
		if(r != null){
			Sprite s = Resources.Load ("Textures/Items/EmptyPotion", typeof(Sprite)) as Sprite;
			r.sprite = s;
		}
		Throw ();
		gameObject.renderer.enabled = true;
		count--;
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
		gameObject.tag = "Potion";
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