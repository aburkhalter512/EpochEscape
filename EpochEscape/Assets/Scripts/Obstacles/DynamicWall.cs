using UnityEngine;
using System.Collections;

/*
 * This script represents a changing wall, and is thus abstract. This class
 * allows for more different type of changing walls to be added easily.
 */
public abstract class DynamicWall : Wall, ITransitional
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

    /*
     * Initializes the Dynamic Wall
     */
    protected void Start()
    {
        base.Start();

        sr = GetComponent<SpriteRenderer>();
        UpdateSize();
    }

    /*
     * Updates the Dynamic Wall.
     */
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
    /*
     * Updates the bounds of the dynamic wall.
     */
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

    public IEnumerator OnFinishTransition()
    {
        // Instructs the wall to begin rotating.
        currentState = DynamicWall.STATES.TO_CHANGE;

        // Instructs the camera to wait until the wall has finished rotating.
        yield return new WaitForSeconds(changeTime);

        Debug.Log("Wall has finished rotating.");
    }
}
