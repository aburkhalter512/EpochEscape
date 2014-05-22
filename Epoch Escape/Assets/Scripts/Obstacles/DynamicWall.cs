using UnityEngine;
using System.Collections;

public abstract class DynamicWall : Wall
{
    #region Inspector Variables
    public float changeTime = 1.0f;
    public int currentIndex = 0; //index of rotations

    public STATES currentState;
	#endregion

    #region Instance Variables
    protected Vector2 size = Vector2.zero;

    private SpriteRenderer sr;
	#endregion

    #region Class Constants
    public enum STATES
    {
        STATIONARY = 0,
        TO_CHANGE,
        CHANGE
    };
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
    {
        base.Start();

        sr = GetComponent<SpriteRenderer>();
        UpdateSize();
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
        base.Update();

        switch (currentState)
        {
            case STATES.STATIONARY:
                stationary();
                break;
            case STATES.TO_CHANGE:
                toChange();
                break;
            case STATES.CHANGE:
                change();
                break;
        }
	}

    #region Update Methods
    protected void UpdateSize()
    {
        if (sr == null)
            return;

        size = sr.bounds.extents;
    }

    protected abstract void stationary();
    protected abstract void toChange();
    protected abstract void change();
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
