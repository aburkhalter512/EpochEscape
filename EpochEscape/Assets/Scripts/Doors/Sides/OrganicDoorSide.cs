using UnityEngine;
using System.Collections;

public class OrganicDoorSide : DoorSide
{
    #region Interface Variables
    #endregion
    
    #region Instance Variables
    BoxCollider2D mCollider;
    #endregion 
    
    protected void Awake()
    {
        base.Awake();

        mCollider = GetComponent<BoxCollider2D>();
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
		GameObject go = GameObject.FindGameObjectWithTag("Player");
		Player p = go.GetComponent<Player>();
		if (p != null) {
			if (p.m_isHoldingBox)
				gameObject.collider2D.isTrigger = false;
			else
				gameObject.collider2D.isTrigger = true;
		}
        return;
    }
    public override void triggerFrontExit()
    {
        return;
    }

    public override void triggerBackEnter()
    {
    	/*
		GameObject go = GameObject.FindGameObjectWithTag("Player");
		Player p = go.GetComponent<Player>();
		if (p != null) {
			if (p.m_isHoldingBox)
				gameObject.collider2D.isTrigger = true;
			else
				gameObject.collider2D.isTrigger = false;
		}
		*/
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

            mCollider.enabled = true;

            mIsActive = true;
            mIsFirstUpdate = false;
        }
    }

    public override void deactivate()
    {
        if (isActive() || mIsFirstUpdate)
        {
            mSR.sprite = deactiveSprite;

            mCollider.enabled = false;

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

    #region Instance Methods
    protected virtual void OnTriggerEnter2D(Collider2D collidee)
    { }
    #endregion
}
