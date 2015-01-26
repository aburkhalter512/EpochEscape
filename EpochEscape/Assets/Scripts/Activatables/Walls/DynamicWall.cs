using UnityEngine;
using System.Collections;

/*
 * This script represents a changing wall, and is thus abstract. This class
 * allows for more different type of changing walls to be added easily.
 */
public abstract class DynamicWall : MonoBehaviour, IActivatable, ITransitional
{
    #region Inspector Variables
    public float changeTime = 1.0f;
    public int currentIndex = 0; //index of rotations
    #endregion

    #region Instance Variables
    protected Vector2 size = Vector2.zero;

    protected SpriteRenderer sr;

    protected STATE mState;
    #endregion

    #region Class Constants
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
        sr = GetComponent<SpriteRenderer>();
        UpdateSize();
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
        mState = STATE.TO_CHANGE;
    }

    public void OnFinishTransition()
    {
        // Instructs the wall to begin rotating.
        mState = DynamicWall.STATE.TO_CHANGE;
    }

    public void OnReadyIdle() { }

    public float GetWaitTime()
    {
        return changeTime;
    }
    #endregion

    #region Instance Methods
    /*
     * Updates the bounds of the dynamic wall.
     */
    protected void UpdateSize()
    {
        if (sr == null)
            return;

        size = sr.bounds.extents;
    }

    protected abstract void toChange();
    protected abstract void change();
    #endregion
}
