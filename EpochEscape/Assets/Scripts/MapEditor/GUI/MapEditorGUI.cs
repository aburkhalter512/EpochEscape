using GUI;
using Utilities;

using System;

using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

namespace MapEditor
{
    class MapEditorGUI
    {
		private GUIHorizontalDialog _topPanel;
		private GUIObject _leftPanel;
    	private GUIVerticalDialog _rightPanel;
    	private static ResourceManager.ResourceManager _rm = ResourceManager.ResourceManager.Get();

    	private static readonly float GUI_PADDING = 8;
    	private static readonly Vector2 GUI_DOOR_BUTTON_SIZE = new Vector2(92, 40);
    	private static readonly Vector2 GUI_ACTIVATOR_BUTTON_SIZE = new Vector2(40, 40);
    	private static readonly float GUI_RIGHT_PANEL_WIDTH = 200;

    	public MapEditorGUI()
    	{
			Color backgroundColor = new Color(.3f, .3f, 1.0f, .3f);
			Color lightBackgroundColor = new Color(.6f, .6f, 1.0f, .3f);

			ObjectManipulator.Get().activate(); // The default tool;

    		_leftPanel = new GUIObject((obj) =>
    		{
    			_leftPanel.name("Left Panel");

    			_leftPanel.addListener(EventTriggerType.PointerEnter, (e) =>
    			{
    				disableTools();
				});
    			_leftPanel.addListener(EventTriggerType.PointerExit, (e) =>
    			{
    				enableTools();
    			});

    			_leftPanel.anchor(new Vector2(0.0f, 1.0f));
    			_leftPanel.position(new Vector2(28, -150), GUIObject.POSITION_TYPE.RELATIVE);
    			_leftPanel.size(new Vector2(56, 300));

    			// Background Image
    			new GUIImage((image) =>
    			{
    				GUIImage background = image as GUIImage;

    				_leftPanel.addChild(background);

    				background.position(Vector2.zero);
    				background.size(_leftPanel.size());
					background.imageColor(backgroundColor);
    			});

    			Vector2 buttonSize = new Vector2(40, 40);
    			// Play Button
    			new GUIButton((button) =>
    			{
    				GUIButton playButton = button as GUIButton;

    				_leftPanel.addChild(button);

    				playButton.anchor(new Vector2(0.5f, 0.0f));
    				playButton.position(new Vector2(0, 254), GUIObject.POSITION_TYPE.RELATIVE);
					playButton.size(buttonSize);
					playButton.setBaseBackground(_rm.sprite("Textures/GUI/Buttons/Play"), Color.white);
					playButton.setOverBackground(_rm.sprite("Textures/GUI/Buttons/PlayHover"), Color.white);
					playButton.setDownBackground(_rm.sprite("Textures/GUI/Buttons/PlayHover"), Color.white);
				});

				// Save Button
    			new GUIButton((button) =>
				{
    				GUIButton saveButton = button as GUIButton;

					_leftPanel.addChild(button);

					saveButton.anchor(new Vector2(0.5f, 0.0f));
					saveButton.position(new Vector2(0, 188), GUIObject.POSITION_TYPE.RELATIVE);
					saveButton.size(buttonSize);
					saveButton.setBaseBackground(_rm.sprite("Textures/GUI/Buttons/Save"), Color.white);
					saveButton.setOverBackground(_rm.sprite("Textures/GUI/Buttons/SaveHover"), Color.white);
					saveButton.setDownBackground(_rm.sprite("Textures/GUI/Buttons/SaveHover"), Color.white);

					saveButton.addListener(EventTriggerType.PointerClick, (e) =>
					{
						string levelPath = EditorUtility.SaveFilePanel("New Level", "Levels", "Epoch Level", "");

						if (levelPath == "")
							return;

						SaveManager.Get().saveLevel(levelPath);
					});
    			});

    			// Open Button
    			new GUIButton((button) =>
    			{
					GUIButton openButton = button as GUIButton;
					
					_leftPanel.addChild(button);

					openButton.anchor(new Vector2(0.5f, 0.0f));
					openButton.position(new Vector2(0, 116), GUIObject.POSITION_TYPE.RELATIVE);
					openButton.size(buttonSize);
					openButton.setBaseBackground(_rm.sprite("Textures/GUI/Buttons/Open"), Color.white);
					openButton.setOverBackground(_rm.sprite("Textures/GUI/Buttons/OpenHover"), Color.white);
					openButton.setDownBackground(_rm.sprite("Textures/GUI/Buttons/OpenHover"), Color.white);

					openButton.addListener(EventTriggerType.PointerClick, (e) =>
					{	
						string levelPath = EditorUtility.OpenFolderPanel("New Level", "Levels", "Epoch Level");

						if (levelPath == "")
							return;

						SaveManager.Get().loadLevel(levelPath);
					});
    			});

    			// Exit Button
    			new GUIButton((button) =>
    			{
					GUIButton exitButton = button as GUIButton;

					_leftPanel.addChild(button);

					exitButton.anchor(new Vector2(0.5f, 0.0f));
					exitButton.position(new Vector2(0, 48), GUIObject.POSITION_TYPE.RELATIVE);
					exitButton.size(buttonSize);
					exitButton.setBaseBackground(_rm.sprite("Textures/GUI/Buttons/Exit"), Color.white);
					exitButton.setOverBackground(_rm.sprite("Textures/GUI/Buttons/ExitHover"), Color.white);
					exitButton.setDownBackground(_rm.sprite("Textures/GUI/Buttons/ExitHover"), Color.white);

					exitButton.addListener(EventTriggerType.PointerClick, (e) =>
					{
						Debug.Log("Clicking exit");
						Driver.Get().progress(Driver.DRIVER_STATE.MAIN_MENU);
					});
    			});
    		});

    		_topPanel = new GUIHorizontalDialog((obj) =>
    		{
    			_topPanel.name("Top Panel");

    			_topPanel.addListener(EventTriggerType.PointerEnter, (e) =>
    			{
    				disableTools();
				});
    			_topPanel.addListener(EventTriggerType.PointerExit, (e) =>
    			{
    				enableTools();
    			});

				_topPanel.anchor(new Vector2(0.0f, 1.0f));
				_topPanel.position(new Vector2(MainCanvas.Get().canvasSize().x / 2 + 28, -28), GUIObject.POSITION_TYPE.RELATIVE);
				_topPanel.size(new Vector2(MainCanvas.Get().canvasSize().x - 56, 56));
				_topPanel.overflow(GUI.GUIContent.OVERFLOW_TYPE.HIDE);
				_topPanel.scrollButtonPosition(28);
				_topPanel.backgroundColor(lightBackgroundColor);
				_topPanel.contentWidth(1300.0f);

				float offset = 48 + GUI_PADDING + GUI_DOOR_BUTTON_SIZE.x / 2;

				string doorPath = "Textures/Activatables/Doors/Assemblies/";
				string pressurePadPath = "Textures/Actuators/Pressure Pads/";
				string staticWallPath = "Textures/Game Environment/Walls/";
				string tilePath = "Textures/Game Environment/";

				Input.InputListener.Get().addListener(
					new Input.Hotkey(new Input.Button(KeyCode.Escape)),
					(key) =>
					{
						resetTools();
					});

				ObjectManipulator.Get().onNewPlacement = (placeableObject) =>
				{
					#region Doors
					if (placeableObject as PlaceableStandardDoor != null)
					{
						updateRightPanel("Standard Door", _rm.sprite(doorPath + "StandardDoor"), GUI_DOOR_BUTTON_SIZE, (content) =>
						{
							create_doorInitiallyOpened((checkbox) =>
							{
								checkbox.position(new Vector2(0, - GUI_PADDING * 1 - GUI_DOOR_BUTTON_SIZE.y * 0.5f), GUIObject.POSITION_TYPE.RELATIVE);
							});
							create_rotationDropDown((dropDown) =>
							{
								dropDown.position(new Vector2(0, - GUI_PADDING * 3 - GUI_DOOR_BUTTON_SIZE.y * 1.5f), GUIObject.POSITION_TYPE.RELATIVE);
							});
						});
					}
					else if (placeableObject as PlaceableTeleporterDoor != null)
					{
						updateRightPanel("Teleporter Door", _rm.sprite(doorPath + "TeleporterDoor"), GUI_DOOR_BUTTON_SIZE, (content) =>
						{
							create_doorInitiallyOpened((checkbox) =>
							{
								checkbox.position(new Vector2(0, - GUI_PADDING * 1 - GUI_DOOR_BUTTON_SIZE.y * 0.5f), GUIObject.POSITION_TYPE.RELATIVE);
							});
							create_rotationDropDown((dropDown) =>
							{
								dropDown.position(new Vector2(0, - GUI_PADDING * 3 - GUI_DOOR_BUTTON_SIZE.y * 1.5f), GUIObject.POSITION_TYPE.RELATIVE);
							});
						});
					}
					else if (placeableObject as PlaceableCheckpointDoor != null)
					{
						updateRightPanel("Checkpoint Door", _rm.sprite(doorPath + "CheckpointDoor"), GUI_DOOR_BUTTON_SIZE, (content) =>
						{
							create_rotationDropDown((dropDown) =>
							{
								dropDown.position(new Vector2(0, - GUI_PADDING * 1 - GUI_DOOR_BUTTON_SIZE.y * 0.5f), GUIObject.POSITION_TYPE.RELATIVE);
							});
						});
					}
					else if (placeableObject as PlaceableDirectionalDoor != null)
					{
						updateRightPanel("One Way Door", _rm.sprite(doorPath + "OneWayDoor"), GUI_DOOR_BUTTON_SIZE, (content) =>
						{
							create_doorInitiallyOpened((checkbox) =>
							{
								checkbox.position(new Vector2(0, - GUI_PADDING * 1 - GUI_DOOR_BUTTON_SIZE.y * 0.5f), GUIObject.POSITION_TYPE.RELATIVE);
							});
							create_rotationDropDown((dropDown) =>
							{
								dropDown.position(new Vector2(0, - GUI_PADDING * 3 - GUI_DOOR_BUTTON_SIZE.y * 1.5f), GUIObject.POSITION_TYPE.RELATIVE);
							});
						});
					}
					else if (placeableObject as PlaceableEntranceDoor != null)
					{
						updateRightPanel("Entrance Door", _rm.sprite(doorPath + "EntranceDoor"), GUI_DOOR_BUTTON_SIZE, (content) =>
						{
							create_rotationDropDown((dropDown) =>
							{
								dropDown.position(new Vector2(0, - GUI_PADDING * 1 - GUI_DOOR_BUTTON_SIZE.y * 0.5f), GUIObject.POSITION_TYPE.RELATIVE);
							});
						});
					}
					#endregion
					// TODO: Power Core Door
					#region Pressure Pads
					else if (placeableObject as PlaceablePressureSwitch != null)
					{
						updateRightPanel("Pressure Switch", _rm.sprite(pressurePadPath + "On Pressure Switch"), GUI_ACTIVATOR_BUTTON_SIZE, (content) =>
						{ });
					}
					else if (placeableObject as PlaceablePressurePlate != null)
					{
						updateRightPanel("Pressure Plate", _rm.sprite(pressurePadPath + "Toggle A Pressure Plate"), GUI_ACTIVATOR_BUTTON_SIZE, (content) =>
						{ });
					}
						#endregion
					else if (placeableObject as PlaceableStaticWall != null)
					{
						updateRightPanel("Static Wall", _rm.sprite(staticWallPath + "SingleUnitSprite"), GUI_ACTIVATOR_BUTTON_SIZE, (content) =>
						{ });
					}
				};
				ObjectManipulator.Get().onSelect = (placeableObject) =>
				{
					#region Doors
					if (placeableObject as PlaceableStandardDoor != null)
					{
						updateRightPanel("Standard Door", _rm.sprite(doorPath + "StandardDoor"), GUI_DOOR_BUTTON_SIZE, (content) =>
						{
							create_doorInitiallyOpened((checkbox) =>
							{
								checkbox.position(new Vector2(0, - GUI_PADDING * 1 - GUI_DOOR_BUTTON_SIZE.y * 0.5f), GUIObject.POSITION_TYPE.RELATIVE);
							});
						});
					}
					else if (placeableObject as PlaceableTeleporterDoor != null)
					{
						updateRightPanel("Teleporter Door", _rm.sprite(doorPath + "TeleporterDoor"), GUI_DOOR_BUTTON_SIZE, (content) =>
						{
							create_doorInitiallyOpened((checkbox) =>
							{
								checkbox.position(new Vector2(0, - GUI_PADDING * 1 - GUI_DOOR_BUTTON_SIZE.y * 0.5f), GUIObject.POSITION_TYPE.RELATIVE);
							});
						});
					}
					else if (placeableObject as PlaceableCheckpointDoor != null)
					{
						updateRightPanel("Checkpoint Door", _rm.sprite(doorPath + "CheckpointDoor"), GUI_DOOR_BUTTON_SIZE, (content) =>
						{ });
					}
					else if (placeableObject as PlaceableDirectionalDoor != null)
					{
						updateRightPanel("One Way Door", _rm.sprite(doorPath + "OneWayDoor"), GUI_DOOR_BUTTON_SIZE, (content) =>
						{
							create_doorInitiallyOpened((checkbox) =>
							{
								checkbox.position(new Vector2(0, - GUI_PADDING * 1 - GUI_DOOR_BUTTON_SIZE.y * 0.5f), GUIObject.POSITION_TYPE.RELATIVE);
							});
						});
					}
					else if (placeableObject as PlaceableEntranceDoor != null)
					{
						updateRightPanel("Entrance Door", _rm.sprite(doorPath + "EntranceDoor"), GUI_DOOR_BUTTON_SIZE, (content) =>
						{ });
					}
					#endregion
					#region Pressure Pads
					else if (placeableObject as PlaceablePressureSwitch != null)
					{
						updateRightPanel("Pressure Switch", _rm.sprite(pressurePadPath + "On Pressure Switch"), GUI_ACTIVATOR_BUTTON_SIZE, (content) =>
						{ });
					}
					else if (placeableObject as PlaceablePressurePlate != null)
					{
						updateRightPanel("Pressure Plate", _rm.sprite(pressurePadPath + "Toggle A Pressure Plate"), GUI_ACTIVATOR_BUTTON_SIZE, (content) =>
						{ });
					}
					#endregion
					else if (placeableObject as PlaceableStaticWall != null)
					{
						updateRightPanel("Static Wall", _rm.sprite(staticWallPath + "SingleUnitSprite"), GUI_ACTIVATOR_BUTTON_SIZE, (content) =>
						{ });
					}
				};
				ObjectManipulator.Get().onCancelPlacement = () =>
				{
					emptyRightPanel();
				};
				ObjectManipulator.Get().onCancelSelect = () =>
				{
					emptyRightPanel();
				};

				#region Door Selectors
				// Standard Door Selector
				new GUIButton((_button) =>
				{
					_topPanel.addChild(_button);
					GUIButton button = _button as GUIButton;

					button.name("Standard Door Selector");

					button.anchor(new Vector2(0.0f, 0.5f));
					button.position(
						new Vector2((GUI_PADDING * 2 + GUI_DOOR_BUTTON_SIZE.x) * 0 + offset, 0.0f),
						GUIObject.POSITION_TYPE.RELATIVE);
					button.size(GUI_DOOR_BUTTON_SIZE);
					button.setBaseBackground(_rm.sprite(doorPath + "StandardDoor"), Color.white);
					button.setOverBackground(_rm.sprite(doorPath + "StandardDoor"), Color.blue);
					button.setDownBackground(_rm.sprite(doorPath + "StandardDoor"), Color.black);

					button.addListener(EventTriggerType.PointerClick, (e) =>
					{
						resetTools();

						ObjectManipulator.Get().setObjectPrefab(PlaceableStandardDoor.getPrefab());
						ObjectManipulator.Get().activate();
					});
				});

				// One Way Door Selector
				new GUIButton((_button) =>
				{
					_topPanel.addChild(_button);
					GUIButton button = _button as GUIButton;

					button.name("One Way Door Selector");

					button.anchor(new Vector2(0.0f, 0.5f));
					button.position(
						new Vector2((GUI_PADDING * 2 + GUI_DOOR_BUTTON_SIZE.x) * 1 + offset, 0.0f),
						GUIObject.POSITION_TYPE.RELATIVE);
					button.size(GUI_DOOR_BUTTON_SIZE);
					button.setBaseBackground(_rm.sprite(doorPath + "OneWayDoor"), Color.white);
					button.setOverBackground(_rm.sprite(doorPath + "OneWayDoor"), Color.blue);
					button.setDownBackground(_rm.sprite(doorPath + "OneWayDoor"), Color.black);

					button.addListener(EventTriggerType.PointerClick, (e) =>
					{
						resetTools();

						ObjectManipulator.Get().setObjectPrefab(PlaceableDirectionalDoor.getPrefab());
						ObjectManipulator.Get().activate();
					});
				});

				// Teleporter Door Selector
				new GUIButton((_button) =>
				{
					_topPanel.addChild(_button);
					GUIButton button = _button as GUIButton;

					button.name("Teleporter Door Selector");

					button.anchor(new Vector2(0.0f, 0.5f));
					button.position(
						new Vector2((GUI_PADDING * 2 + GUI_DOOR_BUTTON_SIZE.x) * 2 + offset, 0.0f),
						GUIObject.POSITION_TYPE.RELATIVE);
					button.size(GUI_DOOR_BUTTON_SIZE);
					button.setBaseBackground(_rm.sprite(doorPath + "TeleporterDoor"), Color.white);
					button.setOverBackground(_rm.sprite(doorPath + "TeleporterDoor"), Color.blue);
					button.setDownBackground(_rm.sprite(doorPath + "TeleporterDoor"), Color.black);

					button.addListener(EventTriggerType.PointerClick, (e) =>
					{
						resetTools();

						ObjectManipulator.Get().setObjectPrefab(PlaceableTeleporterDoor.getPrefab());
						ObjectManipulator.Get().activate();
					});
				});

				// Checkpoint Door Selector
				new GUIButton((_button) =>
				{
					_topPanel.addChild(_button);
					GUIButton button = _button as GUIButton;

					button.name("Checkpoint Door Selector");

					button.anchor(new Vector2(0.0f, 0.5f));
					button.position(
						new Vector2((GUI_PADDING * 2 + GUI_DOOR_BUTTON_SIZE.x) * 3 + offset, 0.0f),
						GUIObject.POSITION_TYPE.RELATIVE);
					button.size(GUI_DOOR_BUTTON_SIZE);
					button.setBaseBackground(_rm.sprite(doorPath + "CheckpointDoor"), Color.white);
					button.setOverBackground(_rm.sprite(doorPath + "CheckpointDoor"), Color.blue);
					button.setDownBackground(_rm.sprite(doorPath + "CheckpointDoor"), Color.black);

					button.addListener(EventTriggerType.PointerClick, (e) =>
					{
						resetTools();

						ObjectManipulator.Get().setObjectPrefab(PlaceableCheckpointDoor.getPrefab());
						ObjectManipulator.Get().activate();
					});
				});

				/*// Power Core Door Selector
				new GUIButton((_button) =>
				{
					_topPanel.addChild(_button);
					GUIButton button = _button as GUIButton;

					button.name("Power Core Door Selector");

					button.anchor(new Vector2(0.0f, 0.5f));
					button.position(
						new Vector2((GUI_PADDING * 2 + GUI_DOOR_BUTTON_SIZE.x) * 4 + offset, 0.0f),
						GUIObject.POSITION_TYPE.RELATIVE);
					button.size(GUI_DOOR_BUTTON_SIZE);
					button.setBaseBackground(_rm.sprite(doorPath + "FullCoreDoor"), Color.white);
					button.setOverBackground(_rm.sprite(doorPath + "FullCoreDoor"), Color.blue);
					button.setDownBackground(_rm.sprite(doorPath + "FullCoreDore"), Color.black);

				});*/

				// Entrance Door Selector
				new GUIButton((_button) =>
				{
					_topPanel.addChild(_button);
					GUIButton button = _button as GUIButton;

					button.name("Power Core Door Selector");

					button.anchor(new Vector2(0.0f, 0.5f));
					button.position(
						new Vector2((GUI_PADDING * 2 + GUI_DOOR_BUTTON_SIZE.x) * 5 + offset, 0.0f),
						GUIObject.POSITION_TYPE.RELATIVE);
					button.size(GUI_DOOR_BUTTON_SIZE);
					button.setBaseBackground(_rm.sprite(doorPath + "EntranceDoor"), Color.white);
					button.setOverBackground(_rm.sprite(doorPath + "EntranceDoor"), Color.blue);
					button.setDownBackground(_rm.sprite(doorPath + "EntranceDoor"), Color.black);

					button.addListener(EventTriggerType.PointerClick, (e) =>
					{
						resetTools();

						ObjectManipulator.Get().setObjectPrefab(PlaceableEntranceDoor.getPrefab());
						ObjectManipulator.Get().activate();
					});
				});
				#endregion

				#region Pressure Pad Selectors
				// Pressure Switch
				new GUIButton((_button) =>
				{
					_topPanel.addChild(_button);
					GUIButton button = _button as GUIButton;

					button.name("Pressure Switch Selector");

					button.anchor(new Vector2(0.0f, 0.5f));
					button.position(
						new Vector2((GUI_PADDING * 2 + GUI_DOOR_BUTTON_SIZE.x) * 6 + offset, 0.0f),
						GUIObject.POSITION_TYPE.RELATIVE);
					button.size(GUI_ACTIVATOR_BUTTON_SIZE);
					button.setBaseBackground(_rm.sprite(pressurePadPath + "On Pressure Switch"), Color.white);
					button.setOverBackground(_rm.sprite(pressurePadPath + "On Pressure Switch"), Color.blue);
					button.setDownBackground(_rm.sprite(pressurePadPath + "On Pressure Switch"), Color.black);

					button.addListener(EventTriggerType.PointerClick, (e) =>
					{
						resetTools();

						ObjectManipulator.Get().setObjectPrefab(PlaceablePressureSwitch.getPrefab());
						ObjectManipulator.Get().activate();
					});
				});

				// Pressure Pad
				new GUIButton((_button) =>
				{
					_topPanel.addChild(_button);
					GUIButton button = _button as GUIButton;

					button.name("Pressure Pad Selector");

					button.anchor(new Vector2(0.0f, 0.5f));
					button.position(
						new Vector2((GUI_PADDING * 2 + GUI_DOOR_BUTTON_SIZE.x) * 7 + offset, 0.0f),
						GUIObject.POSITION_TYPE.RELATIVE);
					button.size(GUI_ACTIVATOR_BUTTON_SIZE);
					button.setBaseBackground(_rm.sprite(pressurePadPath + "Toggle A Pressure Plate"), Color.white);
					button.setOverBackground(_rm.sprite(pressurePadPath + "Toggle A Pressure Plate"), Color.blue);
					button.setDownBackground(_rm.sprite(pressurePadPath + "Toggle A Pressure Plate"), Color.black);

					button.addListener(EventTriggerType.PointerClick, (e) =>
					{
						resetTools();

						ObjectManipulator.Get().setObjectPrefab(PlaceablePressurePlate.getPrefab());
						ObjectManipulator.Get().activate();
					});
				});
					#endregion

				// Static Wall
				new GUIButton((_button) =>
				{
					_topPanel.addChild(_button);
					GUIButton button = _button as GUIButton;

					button.name("Static Wall Selector");

					button.anchor(new Vector2(0.0f, 0.5f));
					button.position(
						new Vector2((GUI_PADDING * 2 + GUI_DOOR_BUTTON_SIZE.x) * 8 + offset, 0.0f),
						GUIObject.POSITION_TYPE.RELATIVE);
					button.size(GUI_ACTIVATOR_BUTTON_SIZE);
					button.setBaseBackground(_rm.sprite(staticWallPath + "SingleUnitSprite"), Color.white);
					button.setOverBackground(_rm.sprite(staticWallPath + "SingleUnitSprite"), Color.blue);
					button.setDownBackground(_rm.sprite(staticWallPath + "SingleUnitSprite"), Color.black);

					button.addListener(EventTriggerType.PointerClick, (e) =>
					{
						resetTools();

						ObjectManipulator.Get().setObjectPrefab(PlaceableStaticWall.getPrefab());
						ObjectManipulator.Get().activate();
					});
				});

				// Tile Creator
				new GUIButton((_button) =>
				{
					_topPanel.addChild(_button);
					GUIButton button = _button as GUIButton;

					button.name("Tile Selector");

					button.anchor(new Vector2(0.0f, 0.5f));
					button.position(
						new Vector2((GUI_PADDING * 2 + GUI_DOOR_BUTTON_SIZE.x) * 9 + offset, 0.0f),
						GUIObject.POSITION_TYPE.RELATIVE);
					button.size(GUI_ACTIVATOR_BUTTON_SIZE);
					button.setBaseBackground(_rm.sprite(tilePath + "Tile"), Color.white);
					button.setOverBackground(_rm.sprite(tilePath + "Tile"), Color.blue);
					button.setDownBackground(_rm.sprite(tilePath + "Tile"), Color.black);

					button.addListener(EventTriggerType.PointerClick, (e) =>
					{
						resetTools();

						TileManipulator.Get().activate();
					});
				});

				// Tile Eraser
				new GUIButton((_button) =>
				{
					_topPanel.addChild(_button);
					GUIButton button = _button as GUIButton;

					button.name("Tile Selector");

					button.anchor(new Vector2(0.0f, 0.5f));
					button.position(
						new Vector2((GUI_PADDING * 2 + GUI_DOOR_BUTTON_SIZE.x) * 10 + offset, 0.0f),
						GUIObject.POSITION_TYPE.RELATIVE);
					button.size(GUI_ACTIVATOR_BUTTON_SIZE);
					button.setBaseBackground(_rm.sprite(tilePath + "TileEraser"), Color.white);
					button.setOverBackground(_rm.sprite(tilePath + "TileEraser"), Color.blue);
					button.setDownBackground(_rm.sprite(tilePath + "TileEraser"), Color.black);

					button.addListener(EventTriggerType.PointerClick, (e) =>
					{
						resetTools();

						TileEraser.Get().activate();
					});
				});
    		});

    		_rightPanel = new GUIVerticalDialog((_dialog) =>
    		{
				_rightPanel.name("Right Panel");

    			_rightPanel.addListener(EventTriggerType.PointerEnter, (e) =>
    			{
    				disableTools();
				});
    			_rightPanel.addListener(EventTriggerType.PointerExit, (e) =>
    			{
    				enableTools();
    			});

    			_rightPanel.anchor(new Vector2(1.0f, 1.0f));
    			_rightPanel.size(new Vector2(GUI_RIGHT_PANEL_WIDTH, MainCanvas.Get().canvasSize().y - _topPanel.size().y));
				_rightPanel.position(new Vector2(-_dialog.size().x / 2, - MainCanvas.Get().canvasSize().y / 2 - _topPanel.size().y / 2), GUIObject.POSITION_TYPE.RELATIVE);
				_rightPanel.backgroundColor(backgroundColor);

				// Title Image
				new GUIImage((_obj) =>
				{
					GUIImage titleImage = _obj as GUIImage;
					titleImage.name("rightPanelTitleImage");

					_rightPanel.addChild(titleImage);

					titleImage.anchor(new Vector2(0.0f, 1.0f));
					titleImage.position(
					new Vector2(GUI_PADDING + GUI_DOOR_BUTTON_SIZE.x / 2, -GUI_PADDING - GUI_DOOR_BUTTON_SIZE.y / 2), 
						GUIObject.POSITION_TYPE.RELATIVE);
					titleImage.size(GUI_DOOR_BUTTON_SIZE);

					titleImage.hide();
				});

				// Title (text)
				new GUI.GUIText((_obj) =>
				{
					GUI.GUIText title = _obj as GUI.GUIText;
					title.name("rightPanelTitle");

					_rightPanel.addChild(title);

					title.anchor(new Vector2(0.5f, 1.0f));
					title.position(
						new Vector2(0, - GUI_PADDING * 3 - GUI_DOOR_BUTTON_SIZE.y * 1.5f), 
						GUIObject.POSITION_TYPE.RELATIVE);
					title.size(new Vector2(_rightPanel.size().x - GUI_PADDING * 2, GUI_DOOR_BUTTON_SIZE.y));

					title.fontSize(20);
					title.fontColor(Color.white);
					title.font(_rm.font("Fonts/Sans"));

					title.hide();
				});

				// Content
				new GUIObject((content) =>
				{
					content.name("rightPanelContent");	

					_rightPanel.addChild(content);

					content.anchor(new Vector2(0.5f, 1.0f));
					content.position(new Vector2(0, - GUI_PADDING * 5 - GUI_DOOR_BUTTON_SIZE.y * 2.5f), GUIObject.POSITION_TYPE.RELATIVE);
					content.size(new Vector2(_rightPanel.size().x - GUI_PADDING * 2, GUI_DOOR_BUTTON_SIZE.y));

					content.hide();
				});
    		});
    	}
    	public void destroy()
    	{
    		_topPanel.destroy();
    		_topPanel = null;

    		_leftPanel.destroy();
    		_leftPanel = null;

    		_rightPanel.destroy();
    		_rightPanel = null;
    	}

