using UnityEngine;
using System.Collections;
using G = GameManager;

public class RedPotion : Item {
	public const float Speed = 2f;
	public const float HEAL_AMOUNT = 25f;
	private bool thrown = false;
	private bool m_isBroken = false;


	//private Animator m_animator;

	public void Start()
	{
		gameObject.tag = "Red Potion";

		//m_animator = GetComponent<Animator>();
	}

	public override void PickUp(Player player)
	{
		PickUpSound.Play ();
		return;
	}

	public override void Activate ()
	{
		ActivateSound.Play ();
		Player player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
		player.m_detectionLevel -= HEAL_AMOUNT;
		if (player.m_detectionLevel < 0)
			player.m_detectionLevel = 0;
		player.m_detectionMax -= HEAL_AMOUNT;
		if (player.m_detectionMax < 0)
			player.m_detecionMax = 0;
	}

	public override void OnTriggerEnter2D(Collider2D other){
		base.OnTriggerEnter2D (other);
	}

	public void Destroy()
	{
		Destroy(gameObject);
	}
}