using UnityEngine;
using System.Collections;

public class Inventory{
	#region member variables
	public const int MAX_ITEMS = 2; //only item currently is potion
	public Item[] inventory = new Item[MAX_ITEMS];
	public int[] inventoryCount = new int[MAX_ITEMS];
	#endregion

    public const int MAX_SLOTSIZE = 2;

	public void addItem(Item i){
		for (int j = 0; j < inventory.Length; j++) {
			if(inventory[j] == null){
				inventory[j] = i;
				inventoryCount[j] = 1;
				break;
			}
			else if(inventory[j].gameObject.tag == i.gameObject.tag && inventoryCount[j] < MAX_SLOTSIZE){
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

	public bool canAdd(Item i){
		if(InventoryFull())
			return false;
		if(inInventory (i))
			return hasSpace (i);
		else
			return hasEmptySlot ();
	}
	
	public bool hasEmptySlot(){
		for(int i = 0; i < inventory.Length; i++)
			if(inventory[i] == null)
				return true;
		return false;
	}

	public bool inInventory(Item i){
		for(int j = 0; j < inventory.Length; j++){
			//if there is a slot for this item
			if(inventory[j] == null)
				continue;
			if(inventory[j].gameObject.tag == i.gameObject.tag)
				return true;
		}
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
				if(inventoryCount[j] < MAX_SLOTSIZE)
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