    	private void updateRightPanel(string title, Sprite titleSprite, Vector2 spriteSize, Action<GUIObject> contentCallback)
    	{
    		GUI.GUIText titleObj = _rightPanel.findChildByName("rightPanelTitle") as GUI.GUIText;
    		GUIImage titleSpriteObj = _rightPanel.findChildByName("rightPanelTitleImage") as GUIImage;
    		GUIObject contentObj = _rightPanel.findChildByName("rightPanelContent");

			Debug.Assert(titleObj != null && titleSpriteObj != null && contentObj != null);

			titleObj.show();
			titleObj.text(title);

			titleSpriteObj.show();
			titleSpriteObj.size(spriteSize);
			titleSpriteObj.image(titleSprite);

			contentObj.show();

			contentCallback(contentObj);
    	}

    	private void create_doorInitiallyOpened(Action<GUICheckbox> callback)
    	{
    		GUIObject content = _rightPanel.findChildByName("rightPanelContent");
    		
    		new GUICheckbox((_obj) =>
			{
				GUICheckbox checkbox = _obj as GUICheckbox;

				content.addChild(checkbox);

				checkbox.text("Initially Closed");
				checkbox.anchor(new Vector2(0.5f, 1.0f));
				checkbox.size(new Vector2(content.size().x, GUI_DOOR_BUTTON_SIZE.y));
				checkbox.fontColor(Color.white);

				checkbox.onToggle = (state) =>
				{
					switch (state)
					{
						case GUICheckbox.STATE.CHECKED:
						{
							PlaceableDoor door = ObjectManipulator.Get().getObject() as PlaceableDoor;

							Debug.Assert(door != null);

							door.deactivate();
							break;
						}
						case GUICheckbox.STATE.UNCHECKED:
						{
							PlaceableDoor door = ObjectManipulator.Get().getObject() as PlaceableDoor;

							Debug.Assert(door != null);

							door.activate();
							break;
						}
					}
				};

				if (callback != null)
					callback(checkbox);
    		});
    	}
    	private void create_rotationDropDown(Action<GUIDropDown> callback)
		{
    		GUIObject content = _rightPanel.findChildByName("rightPanelContent");

			// Rotation
			new GUIDropDown((_obj) =>
			{
				GUIDropDown dropDown = _obj as GUIDropDown;
				dropDown.name("Rotation Selector");

				content.addChild(dropDown);

				dropDown.anchor(new Vector2(0.5f, 1.0f));
				dropDown.size(new Vector2(content.size().x, GUI_DOOR_BUTTON_SIZE.y / 2));

				dropDown.addOption("Left");
				dropDown.addOption("Up");
				dropDown.addOption("Right");
				dropDown.addOption("Down");

				dropDown.onSelect = (oldSelect, newSelect) =>
				{
					PlaceableDoor door = ObjectManipulator.Get().getObject() as PlaceableDoor;

					switch (newSelect)
					{
						case "Left":
							door.rotateTo(SIDE_4.LEFT);

							break;
						case "Up":
							door.rotateTo(SIDE_4.TOP);

							break;
						case "Right":
							door.rotateTo(SIDE_4.RIGHT);

							break;
						case "Down":
							door.rotateTo(SIDE_4.BOTTOM);

							break;
					}
				};

				if (callback != null)
					callback(dropDown);
			});
    	}

    	private void resetTools()
		{
			ObjectManipulator.Get().clear();
			TileManipulator.Get().deactivate();
			TileEraser.Get().deactivate();
    	}

    	private void disableTools()
    	{
			ObjectManipulator.Get().disableControls();
			TileManipulator.Get().disableControls();
			TileEraser.Get().disableControls();
    	}
    	private void enableTools()
    	{
			ObjectManipulator.Get().enableControls();
			TileManipulator.Get().enableControls();
			TileEraser.Get().enableControls();
    	}

    	private void emptyRightPanel()
		{
    		GUI.GUIText titleObj = _rightPanel.findChildByName("rightPanelTitle") as GUI.GUIText;
    		GUIImage titleSpriteObj = _rightPanel.findChildByName("rightPanelTitleImage") as GUIImage;
    		GUIObject contentObj = _rightPanel.findChildByName("rightPanelContent");

			Debug.Assert(titleObj != null && titleSpriteObj != null && contentObj != null);

			titleObj.hide();
			titleSpriteObj.hide();

			_rightPanel.findChildByName("rightPanelContent").destroyAllChildren();
			contentObj.hide();
    	}
    }
}
