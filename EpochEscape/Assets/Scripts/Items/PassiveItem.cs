using UnityEngine;
using System.Collections;

public abstract class PassiveItem : Item
{
	#region Interface Variables
	#endregion

	#region Instance Variables
	#endregion

	#region Inteface Variables
    public abstract void destroy();
	#endregion

	#region Instance Variables
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            if (player.inventory.canAdd(this))
            {
                PickUp(player);

                destroy();
            }
        }
    }
	#endregion
}
