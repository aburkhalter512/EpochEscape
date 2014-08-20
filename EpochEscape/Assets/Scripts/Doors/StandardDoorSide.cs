using UnityEngine;
using System.Collections;

public class StandardDoorSide : DoorSide
{
    #region Interface Variables
    public Sprite openSprite;
    public Sprite closeSprite;
    #endregion

    #region Instance Variables
    protected BoxCollider2D mCollider;

    bool mIsOpened;
	#endregion

    #region Class Constants
    public static string OPEN_SPRITE = "Textures/Doors/DoorSides/Open";
    public static string CLOSE_SPRITE = "Textures/Doors/DoorSides/Closed";
    #endregion

    //Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
        base.Start();

        if (openSprite == null)
            openSprite = Resources.Load<Sprite>(OPEN_SPRITE);

        if (closeSprite == null)
            closeSprite = Resources.Load<Sprite>(CLOSE_SPRITE);

        mCollider = gameObject.GetComponent<BoxCollider2D>();

        mSR.sprite = closeSprite;
        mIsOpened = false;
	}
	
	#region Interface Methods
    public override void init()
    {
        return;
    }

    public override void open()
    {
        if (!mIsOpened)
        {
            mSR.sprite = openSprite;
            mIsOpened = true;

            Destroy(mCollider);
            mCollider = null;
        }
    }

    public override void close()
    {
        if (mIsOpened)
        {
            mSR.sprite = closeSprite;
            mIsOpened = false;

            mCollider = gameObject.AddComponent<BoxCollider2D>();
            mCollider.size = DoorSide.SIZE;
            mCollider.center = new Vector2(0.0f, -.1f);
        }
    }

    public override void toggle()
    {
        if (!mIsOpened)
        {
            mSR.sprite = openSprite;
            mIsOpened = true;
        }
        else
        {
            mSR.sprite = closeSprite;
            mIsOpened = false;
        }
    }

    public override void triggerFrontEnter()
    {
        if (!mWasFrontHit && !mWasBackHit)
        {
            if (mSide == NEWDoor.SIDE.FIRST)
                mDoor.firstSideState = NEWDoor.SIDE_STATE.OPENED;
            else
                mDoor.secondSideState = NEWDoor.SIDE_STATE.OPENED;
        }

        mWasFrontHit = true;
    }

    public override void triggerFrontExit()
    {
        if (mWasFrontHit && !mWasBackHit)
        {
            if (mSide == NEWDoor.SIDE.FIRST)
                mDoor.firstSideState = NEWDoor.SIDE_STATE.CLOSED;
            else
                mDoor.secondSideState = NEWDoor.SIDE_STATE.CLOSED;
        }

        mWasFrontHit = false;
    }

    public override void triggerBackEnter()
    {
        if (!mWasFrontHit && !mWasBackHit)
        {
            if (mSide == NEWDoor.SIDE.FIRST)
                mDoor.firstSideState = NEWDoor.SIDE_STATE.OPENED;
            else
                mDoor.secondSideState = NEWDoor.SIDE_STATE.OPENED;
        }

        mWasBackHit = true;
    }

    public override void triggerBackExit()
    {
        if (!mWasFrontHit && mWasBackHit)
        {
            if (mSide == NEWDoor.SIDE.FIRST)
                mDoor.firstSideState = NEWDoor.SIDE_STATE.CLOSED;
            else
                mDoor.secondSideState = NEWDoor.SIDE_STATE.CLOSED;
        }

        mWasBackHit = false;
    }
	#endregion
}
