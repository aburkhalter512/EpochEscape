using UnityEngine;
using System.Collections;

public class PressureSwitch : PressurePlate
{
	#region Inspector Variables
	#endregion

	#region Instance Variables
	#endregion

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
	}

    #region Update Methods
    void OnTriggerEnter2D(Collider2D collidee)
    {
		if(collidee.tag == "Player")
		{
	        base.OnTriggerEnter2D(collidee);

	        Destroy(gameObject);
		}
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
