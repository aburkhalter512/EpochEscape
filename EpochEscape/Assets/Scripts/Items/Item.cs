using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour {
	public AudioSource PickUpSound = null;
	public AudioSource ActivateSound = null;
	public Item traversal;
	public Item next = null;
	public bool inInventory = false;
	abstract public void PickUp(Player player);	
	abstract public void Activate();
	public virtual void Add(Item i){
		traversal = this;
		while(traversal.next != null)
			traversal = traversal.next;
		traversal.next = i;
	}
	public virtual void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Player" && other.GetComponent<Player>().inventory.canAdd(this) && !inInventory)
		{
			Player player = other.GetComponent<Player>();
			PickUp(player);
			player.inventory.addItem(this);
			inInventory = true;
			gameObject.renderer.enabled = false;
		}
	}
}
