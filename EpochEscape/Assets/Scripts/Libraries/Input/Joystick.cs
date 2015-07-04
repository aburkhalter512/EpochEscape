using UnityEngine;
using System.Collections;

namespace Input
{
    /*
     * Wrapper class for two axes, allowing a single Vector2 to be returned from two
     * separate input axes.
     * 
     * The main purpose of this class is to allow custom keybindings for those players
     * who don't have access to a controller/joystick.
     */
    public class Joystick
    {
        #region Instance Variables
        Axis mHorizontal = null;
        Axis mVertical = null;
        #endregion

        public Joystick(Axis horizontal, Axis vertical)
        {
            mHorizontal = horizontal;
            mVertical = vertical;
        }

        #region Interface Methods
        /*
     * Sets horizontal axis of the joystick.
     * 
     * Returns:
     *      Returns true if the axis was set.
     *      
     * Preconditions:
     *      'horizontal' must not be null
     */
        bool setHorizontalAxis(Axis horizontal)
        {
            if (horizontal == null)
                return false;

            mHorizontal = horizontal;

            return true;
        }

        /*
         * Sets vertical axis of the joystick.
         * 
         * Returns:
         *      Returns true if the axis was set.
         *      
         * Preconditions:
         *      'vertical' must not be null
         */
        bool setVerticalAxis(Axis vertical)
        {
            if (vertical == null)
                return false;

            mVertical = vertical;

            return true;
        }

        /*
         * Returns the smoothed value of the joystick. Valid values are between
         * -1 to 1.
         * 
         * Returns:
         *      Returns the smoothed value of the joystick.
         */
        public Vector2 get()
        {
            if (mHorizontal == null || mVertical == null)
                return Vector2.zero;

            return new Vector2(mHorizontal.get(), mVertical.get());
        }

        /*
         * Returns the unsmoothed value of the joystick. Valid values are between
         * -1 to 1.
         * 
         * Returns:
         *      Returns the unsmoothed value of the joystick.
         */
        public Vector2 getRaw()
        {
            if (mHorizontal == null || mVertical == null)
                return Vector2.zero;

            return new Vector2(mHorizontal.getRaw(), mVertical.getRaw());
        }

        /*
         * Returns the values of the axes associated with the joystick.
         * 
         * Returns:
         *      Returns the name of the axes associated with the joystick.
         */
        public override string ToString()
        {
            if (mHorizontal == null || mVertical == null)
                return "Not Assigned";

            return "Horizontal " + mHorizontal + "\nVertical " + mVertical;
        }
        #endregion
    }
}
