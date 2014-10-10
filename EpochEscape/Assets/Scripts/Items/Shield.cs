using UnityEngine;
using System.Collections;

public class Shield : ActiveItem {

	// Use this for initialization
	void Start () {
		gameObject.tag = "Special Item";
	}

	public override void PickUp(Player player)
	{
		/*PickUpSound.Play ();
		player.m_hasSpecialItem = true;
		player.m_specialItemCollectTime = Time.time;
		player.m_playSpecialItemAnim = true;*/
	}

	public override void Activate(){
		/*ActivateSound.Play ();

		GameObject playerObject = GameObject.FindWithTag("Player");

		if(playerObject != null)
		{
			Player player = playerObject.GetComponent<Player>();

			if(player != null)
			{
				player.m_isAttacking = false;
				player.m_isShieldActive = true;
				player.m_shieldTime = Time.time;
			}
		}*/
	}
}
