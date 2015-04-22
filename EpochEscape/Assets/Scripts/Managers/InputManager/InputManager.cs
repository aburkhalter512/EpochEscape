using UnityEngine;
using System.Collections.Generic;

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
	
	// Player buttons
    public Button actionButton = null;
    public Button specialActionButton = null;
    public Button interactButton = null;
	
	// Map editor buttons
    public Button primaryPlace = null;
	
    public List<Button> toolSelection;
    public Button toolChanger;

    public Button alternateInput = null;
    public Button auxillaryInput = null;

    public Button cancelInput = null;
	
	// Camera Buttons
    public Button cameraMoveButton = null;
    public Joystick cameraJoystick = null;
    public Axis cameraZoom = null;
    public Button cameraZoomLeftModifier = null;
    public Button cameraZoomRightModifier = null;

    public Button editorSaveButton = null;
	
    public Mouse mouse = null;
    #endregion

    protected override void Awaken()
    {
        toolSelection = new List<Button>();
    }
	
    //Put all initialization code here
    //Remember to comment!
    protected override void Initialize()
    {
        primaryJoystick = new Joystick(
            new Axis(new Button(KeyCode.LeftArrow), new Button(KeyCode.RightArrow)),
            new Axis(new Button(KeyCode.DownArrow), new Button(KeyCode.UpArrow)));

        mouse = Mouse.Get();

        actionButton = new Button(KeyCode.Space);

        specialActionButton = new Button(KeyCode.A);

        interactButton = new Button(KeyCode.Return);
		
		// Map editor buttons
        toolSelection.Add(new Button(KeyCode.Alpha1));
        toolSelection.Add(new Button(KeyCode.Alpha2));
        toolSelection.Add(new Button(KeyCode.Alpha3));
        toolSelection.Add(new Button(KeyCode.Alpha4));
        toolSelection.Add(new Button(KeyCode.Alpha5));
        toolSelection.Add(new Button(KeyCode.Alpha6));
        toolSelection.Add(new Button(KeyCode.Alpha7));
        toolSelection.Add(new Button(KeyCode.Alpha8));
        toolSelection.Add(new Button(KeyCode.Alpha9));
        toolSelection.Add(new Button(KeyCode.Alpha0));

        toolChanger = new Button(KeyCode.T);

        primaryPlace = new Button(Mouse.BUTTONS.LEFT);

        alternateInput = new Button(KeyCode.LeftAlt);
        auxillaryInput = new Button(KeyCode.LeftControl);

        cancelInput = new Button(KeyCode.Escape);
		
        cameraMoveButton = new Button(mouse.button(Mouse.BUTTONS.RIGHT));
        cameraJoystick = new Joystick(new Axis("Mouse X"), new Axis("Mouse Y"));
        cameraZoom = new Axis(new Button(KeyCode.Z), new Button(KeyCode.X));
        cameraZoomLeftModifier = new Button(KeyCode.LeftAlt);
        cameraZoomRightModifier = new Button(KeyCode.RightAlt);

        editorSaveButton = new Button(KeyCode.S);
    }
}
