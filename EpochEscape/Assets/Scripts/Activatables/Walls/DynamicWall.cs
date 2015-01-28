using UnityEngine;
using System.Collections;

/*
 * This script represents a changing wall, and is thus abstract. This class
 * allows for more different type of changing walls to be added easily.
 */
public abstract class DynamicWall : MonoBehaviour, IActivatable
{
    #region Instance Variables
    protected int mCurrentIndex = 0;
    protected float mCurrentChangeTime = 0.0f;

    protected STATE mState;
    #endregion

    #region Class Constants
    public static readonly float CHANGE_TIME = 1.0f;

    public enum STATE
    {
        STATIONARY = 0,
        TO_CHANGE,
        CHANGE
    };
    #endregion

    /*
     * Initializes the Dynamic Wall
     */
    protected void Awake()
    {
        mState = STATE.STATIONARY;
    }

    /*
     * Updates the Dynamic Wall.
     */
    protected void Update()
    {
        switch (mState)
        {
            case STATE.STATIONARY:
                break;
            case STATE.TO_CHANGE:
                toChange();
                break;
            case STATE.CHANGE:
                change();
                break;
        }
    }

    #region Interface Methods
    /*
     * Only activate() does changes the dynamic wall. Both deactivate and toggle are empty methods
     */
    public void activate() { }
    public void deactivate() { }
    public void toggle()
    {
        if (mState != STATE.CHANGE)
            mState = STATE.TO_CHANGE;
    }
    #endregion

    #region Instance Methods
    protected abstract void toChange();
    protected abstract void change();
    #endregion
}
