using UnityEngine;
using System.Collections;
using G = GameManager;

public class RedPotion : Item {
	public const int MAX_COUNT = 2;
	public const float Speed = 2f;
	private bool thrown = false;
	private bool m_isBroken = false;

	//private Animator m_animator;

	public void Start()
	{
		gameObject.tag = "Red Potion";

		//m_animator = GetComponent<Animator>();
	}

	public void Update ()
	{
	}

	public override void PickUp(Player player)
	{

	}

	public override void Activate ()
	{
		audio.Play ();
		Player player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
		player.m_detectionLevel -= 25f;
		if (player.m_detectionLevel < 0)
			player.m_detectionLevel = 0;
	}

	public override void Add(){
		if (count < MAX_COUNT)
			count++;
	}

	public override void OnTriggerEnter2D(Collider2D other){
		base.OnTriggerEnter2D (other);
	}

	public void Destroy()
	{
		Destroy(gameObject);
	}
}