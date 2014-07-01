using UnityEngine;
using System.Collections;

/*
 * A class that wraps a bit of custom functionality into Unity's Axis System.
 * It supports Unity Axes as well buttons acting like axes.
 * 
 * Buttons act like an axis in this class by assigning a 'negative' button
 * and a 'positive' button. When the 'negative' button is pressed, a -1 is
 * returned, and when the 'positive' is pressed, a 1 is returned.
 */
public class Axis
{
	#region Instance Variables
    //Two bools for error checking
    bool mIsAxis;
    bool mIsKeys;

    string mAxis;

    Button mNegative;
    Button mPositive;
	#endregion

    /*
     * Initializes the axis to a pre-existing Unity Input Axis
     * 
     * Precondition:
     *      'axis' must be a valid unity axis
     */
    public Axis(string axis)
    {
        setInput(axis);
    }

    /*
     * Initializes the axis to a pair of keys
     * 
     * Precondition:
     *      'negative' and 'postive' must not be the same key
     */
    public Axis(Button negative, Button postive)
    {
        setInput(negative, postive);
    }

    #region Interface Methods
    /*
     * Sets the axis to a pre-existing Unity Input Axis
     * 
     * Returns:
     *      Returns true if the axis was set, false if not
     * 
     * Precondition:
     *      'axis' must be a valid unity axis
     */
    public bool setInput(string axis)
    {
        //Argument checking
        if (axis == null)
            return false;

        bool foundAxis = false;
        string[] axes = Input.GetJoystickNames();

        for (int i = 0; i < axes.Length; i++)
        {
            if (axis == axes[i])
            {
                foundAxis = true;
                break;
            }
        }

        if (!foundAxis)
            return false;

        mIsAxis = true;
        mIsKeys = false;

        mAxis = axis;

        return true;
    }

    /*
     * Sets the axis to a pair of keys
     * 
     * Precondition:
     *      'negative' and 'postive' must not be the same key
     */
    public bool setInput(Button negative, Button postive)
    {
        //Argument Checking
        if (negative == null || postive == null || negative == postive)
            return false;

        mIsAxis = false;
        mIsKeys = true;

        mNegative = negative;
        mPositive = postive;

        return true;
    }

    /*
     * Returns the unsmoothed value of the axis. Valid values are between -1 to 1.
     * 
     * Returns:
     *      Returns the unsmoothed value of the axis
     */
    public float getRaw()
    {
        if (!isAssigned())
            return 0;

        if (mIsAxis)
            return Input.GetAxisRaw(mAxis);
        else //Devil Magic! (Just checking to see if either key is pressed and adding the results)
            return (mNegative.get() ? -1 : 0) + (mPositive.get() ? 1 : 0);
    }

    /*
     * Returns the smoothed value of the axis. Valid values are between -1 to 1.
     * 
     * Returns:
     *      Returns the smoothed value of the axis
     */
    public float get()
    {
        if (!isAssigned())
            return 0;

        if (mIsAxis)
            return Input.GetAxis(mAxis);
        else //Devil Magic! (Just checking to see if either key is pressed and adding the results)
            return (mNegative.get() ? -1 : 0) + (mPositive.get() ? 1 : 0);
    }

    public bool isAssigned()
    {
        if (mIsAxis || mIsKeys)
            return true;
        else
            return false;
    }

    /*
     * Returns the inputs associated with the axis.
     * 
     * Returns:
     *      Returns the inputs associated with the axis.
     */
    public override string ToString()
    {
        if (!isAssigned())
            return "Not Assigned";

        if (mIsAxis)
            return mAxis;
        else
            return "Negative: " + mNegative + ", Positive: " + mPositive;
    }
    #endregion
}
