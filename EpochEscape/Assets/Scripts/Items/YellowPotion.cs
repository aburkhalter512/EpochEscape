using UnityEngine;
using System.Collections;

public class YellowPotion : PassiveItem
{
	#region Interface Variables
	#endregion

	#region Instance Variables
	#endregion

	#region Inteface Variables
    public override void PickUp(Player player)
    {
        if (player.inventory.add(this))
        {
            //Do fancy stuff here
        }
    }

    public override void destroy()
    {
        Destroy(gameObject);
    }
	#endregion

	#region Instance Variables
	#endregion
}
