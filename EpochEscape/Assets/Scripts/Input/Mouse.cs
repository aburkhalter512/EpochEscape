using UnityEngine;
using System.Collections;

/*
 * A wrapper class that encapsulates and centralizes all mouse functionality.
 * Useful mouse positions can be retrieved from this class as well as button
 * inputs.
 * 
 * The main purpose of this class is to centralize all mouse functionality,
 * i.e. make it easier to access mouse data.
 */
public class Mouse : UnitySingleton<Mouse>
{
    #region Instance Variables
    Vector3 mScreenPosition;
    Vector3 mWorldPosition;

    Button mLeft;
    Button mRight;
    Button mMiddle;
    #endregion

    #region Class Constants
    public enum BUTTONS
    {
        LEFT = 0,
        RIGHT,
        MIDDLE
    }
    #endregion

    //Put all initialization code here
    //Remember to comment!
    protected void Start()
    {
        UpdatePosition();

        mLeft = new Button(BUTTONS.LEFT);
        mRight = new Button(BUTTONS.RIGHT);
        mMiddle = new Button(BUTTONS.MIDDLE);
    }

    //Put all update code here
    //Remember to comment!
    protected void Update()
    {
        UpdatePosition();
    }

    #region Update Methods
    /*
     * Updates the mouse position both in screen and in world.
     */
    protected void UpdatePosition()
    {
        mScreenPosition.Set(
            Input.mousePosition.x, 
            Input.mousePosition.y, 
            Camera.main.transform.position.z);
        mWorldPosition = Camera.main.ScreenToWorldPoint(mScreenPosition);
    }
    #endregion
    
    #region Interface Methods
    /*
     * Returns the screen position of the mouse as a Vector3.
     * 
     * Returns:
     *      A Vector3 containing the screen position of the mouse. The Z
     *      coordinate will be the Z coordinate of the main camera.
     */
    public Vector3 inScreen()
    {
        return Utilities.copy(mScreenPosition);
    }

    /*
     * Returns the world position of the mouse as a Vector3.
     * 
     * Returns:
     *      A Vector3 containing the world position of the mouse. The Z
     *      coordinate will be the Z coordinate of the main camera.
     */
    public Vector3 inWorld()
    {
        return Utilities.copy(mWorldPosition);
    }

    /*
     * Returns a mouse button, which supports Press, Down, and Up events.
     * See Mouse.BUTTONS to see all supported mouse buttons.
     * 
     * Returns:
     *      The appropriate mouse button.
     */
    public Button button(BUTTONS mouseButton)
    {
        Button retVal = null;

        if (mLeft == null)
            Start();

        switch (mouseButton)
        {
            case BUTTONS.LEFT:
                retVal = mLeft;
                break;
            case BUTTONS.RIGHT:
                retVal = mRight;
                break;
            case BUTTONS.MIDDLE:
                retVal = mMiddle;
                break;
            default:
                return null;
        }

        if (retVal == null)
            return null;

        return retVal.clone();
    }
    #endregion
}
