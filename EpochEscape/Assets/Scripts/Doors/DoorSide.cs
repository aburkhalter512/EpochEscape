using UnityEngine;
using System.Collections;

public abstract class DoorSide : MonoBehaviour
{
    #region Inspector Variables
    public NEWDoor.SIDE side = NEWDoor.SIDE.FIRST;

    public bool wasBackHit
    {
        get
        {
            return mWasBackHit;
        }
    }
    public bool wasFrontHit
    {
        get
        {
            return mWasFrontHit;
        }
    }
	#endregion

	#region Instance Variables
    protected DoorSideFrontCollider mFrontCollider;
    protected DoorSideBackCollider mBackCollider;

    protected NEWDoor.SIDE mSide;

    protected bool mWasFrontHit = false;
    protected bool mWasBackHit = false;

    protected NEWDoor mDoor;

    protected SpriteRenderer mSR = null;
	#endregion

    #region Class Constants
    public static Vector2 SIZE = new Vector2(NEWDoor.SIZE.x, NEWDoor.SIZE.y / 2); 
    #endregion

    //Put all initialization code here
	//Remember to comment!
	protected virtual void Start()
	{
        mDoor = transform.parent.GetComponent<NEWDoor>();
        if (mDoor == null)
        {
            GameObject.Destroy(gameObject);
            return;
        }

        mDoor.attachSide(side, this);

        mSR = GetComponent<SpriteRenderer>();
        mSR.sortingOrder = mDoor.GetComponent<SpriteRenderer>().sortingOrder - 1;

        mSide = side;
	}

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
	}

    #region Interface Methods
    public abstract void triggerFrontEnter();
    public abstract void triggerFrontExit();

    public abstract void triggerBackEnter();
    public abstract void triggerBackExit();

    public abstract void init();

    public abstract void open();

    public abstract void close();

    public abstract void toggle();
	#endregion
}
