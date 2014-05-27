using UnityEngine;
using System.Collections;

public class Door : Obstacle
{
	#region Inspector Variables
    public STATE currentState;

    public Sprite closedDoor;
    public Sprite openedDoor;
	#endregion

    #region Instance Variables
    private STATE mPreviousState = STATE.CLOSED;

    private BoxCollider2D mMainCollider = null;
    private Vector2 mSize;

    private SpriteRenderer mSR;
	#endregion

    #region Class Constants
    public enum STATE
    {
        OPENED,
        CLOSED,
        UN_INIT
    }
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected new void Start()
	{
        mPreviousState = STATE.UN_INIT;

        mSR = gameObject.GetComponent<SpriteRenderer>();
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected new void Update()
	{
        Debug.Log(mPreviousState);
        if (mPreviousState != currentState)
        {
            mSize = new Vector2(mSR.bounds.extents.x * 2, mSR.bounds.extents.y * 2);

            switch (currentState)
            {
                case STATE.CLOSED:
                    Close();
                    break;
                case STATE.OPENED:
                    Open();
                    break;
            }

            mPreviousState = currentState;
        }
	}

	#region Update Methods
    protected void Close()
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

    protected void Open()
    {
        if (mMainCollider != null)
        {
            Object.Destroy(mMainCollider);
            mMainCollider = null;
        }

        mSR.sprite = openedDoor;
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
