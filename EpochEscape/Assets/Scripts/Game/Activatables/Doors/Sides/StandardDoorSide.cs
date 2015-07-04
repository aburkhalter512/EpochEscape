using UnityEngine;
using System.Collections;

public class StandardDoorSide : DoorSide
{
    #region Interface Variables
    #endregion
    
    #region Instance Variables
    BoxCollider2D mCollider = null;
    #endregion 

    protected new void Awake()
    {
        base.Awake();

        mCollider = gameObject.AddComponent<BoxCollider2D>();
        mCollider.size = DoorSide.SIZE;
        mCollider.offset = new Vector2(0.0f, -.125f);
    }
    
    #region Interface Methods
    public override void triggerFrontEnter()
    {
        return;
    }
    public override void triggerFrontExit()
    {
        return;
    }

    public override void triggerBackEnter()
    {
        return;
    }
    public override void triggerBackExit()
    {
        return;
    }

    public override void activate()
    {
        if (!isActive() || mIsFirstUpdate)
        {
            mSR.sprite = activeSprite;

            mCollider.enabled = false;

            mIsActive = true;
            mIsFirstUpdate = false;
        }
    }

    public override void deactivate()
    {
        if (isActive() || mIsFirstUpdate)
        {
            mSR.sprite = inactiveSprite;

            mCollider.enabled = true;

            mIsActive = false;
            mIsFirstUpdate = false;
        }
    }
    #endregion
}
