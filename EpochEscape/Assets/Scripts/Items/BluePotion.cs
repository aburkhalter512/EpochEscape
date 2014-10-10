using UnityEngine;
using System.Collections;

public class BluePotion : PassiveItem
{
	#region Interface Variables
	#endregion

	#region Instance Variables
	#endregion

	protected virtual void Awake()
	{ }

	protected virtual void Start()
	{ }

	protected virtual void Update()
	{ }

	#region Inteface Variables
    public override void PickUp(Player player)
    {
        return;
    }

    public override void destroy()
    {
        return;
    }
	#endregion

	#region Instance Variables
	#endregion
}
