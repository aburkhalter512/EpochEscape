using UnityEngine;
using System.Collections;

public class OneWayDoorTrigger : MonoBehaviour
{
	#region Inspector Variables
	#endregion

	#region Instance Variables
    protected Player player;
	#endregion

	#region Class Constants
	#endregion

	#region Initialization Methods
	#endregion

	#region Update Methods
    protected void OnTriggerEnter2D(Collider2D collidee)
    {
        if (collidee.gameObject.tag == "Player")
        {
            Door parent = transform.parent.gameObject.GetComponent<Door>();

            if (parent != null)
            {
                parent.currentState = Door.STATE.OPENED;
                Destroy(this);
            }
        }
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
