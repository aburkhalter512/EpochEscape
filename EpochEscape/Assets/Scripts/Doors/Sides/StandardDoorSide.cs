using UnityEngine;
using System.Collections;

public class StandardDoorSide : DoorSide
{
    #region Interface Variables
    #endregion
    
    #region Instance Variables
    BoxCollider2D mCollider = null;
    #endregion 
    
    protected void Awake()
    {
        base.Awake();
    }
    
    protected void Start()
    {
        base.Start();
    }
    
    protected void Update()
    {
        base.Update();
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

            if (mCollider != null)
                GameObject.Destroy(mCollider);

            mIsActive = true;
            mIsFirstUpdate = false;
        }
    }

    public override void deactivate()
    {
        if (isActive() || mIsFirstUpdate)
        {
            mSR.sprite = deactiveSprite;

            mCollider = gameObject.AddComponent<BoxCollider2D>();
            mCollider.size = DoorSide.SIZE;
            mCollider.center = new Vector2(0.0f, -.125f);

            mIsActive = false;
            mIsFirstUpdate = false;
        }
    }

    public override void toggle()
    {
        if (mIsActive)
            deactivate();
        else
            activate();
    }
    #endregion
}
