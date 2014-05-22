using UnityEngine;
using System.Collections;

public class Inventory{
	#region member variables
	public const int MAX_ITEMS = 1; //only item currently is potion
	public Item[] inventory = new Item[MAX_ITEMS];
	public int[] inventoryCount = new int[MAX_ITEMS];
	#endregion

    public const int MAX_POTIONS = 2;

	public void addItem(Item i){
		for (int j = 0; j < inventory.Length; j++) {
			if(inventory[j] == null){
				inventory[j] = i;
				inventoryCount[j] = 1;
				break;
			}
			else if(inventory[j].gameObject.tag == i.gameObject.tag && inventoryCount[j] < MAX_POTIONS){
				inventory[j].Add ();
				inventory[j].next = i;
				inventoryCount[j]++;
				break;
			}
		}
	}
	public void activateItem(int slot){
		if(inventory[slot] != null){
			inventory[slot].Activate();
			inventory[slot] = inventory[slot].next;
            inventoryCount[slot]--;
		}
	}

	public void clear(){
		for (int i = 0; i < inventory.Length; i++)
			inventory [i] = null;
	}
}
