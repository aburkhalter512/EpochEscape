using UnityEngine;
using System.Collections;

public class Door : Obstacle
{
	#region Inspector Variables
    public STATE currentState;

    public Sprite closedDoor;
    public Sprite openedDoor;

    public Vector2 mSize;
	#endregion

    #region Instance Variables
    private STATE mPreviousState = STATE.CLOSED;

    private BoxCollider2D mMainCollider = null;

    private SpriteRenderer mSR;
	#endregion

    #region Class Constants
    public enum STATE
    {
        OPENED,
        CLOSED,
        UN_INIT,
        DESTROY
    }
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected virtual void Start()
	{
        mPreviousState = STATE.UN_INIT;

        mSR = gameObject.GetComponent<SpriteRenderer>();
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected virtual void Update()
	{
        if (mPreviousState != currentState)
        {
            switch (currentState)
            {
                case STATE.UN_INIT:
                    UnInit();
                    break;
                case STATE.CLOSED:
                    Close();
                    break;
                case STATE.OPENED:
                    Open();
                    break;
                case STATE.DESTROY:
                    Destroy();
                    break;
            }

            mPreviousState = currentState;
        }
	}

	#region Update Methods
    protected virtual void UnInit()
    {
    }

    protected virtual void Close()
    {
        if (mMainCollider == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
            mMainCollider = gameObject.GetComponent<BoxCollider2D>();
        }

        mMainCollider.size = mSize;
        mMainCollider.center = Vector2.zero;

        mSR.sprite = closedDoor;
    }

    protected virtual void Open()
    {
        if (mMainCollider != null)
        {
            Object.Destroy(mMainCollider);
            mMainCollider = null;
        }

        mSR.sprite = openedDoor;
    }

    protected virtual void Destroy()
    {
        Destroy(gameObject);
    }
	#endregion

    #region Interface Methods
    public STATE Toggle()
    {
        switch (currentState)
        {
            case STATE.CLOSED:
                currentState = STATE.OPENED;
                break;
            case STATE.OPENED:
                currentState = STATE.CLOSED;
                break;
        }

        return currentState;
    }
    #endregion

    #region Static Methods
    #endregion

    #region Utilities
    #endregion
}
