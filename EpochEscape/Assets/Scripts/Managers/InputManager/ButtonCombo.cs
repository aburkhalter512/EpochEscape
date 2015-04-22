using UnityEngine;
using System.Collections.Generic;

//A wrapper class for detecting button combinations presses, downs and ups.
//This class supports any number of buttons (but is obviously limited by the
//number of physical buttons available).
public class ButtonCombo
{
    #region Instance Variables
    List<Button> mButtons;
    #endregion

    /*
     * Initializes the ButtonCombo to an array of buttons
     */
    public ButtonCombo(Button[] buttons)
    {
        mButtons = new List<Button>();

        if (buttons != null)
        {
            //Copy only non-null elements
            for (int i = 0; i < buttons.Length; i++)
                if (buttons[i] != null)
                    mButtons.Add(buttons[i]);
        }
    }

    /*
     * Intializes the ButtonCombo as a copy of another ButtonCombo
     */
    public ButtonCombo(ButtonCombo combo)
    {
        mButtons = new List<Button>();
        mButtons.AddRange(combo.mButtons);
    }

    /*
     * Assigns the ButtonCombo to the array of Buttons. This method reinitializes
     * the ButtonCombo.
     * 
     * Returns:
     *      False if the array of buttons passed is null, true if not
     */
    public bool setInput(Button[] buttons)
    {
        if (buttons == null)
            return false;

        mButtons.Clear();

        if (buttons != null)
        {
            //Copy only non-null elements
            for (int i = 0; i < buttons.Length; i++)
                if (buttons[i] != null)
                    mButtons.Add(buttons[i]);
        }

        return true;
    }

    /*
     * Returns the current associated button to the ButtonCombo as an array.
     * 
     * Returns:
     *      Returns an array of Buttons specifying what Buttons the ButtonCombo
     *      References.
     */
    public Button[] getInput()
    {
        return mButtons.ToArray();
    }

    /*
     * Returns true if all of the Buttons in the combo are being pressed.
     * 
     * Returns:
     *      Returns true if all of the Buttons in the combo are being pressed.
     */
    public bool get()
    {
        //If any button is not pressed, return false
        foreach (Button button in mButtons)
            if (!button.get())
                return false;

        return true;
    }

    /*
     * Returns true if all of the Buttons in the combo are being held down and a button
     * was recently pressed. All of the buttons must be pressed at once or at least one
     * Button must be pressed while the others are being held down.
     * 
     * Returns:
     *      Returns true if all of the Buttons in the combo are being pressed and a button
     *      was recently pressed.
     */
    public bool getDown()
    {
        //If any button is not pressed or not one button has been recently pressed, return false
        bool hasDown = false;

        foreach (Button button in mButtons)
        {
            if (button.getDown())
                hasDown = true;
            else if (!button.get())
                return false;
        }

        return hasDown;
    }

    /*
     * Returns true if all of the Buttons in the combo are not being pressed and a button
     * was recently released. All of the buttons must be released at once or at least one
     * Button must be released while the others are not being held down.
     * 
     * Returns:
     *      Returns true if all of the Buttons in the combo are being pressed and a button
     *      was recently pressed.
     */
    public bool getUp()
    {
        //If any button is pressed or not one button has been recently released, return false
        bool hasUp = false;

        foreach (Button button in mButtons)
        {
            if (button.getUp())
                hasUp = true;
            else if (button.get())
                return false;
        }

        return hasUp;
    }
}
