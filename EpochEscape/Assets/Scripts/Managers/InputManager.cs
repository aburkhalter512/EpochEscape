using UnityEngine;
using System.Collections;

/*
 * This class is the central location for all key bindings. It is made
 * for manual customization, thus has skupport for user defined key
 * bindings.
 */
public class InputManager : UnitySingleton<InputManager>
{
    #region Inspector Variables
    public KeyCode[] exitCodes = { KeyCode.Escape };

    //Modifying the following input classes allows custom key bindings to work
    public Joystick primaryJoystick = null;
    public Button actionButton = null;
    public Button specialActionButton = null;
    public Button[] itemButtons = null;
    public Axis itemSwitcher = null;
    public Mouse mouse = null;
	#endregion

    #region Instance Variables
    Vector2 mPrimaryJoystick = Vector2.zero;
    public Vector3 mMouseInScreen = Vector3.zero;
    public Vector3 mMouseInWorld = Vector3.zero;
	#endregion

    //Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
        primaryJoystick = new Joystick(
            new Axis(new Button(KeyCode.LeftArrow), new Button(KeyCode.RightArrow)),
            new Axis(new Button(KeyCode.DownArrow), new Button(KeyCode.UpArrow)));

        itemButtons = new Button[10];
        itemButtons[0].setInput(KeyCode.Alpha1);
        itemButtons[1].setInput(KeyCode.Alpha2);
        itemButtons[2].setInput(KeyCode.Alpha3);
        itemButtons[3].setInput(KeyCode.Alpha4);
        itemButtons[4].setInput(KeyCode.Alpha5);
        itemButtons[5].setInput(KeyCode.Alpha6);
        itemButtons[6].setInput(KeyCode.Alpha7);
        itemButtons[7].setInput(KeyCode.Alpha8);
        itemButtons[8].setInput(KeyCode.Alpha9);
        itemButtons[9].setInput(KeyCode.Alpha0);

        itemSwitcher = new Axis("Mouse ScrollWheel");

        mouse = Mouse.getInstance();

        actionButton = mouse.button(Mouse.BUTTONS.LEFT);

        specialActionButton = mouse.button(Mouse.BUTTONS.RIGHT);
	}

    #region Inteface Methods
    /*
     * Returns true if any of the exit key codes are being pressed
     * 
     * Returns:
     *      Returns true if any of exit key codes are being pressed.
     */
    public bool wantsExit()
    {
        foreach (KeyCode exitCode in exitCodes)
            if (Input.GetKey(exitCode))
                return true;

        return false;
    }
    #endregion
}
