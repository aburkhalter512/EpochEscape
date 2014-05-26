using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour {
	public Item next = null;
	public int count;
	public bool inInventory = false;
	abstract public void PickUp(Player player);	
	abstract public void Activate();
	abstract public void Add();
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
