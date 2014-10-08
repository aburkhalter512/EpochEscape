using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public abstract class Item : MonoBehaviour {
	public AudioSource PickUpSound;
	public AudioSource ActivateSound;

	public Item traversal;
	public Item next = null;
	public bool inInventory = false;

	public abstract void PickUp(Player player);	
	public abstract void Activate();

	public virtual void Add(Item i){
		traversal = this;
		while(traversal.next != null)
			traversal = traversal.next;
		traversal.next = i;
	}

	public virtual void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player")
		{
			if(other.GetComponent<Player>().inventory.canAdd(this) && !inInventory)
			{
				Player player = other.GetComponent<Player>();
				PickUp(player);
				player.inventory.addItem(this);
				inInventory = true;
				gameObject.renderer.enabled = false;
			}
		}
	}
}
