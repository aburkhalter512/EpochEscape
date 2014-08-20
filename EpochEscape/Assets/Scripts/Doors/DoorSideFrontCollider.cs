using UnityEngine;
using System.Collections;

public class DoorSideFrontCollider : MonoBehaviour
{
	#region Instance Variables
    protected DoorSide mDoorSide;

    protected BoxCollider2D mCollider;
	#endregion

    #region Class Constants
    public static Vector2 SIZE = new Vector2(DoorSide.SIZE.x, DoorSide.SIZE.y);
    #endregion

    //Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
        mDoorSide = transform.parent.GetComponent<DoorSide>();

        if (mDoorSide == null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
	}

	#region Update Methods
    public void OnTriggerEnter2D(Collider2D collidee)
    {
        if (collidee.tag == "Player")
            mDoorSide.triggerFrontEnter();
    }

    public void OnTriggerExit2D(Collider2D collidee)
    {
        if (collidee.tag == "Player")
            mDoorSide.triggerFrontExit();
    }
	#endregion
}
