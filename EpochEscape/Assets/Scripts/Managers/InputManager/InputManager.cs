﻿using UnityEngine;
using System.Collections;
using System.Xml.Linq;

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
    public Button actionButton = null;
    public Button specialActionButton = null;
    public Button interactButton = null;
    public Button mapButton = null;
    public Button[] itemButtons = null;
    public Mouse mouse = null;
    #endregion

    //Put all initialization code here
    //Remember to comment!
    protected override void Initialize()
    {
        primaryJoystick = new Joystick(
            new Axis(new Button(KeyCode.LeftArrow), new Button(KeyCode.RightArrow)),
            new Axis(new Button(KeyCode.DownArrow), new Button(KeyCode.UpArrow)));

        itemButtons = new Button[10];
        if (itemButtons != null)
        {
            itemButtons[0] = new Button(KeyCode.Alpha1);
            itemButtons[1] = new Button(KeyCode.Alpha2);
            itemButtons[2] = new Button(KeyCode.Alpha3);
            itemButtons[3] = new Button(KeyCode.Alpha4);
            itemButtons[4] = new Button(KeyCode.Alpha5);
            itemButtons[5] = new Button(KeyCode.Alpha6);
            itemButtons[6] = new Button(KeyCode.Alpha7);
            itemButtons[7] = new Button(KeyCode.Alpha8);
            itemButtons[8] = new Button(KeyCode.Alpha9);
            itemButtons[9] = new Button(KeyCode.Alpha0);
        }

        mouse = Mouse.Get();

        actionButton = new Button(KeyCode.Space);

        specialActionButton = new Button(KeyCode.A);

        interactButton = new Button(KeyCode.Return);

        mapButton = new Button(KeyCode.M);
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
