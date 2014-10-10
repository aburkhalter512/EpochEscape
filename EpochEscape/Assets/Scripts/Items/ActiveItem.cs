using UnityEngine;
using System.Collections;

public abstract class ActiveItem : Item
{
	#region Interface Variables
	#endregion

	#region Instance Variables
    protected bool mIsInventory = false;
	#endregion

	#region Inteface Variables
    public abstract void Activate();
	#endregion

	#region Instance Variables
	#endregion
}
