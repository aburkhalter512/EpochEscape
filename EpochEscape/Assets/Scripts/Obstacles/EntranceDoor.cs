using UnityEngine;
using System.Collections;

public class EntranceDoor : Door
{
	#region Inspector Variables
	#endregion

	#region Instance Variables
    protected Player player;
	#endregion

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected override void Start()
	{
        base.Start();
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected override void Update()
	{
        base.Update();
	}

	#region Update Methods

    protected void OnTriggerExit2D(Collider2D collidee)
    {
        if (collidee.gameObject.tag == "Player")
        {
            Object.Destroy(gameObject.GetComponent<BoxCollider2D>());

            currentState = STATE.CLOSED;
        }
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
