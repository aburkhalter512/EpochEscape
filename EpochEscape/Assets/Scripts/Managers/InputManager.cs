using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    #region Inspector Variables
    static KeyCode[] exitCodes = { KeyCode.Escape };
	#endregion

	#region Instance Variables
	#endregion

	#region Class Constants
	#endregion

    #region Class Variables
    static Vector2 primaryJoystick = Vector2.zero;
    #endregion

    //Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
        UpdateKeyboard();
	}

	#region Update Methods
    static void UpdateKeyboard()
    {
        primaryJoystick.x = Input.GetAxis("Horizontal");
        primaryJoystick.y = Input.GetAxis("Vertical");
    }

    public static Vector2 getPrimaryJoystick()
    {
        return Utilities.copy(primaryJoystick);
    }

    public static bool wantsExit()
    {
        foreach (KeyCode exitCode in exitCodes)
            if (Input.GetKey(exitCode))
                return true;

        return false;
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
