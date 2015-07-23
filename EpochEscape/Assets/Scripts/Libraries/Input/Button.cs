using UnityEngine;
using System.Collections;

namespace Input
{
    /*
     * A Universal wrapper for all different types of unity buttons. This class
     * supports keys as buttons, mouse buttons as buttons, and unity defined
     * buttons as buttons.
     * 
     * The main purpose of this class is to allow easily customizable key bindings.
     */
    public class Button
    {
        #region Instance Variables
        bool mIsKey = false;
        bool mIsButton = false;
        bool mIsMouse = false;

        KeyCode mKey;
        string mButton = null;
        int mMouseButton = -1;
        #endregion

        /*
     * Initializes the Button to a specific key
     */
        public Button(KeyCode key)
        {
            setInput(key);
        }

        /*
         * Initializes the Button to a specific mouse button.
         * Valid values are 0 (left mouse button), 1 (right mouse button), and 2
         * (middle mouse button).
         */
        public Button(Mouse.BUTTONS mouseButton)
        {
            setInput(mouseButton);
        }

        /*
         * Initializes the Button to a specific Unity defined button.
         * Valid values are those set by the unity input manager.
         */
        public Button(string button)
        {
            setInput(button);
        }

        /*
         * A deep copy constructor. After copying, the two Buttons will be identical.
         */
        public Button(Button button)
        {
            mIsKey = button.mIsKey;
            mIsMouse = button.mIsMouse;
            mIsButton = button.mIsButton;

            mKey = button.mKey;
            mMouseButton = button.mMouseButton;
            mButton = button.mButton;
        }

        #region Interface Methods
        /*
     * Assigns the Button to a specific key
     */
        public bool setInput(KeyCode key)
        {
            mIsKey = true;
            mIsMouse = false;
            mIsButton = false;

            mKey = key;

            return true;
        }

        /*
         * Assigns the Button to a specific mouse button.
         * Valid values are 0 (left mouse button), 1 (right mouse button), and 2
         * (middle mouse button).
         */
        public bool setInput(Mouse.BUTTONS mouseButton)
        {
            mIsKey = false;
            mIsMouse = true;
            mIsButton = false;

            mMouseButton = (int)mouseButton;

            return true;
        }

        /*
         * Assigns the Button to a specific Unity defined button.
         * Valid values are those set by the unity input manager.
         */
        public bool setInput(string button)
        {
            if (button == null)
                return false;

            mIsKey = false;
            mIsMouse = false;
            mIsButton = true;

            mButton = button;

            return true;
        }

        /*
         * Returns true if the Button is being pressed.
         * 
         * Returns:
         *      Returns true if the Button is being pressed.
         */
        public bool get()
        {
            if (!(mIsKey || mIsMouse))
                return false;

            if (mIsKey)
                return UnityEngine.Input.GetKey(mKey);
            else if (mIsButton)
                return UnityEngine.Input.GetButton(mButton);
            else
                return UnityEngine.Input.GetMouseButton(mMouseButton);
        }

        /*
         * Returns true if the Button was pressed in the last frame.
         * 
         * Returns:
         *      Returns true if the Button was pressed in the last frame.
         */
        public bool getDown()
        {
            if (!(mIsKey || mIsMouse))
                return false;

            if (mIsKey)
                return UnityEngine.Input.GetKeyDown(mKey);
            else if (mIsButton)
                return UnityEngine.Input.GetButtonDown(mButton);
            else
                return UnityEngine.Input.GetMouseButtonDown(mMouseButton);
        }

        /*
         * Returns true if the Button was released in the last frame.
         * 
         * Returns:
         *      Returns true if the Button was released in the last frame.
         */
        public bool getUp()
        {
            if (!(mIsKey || mIsMouse))
                return false;

            if (mIsKey)
                return UnityEngine.Input.GetKeyUp(mKey);
            else if (mIsButton)
                return UnityEngine.Input.GetButtonUp(mButton);
            else
                return UnityEngine.Input.GetMouseButtonUp(mMouseButton);
        }

        /*
         * Returns the inputs associated with the button.
         * 
         * Returns:
         *      Returns the inputs associated with the button.
         */
        public override string ToString()
        {
            if (mIsKey)
                return "Button: " + mKey;
            else if (mIsButton)
                return "Button: " + mButton;
            else if (mIsMouse)
            {
                switch (mMouseButton)
                {
                    case 0:
                        return "Button: Left Mouse Button";
                    case 1:
                        return "Button: Right Mouse Button";
                    case 2:
                        return "Button: Middle Mouse Button";
                    default:
                        return "Button: Not Assigned";
                }
            }
            else
                return "Button: Not Assigned";
        }

        /*
         * Returns a complete deep copy of the Button
         * 
         * Returns:
         *      Returns a complete deep copy of the Button
         */
        public Button clone()
        {
            return new Button(this);
        }
        #endregion
    }
}
