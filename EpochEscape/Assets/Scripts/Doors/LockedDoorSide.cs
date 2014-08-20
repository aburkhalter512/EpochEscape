using UnityEngine;
using System.Collections;

public class LockedDoorSide : DoorSide
{
    #region Interface Variables
    public Sprite closeSprite;
    #endregion

    #region Instance Variables
    protected BoxCollider2D mCollider;
	#endregion

    #region Class Constants
    public static string CLOSE_SPRITE = "Textures/Doors/DoorSides/Locked";
    #endregion

    //Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
        base.Start();

        if (closeSprite == null)
            closeSprite = Resources.Load<Sprite>(CLOSE_SPRITE);

        mCollider = gameObject.GetComponent<BoxCollider2D>();
        mCollider.isTrigger = false;

        mSR.sprite = closeSprite;
	}
	
	#region Interface Methods
    public override void init()
    {
        return;
    }

    public override void open()
    {
        return;
    }

    public override void close()
    {
        return;
    }

    public override void toggle()
    {
        return;
    }

    public override void triggerFrontEnter()
    {
        mWasFrontHit = true;
    }

    public override void triggerFrontExit()
    {
        mWasFrontHit = false;
    }

    public override void triggerBackEnter()
    {
        mWasBackHit = true;
    }

    public override void triggerBackExit()
    {
        mWasBackHit = false;
    }
	#endregion
}
