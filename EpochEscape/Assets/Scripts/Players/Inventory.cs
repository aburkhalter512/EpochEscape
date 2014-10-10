using UnityEngine;
using System.Collections;

public class Inventory
{
    #region Interface variables
    public ActiveItemNode[] activeItems = new ActiveItemNode[ACTIVE_ITEM_COUNT];
    #endregion
    
	#region Instance Variables
    int mSpecialStamina = MAX_SPECIAL_STAMINA;
	#endregion

    #region Class Constants
    public const int MIN_SPECIAL_STAMINA = 0;
    public const int MAX_SPECIAL_STAMINA = 3;

    public const int RED_POTION_SLOT = 0;
    public const int GREEN_POTION_SLOT = 1;
    public const int ACTIVE_ITEM_COUNT = 2;
    public static readonly int[] MAX_ACTIVE_ITEMS = new int[]{10, 10};

    public class ActiveItemNode
    {
        public ActiveItemNode(ActiveItem activeItem, ActiveItemNode node)
        {
            data = activeItem;
            link = node;

            if (node != null)
                nodesAttached = node.nodesAttached + 1;
        }

        public ActiveItemNode link = null;

        public int nodesAttached = 0;
        public ActiveItem data = null;
    }
    #endregion

    public Inventory()
    { }
    
    public int getSize()
    {
        return ACTIVE_ITEM_COUNT;
    }

    public bool add(ActiveItem item)
    {
        if (item == null)
            return false;

        int slot = hash(item);
        if (slot == -1) //Does the item exist in the inventory
            return false;

        if (activeItems[slot] == null)
            activeItems[slot] = new ActiveItemNode(item, null);
        else if (activeItems[slot].nodesAttached + 1 < MAX_ACTIVE_ITEMS[slot])
            activeItems[slot] = new ActiveItemNode(item, activeItems[slot]);

        return true;
    }

    public bool add(PassiveItem item)
    {
        if (item == null)
            return false;

        if (item as YellowPotion != null)
        {
            if (mSpecialStamina >= MAX_SPECIAL_STAMINA)
                return false;

            mSpecialStamina++;

            return true;
        }
        else if (item as BluePotion != null)
        {
            //Add oxygen conditionals here
        }

        return false;
    }

    public void activateItem(int slot)
    {
        //if inventory slot is not empty
        if(activeItems[slot] != null)
        {
            //activate and adjust inventory count and slot
            activeItems[slot].data.Activate();
            activeItems[slot] = activeItems[slot].link;
        }
    }
    
    public bool canAdd(ActiveItem item)
    {
        int slot = hash(item);

        if (activeItems[slot] == null) ; //Skip over this case
        else if (slot == -1 || activeItems[slot].nodesAttached + 1 >= MAX_ACTIVE_ITEMS[slot])
            return false;

        return true;
    }

    public bool canAdd(PassiveItem item)
    {
        if (item == null)
            return false;

        if (item as YellowPotion != null)
        {
            if (mSpecialStamina >= MAX_SPECIAL_STAMINA)
                return false;

            return true;
        }
        else if (item as BluePotion != null)
        {
            //Add oxygen conditionals here
        }

        return false;
    }

    public void clear()
    {
        ActiveItemNode toDelete;
        ActiveItemNode next;

        for (int i = 0; i < activeItems.Length; i++)
        {
            toDelete = activeItems[i];
            if (toDelete != null)
            {
                if (toDelete.link != null)
                {
                    while (toDelete != null)
                    {
                        next = toDelete.link;

                        toDelete.link = null;
                        toDelete.data = null;

                        toDelete = next;
                    }
                }
            }

            activeItems[i] = null;
        }
    }
    
    public bool activateSpecialItem()
    {
    	if (mSpecialStamina > 0)
    	{
            mSpecialStamina--;
    		
    		return true;
    	}
    	
    	return false;
    }

    public int hash(ActiveItem item)
    {
        if (item == null)
            return -1;

        int retVal = -1;

        if (item as RedPotion != null)
            return RED_POTION_SLOT;
        else if (item as GreenPotion != null)
            return GREEN_POTION_SLOT;

        return retVal;
    }

    public int getSpecialStamina()
    {
        return mSpecialStamina;
    }

    public float getPercentSpecialStamina()
    {
        return ((float) mSpecialStamina) / MAX_SPECIAL_STAMINA;
    }
}
