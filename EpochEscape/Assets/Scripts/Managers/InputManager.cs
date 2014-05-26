using UnityEngine;
using System.Collections;

public class InputManager : UnitySingleton<InputManager>
{
    #region Inspector Variables
    public KeyCode[] exitCodes = { KeyCode.Escape };
    public Vector2 primaryJoystick
    {
        get
        {
            return mPrimaryJoystick;
        }
    }

    public Vector3 mouseInScreen
    {
        get
        {
            return mMouseInScreen;
        }
    }

    public Vector3 mouseInWorld
    {
        get
        {
            return mMouseInWorld;
        }
    }
	#endregion

    #region Instance Variables
    Vector2 mPrimaryJoystick = Vector2.zero;
    public Vector3 mMouseInScreen = Vector3.zero;
    public Vector3 mMouseInWorld = Vector3.zero;
	#endregion

	#region Class Constants
	#endregion

    #region Class Variables
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
        UpdateMouse();
	}

	#region Update Methods
    void UpdateKeyboard()
    {
        mPrimaryJoystick.x = Input.GetAxis("Horizontal");
        mPrimaryJoystick.y = Input.GetAxis("Vertical");
    }

    void UpdateMouse()
    {
        mMouseInScreen.Set(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z);
        mMouseInWorld = Camera.main.ScreenToWorldPoint(mMouseInScreen);
    }

    public bool wantsExit()
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
