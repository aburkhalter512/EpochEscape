using MapEditor;

using UnityEngine;

public class Driver : Manager<Driver>
{
	public enum DRIVER_STATE
	{
		MAIN_MENU,
		GAME,
		EDITOR,
		EDITOR_GAME,
		EXIT
	}

	private DRIVER_STATE _state;

	private CameraManager _cm;
	private MainMenu _mainMenu;

	// Map Variables
	private MapEditor.MapEditorGUI _mapGUI;

	protected override void Awaken()
	{
		_cm = CameraManager.Get();
		toMainMenu();
	}

	protected override void Initialize ()
	{ }

	public bool progress(DRIVER_STATE newState)
	{
		if (!canProgressTo(newState))
			return false;

		progressFrom();

		progressTo(newState);

		return true;
	}

	private bool canProgressTo(DRIVER_STATE newState)
	{
		switch (_state)
		{
			case DRIVER_STATE.MAIN_MENU:
			{
				switch (newState)
				{
					case DRIVER_STATE.GAME:
					case DRIVER_STATE.EDITOR:
					case DRIVER_STATE.EXIT:
						return true;
					default:
						return false;
				}
			}
			case DRIVER_STATE.GAME:
			{
				switch (newState)
				{
					case DRIVER_STATE.MAIN_MENU:
						return true;
					default:
						return false;
				}
			}
			case DRIVER_STATE.EDITOR:
			{
				switch (newState)
				{
					case DRIVER_STATE.MAIN_MENU:
					case DRIVER_STATE.EDITOR_GAME:
						return true;
					default:
						return false;
				}
			}
			case DRIVER_STATE.EDITOR_GAME:
			{
				switch (newState)
				{
					case DRIVER_STATE.EDITOR:
						return true;
					default:
						return false;
				}
			}
			case DRIVER_STATE.EXIT:
			{
				switch (newState)
				{
					default:
						return false;
				}
			}
			default:
				return true;
		}
	}

	private void progressFrom()
	{
		switch (_state)
		{
			case DRIVER_STATE.MAIN_MENU:
				fromMainMenu();
				break;
			case DRIVER_STATE.EDITOR:
				fromEditor();
				break;
			case DRIVER_STATE.GAME:
				fromGame();
				break;
			case DRIVER_STATE.EDITOR_GAME:
				fromEditorGame();
				break;
			default:
				break;
		}
  	}
  	private void progressTo(DRIVER_STATE newState)
	{
		switch (newState)
		{
			case DRIVER_STATE.MAIN_MENU:
				toMainMenu();
				break;
			case DRIVER_STATE.GAME:
				toGame();
				break;
			case DRIVER_STATE.EDITOR:
				toEditor();
				break;
			case DRIVER_STATE.EDITOR_GAME:
				toEditorGame();
				break;
			case DRIVER_STATE.EXIT:
				toExit();
				break;
		}
  	}

	private void toMainMenu()
	{
		_mainMenu = new MainMenu();

		_cm.camType(CameraManager.CAM_TYPE.MAIN_MENU);

		_state = DRIVER_STATE.MAIN_MENU;
	}
	private void fromMainMenu()
	{
		_mainMenu.destroy();

		_mainMenu = null;
	}

	private void toGame()
	{
		_cm.camType(CameraManager.CAM_TYPE.GAME);

		_state = DRIVER_STATE.GAME;
	}
	private void fromGame()
	{

	}

	private void toEditor()
	{
		MapEditor.InputManager.Get();
		MapEditor.MapEditor mapEditor = MapEditor.MapEditor.Get();
		Utilities.GOCallback callback = mapEditor.gameObject.AddComponent<Utilities.GOCallback>();
		callback.startCallback = (obj) =>
		{
			mapEditor.initDefault();
		};
		_mapGUI = new MapEditorGUI();

		_cm.camType(CameraManager.CAM_TYPE.EDITOR);

		_state = DRIVER_STATE.EDITOR;
	}
	private void fromEditor()
	{
		MapEditor.MapEditor.Get().destroy();
		MapEditor.InputManager.Get().destroy();
		_mapGUI.destroy();
	}

	private void fromEditorGame()
	{

	}
	private void toEditorGame()
	{

	}

	private void toExit()
	{
		Application.Quit();

		_state = DRIVER_STATE.EXIT;
	}
}
