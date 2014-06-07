using UnityEngine;
using System.Collections;

public class Inventory{
	#region member variables
	public const int UNIQUE_ITEMS = 3; //only item currently is potion
	public const int MAX_STACK = 10;
	public const int POTION_SLOT = 0;
	public const int FLASK_SLOT = 1;
	public const int DASH_SLOT = 2;

	public Item[] inventory = new Item[UNIQUE_ITEMS];
	public int[] inventoryCount = new int[UNIQUE_ITEMS];
	#endregion


	public Inventory(){
	}

	public void addItem(Item i){
		switch (i.tag) {
			case "Red Potion":
				if (inventory[POTION_SLOT] == null) {
					inventory[POTION_SLOT] = i;
				} else {
					inventory[POTION_SLOT].Add(i);
				}			
				inventoryCount[POTION_SLOT]++;
				break;
			case "EmptyFlask":
				if (inventory[FLASK_SLOT] == null) {
					inventory[FLASK_SLOT] = i;
				} else {
					inventory[FLASK_SLOT].Add(i);
				}			
				inventoryCount[FLASK_SLOT]++;
				break;
		case "DashPotion":
			if(inventory[DASH_SLOT] == null){
				inventory[DASH_SLOT] = i;
			}
			else{
				inventory[DASH_SLOT].Add(i);
			}
			inventoryCount[DASH_SLOT]++;
			break;
		}
	}

	public void activateItem(int slot){
		//if inventory slot is not empty
		if(inventory[slot] != null){
			//activate and adjust inventory count and slot
			inventory[slot].Activate();
			inventory[slot] = inventory[slot].next;
            inventoryCount[slot]--;
		}
	}
	
	public bool canAdd(Item i){
		//if the current item is in the inventory
		if(inInventory (i))
			//return whether or not there is space for it
			return hasSpace (i);
		//if it's not in the inventory check if there is an empty slot for it
		else
			return hasEmptySlot ();
	}
	
	public bool hasEmptySlot(){
		//if inventory has empty slot return true
		for(int i = 0; i < inventory.Length; i++)
			if(inventory[i] == null)
				return true;
		//otherwise return false
		return false;
	}

	public bool inInventory(Item i){
		for(int j = 0; j < inventory.Length; j++){
			//if there is a slot for this item
			//if inventory slot is null continue to next slot
			if(inventory[j] == null)
				continue;
			//return true
			if(inventory[j].gameObject.tag == i.gameObject.tag)
				return true;
		}
		//otherwise return null
		return false;
	}
	
	public bool hasSpace(Item i){
		//go through array
		for(int j = 0; j < inventory.Length; j++){
			//if there is a slot for this item
			if(inventory[j] == null)
				continue;
			if(inventory[j].gameObject.tag == i.gameObject.tag)
				//if there is space
				if(inventoryCount[j] < MAX_STACK)
					return true;
		}
		//no slot for item
		return false;
	}

	public bool InventoryFull(){
		for(int i = 0; i < inventoryCount.Length; i++)
			if(inventoryCount[i] > 0)
				return false;
		return false;
	}

	public void clear(){
		for (int i = 0; i < inventory.Length; i++)
			inventory [i] = null;
	}
}
