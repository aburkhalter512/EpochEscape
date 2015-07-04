using UnityEngine;
using System.Collections.Generic;
using Input;

namespace Game
{
    /*
     * This class is the central location for all key bindings. It is made
     * for manual customization, thus has skupport for user defined key
     * bindings.
     */
    public class InputManager : Manager<InputManager>
    {
        #region Inspector Variables
        public KeyCode[] exitCodes = { KeyCode.Escape };

        //Modifying the following input classes allows custom key bindings to work
        public Joystick primaryJoystick = null;
        public Joystick secondaryJoystick = null;

        // Player buttons
        public Button actionButton = null;
        public Button specialActionButton = null;
        public Button interactButton = null;

        public Mouse mouse = null;

        public float repeatSpeed;
        #endregion

        protected override void Awaken()
        {
            repeatSpeed = 0.33f;

            primaryJoystick = new Joystick(
                new Axis(new Button(KeyCode.LeftArrow), new Button(KeyCode.RightArrow)),
                new Axis(new Button(KeyCode.DownArrow), new Button(KeyCode.UpArrow)));
            secondaryJoystick = new Joystick(
                new Axis(new Button(KeyCode.A), new Button(KeyCode.D)),
                new Axis(new Button(KeyCode.S), new Button(KeyCode.W)));

            mouse = Mouse.Get();

            actionButton = new Button(KeyCode.Space);

            specialActionButton = new Button(KeyCode.A);

            interactButton = new Button(KeyCode.Return);
        }

        //Put all initialization code here
        //Remember to comment!
        protected override void Initialize()
        {
        }
    }
}
