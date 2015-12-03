using GUI;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

using ResourceManager;
using Utilities;

public class MainMenu
{
	private static readonly Vector2 ARROW_SIZE = new Vector2(40, 40);
	private static ResourceManager.ResourceManager _rm = ResourceManager.ResourceManager.Get();

	private class MainPage
	{
		private static GUIImage _pageTitle;
		private static GUIButton _levelButton;
		private static GUIButton _editorButton;
		private static GUIButton _optionsButton;
		private static GUIButton _exitButton;

		private static bool _isShown;

		public static void show()
		{
			if (_isShown)
				return;

			_isShown = true;

			Debug.Log("Showing Main Page.");

			// Epoch Escape Title ======================================================================================
			_pageTitle = new GUIImage((_obj) =>
			{
				GUIImage obj = _obj as GUIImage;
				obj.position(new Vector3(0, 0.7f));
				obj.size(new Vector2(800, 120));
				obj.image(_rm.sprite("Textures/GUI/MainMenu/EpochEscape"));
			});

			// Level Button ============================================================================================
			_levelButton = new GUIButton((_obj) =>
			{
				GUIButton obj = _obj as GUIButton;
				obj.name("levelButton");
				obj.position(new Vector3(0, 0.2f));
				obj.size(new Vector2(150, 60));
				obj.setBaseBackground(_rm.sprite("Textures/GUI/MainMenu/LevelsButton"), Color.white);
				obj.setOverBackground(_rm.sprite("Textures/GUI/MainMenu/LevelsButtonHover"), Color.white);
				obj.setDownBackground(_rm.sprite("Textures/GUI/MainMenu/LevelsButtonHover"), Color.white);

				obj.addListener(EventTriggerType.PointerClick, (e) =>
				{
					LevelsPage.show();
					MainPage.hide();
				});
			});

			// Editor Button ===========================================================================================
			_editorButton = new GUIButton((_obj) =>
			{
				GUIButton obj = _obj as GUIButton;
				obj.name("editorButton");
				obj.position(new Vector2(0, -0.1f));
				obj.size(new Vector2(150, 60));
				obj.setBaseBackground(_rm.sprite("Textures/GUI/MainMenu/EditorButton"), Color.white);
				obj.setOverBackground(_rm.sprite("Textures/GUI/MainMenu/EditorButtonHover"), Color.white);
				obj.setDownBackground(_rm.sprite("Textures/GUI/MainMenu/EditorButtonHover"), Color.white);

				obj.addListener(EventTriggerType.PointerClick, (e) =>
				{
					Driver.Get().progress(Driver.DRIVER_STATE.EDITOR);
				});
			});

			// Options Button ==========================================================================================
			_optionsButton = new GUIButton((_obj) =>
			{
				GUIButton obj = _obj as GUIButton;

				obj.name("optionsButton");
				obj.position(new Vector2(0, -0.4f));
				obj.size(new Vector2(150, 60));
				obj.setBaseBackground(_rm.sprite("Textures/GUI/MainMenu/OptionsButton"), Color.white);
				obj.setOverBackground(_rm.sprite("Textures/GUI/MainMenu/OptionsButtonHover"), Color.white);
				obj.setDownBackground(_rm.sprite("Textures/GUI/MainMenu/OptionsButtonHover"), Color.white);

				obj.addListener(EventTriggerType.PointerClick, (e) =>
				{
					OptionsPage.show();
					MainPage.hide();
				});
			});

			// Exit Button =============================================================================================
			_exitButton = new GUIButton((_obj) =>
			{
				GUIButton obj = _obj as GUIButton;

				obj.name("exitButton");
				obj.position(new Vector2(0, -0.7f));
				obj.size(new Vector2(150, 60));
				obj.setBaseBackground(_rm.sprite("Textures/GUI/MainMenu/ExitButton"), Color.white);
				obj.setOverBackground(_rm.sprite("Textures/GUI/MainMenu/ExitButtonHover"), Color.white);
				obj.setDownBackground(_rm.sprite("Textures/GUI/MainMenu/ExitButtonHover"), Color.white);

				obj.addListener(EventTriggerType.PointerClick, (e) =>
				{
					Debug.Log("Exiting Game.");

					Driver.Get().progress(Driver.DRIVER_STATE.EXIT);
				});
			});
		}

		public static void hide()
		{
			if (!_isShown)
				return;

			Debug.Log("Hiding Main Page.");

			_pageTitle.destroy();
			_pageTitle = null;

			_levelButton.destroy();
			_levelButton = null;

			_editorButton.destroy();
			_editorButton = null;

			_optionsButton.destroy();
			_optionsButton = null;

			_exitButton.destroy();
			_exitButton = null;

			_isShown = false;
		}
	}

	private class LevelsPage
	{
		private static GUIImage _pageTitle;
		private static GUIButton _backButton;

		private static GUIVerticalDialog _levelsContainer;

		private static bool _isShown;

		public static void show()
		{
			if (_isShown)
				return;

			_isShown = true;

			Debug.Log("Showing Main Page.");

			// Epoch Escape Title ======================================================================================
			_pageTitle = new GUIImage((_obj) =>
			{
				_pageTitle.position(new Vector2(0, 0.7f));
				_pageTitle.size(new Vector2(800, 120));
				_pageTitle.image(_rm.sprite("Textures/GUI/MainMenu/EpochLevels"));
			});

			_backButton = new GUIButton((_obj) =>
			{
				GUIButton obj = _obj as GUIButton;

				obj.name("backButton");
				obj.position(new Vector2(-0.9f, 0.7f));
				obj.size(ARROW_SIZE);
				obj.setBaseBackground(_rm.sprite("Textures/GUI/Buttons/LeftArrow"), Color.white);
				obj.setOverBackground(_rm.sprite("Textures/GUI/Buttons/LeftArrowHover"), Color.white);
				obj.setDownBackground(_rm.sprite("Textures/GUI/Buttons/LeftArrowHover"), Color.white);

				obj.addListener(EventTriggerType.PointerClick, (e) =>
				{
					MainPage.show();
					LevelsPage.hide();
				});
			});

			_levelsContainer = new GUIVerticalDialog((_obj) =>
			{
				GUIVerticalDialog obj = _obj as GUIVerticalDialog;
				obj.name("levelsContainer");

				obj.position(new Vector2(0, -0.2f), GUIObject.POSITION_TYPE.PERCENT);
				obj.size(new Vector2(600, 400));
				obj.overflow(GUI.GUIContent.OVERFLOW_TYPE.HIDE);
				obj.backgroundImage(null);
				obj.backgroundColor(new Color(0, 0, 0, 0.1f));
				obj.contentHeight(800);

				new GUI.GUIText((_level1) =>
				{
					GUI.GUIText level1 = _level1 as GUI.GUIText;
					obj.addChild(level1);

					level1.name("Level1");
					level1.text("This is the first level");
					level1.font(Resources.Load<Font>("Fonts/Sans"));
					level1.fontColor(Color.black);
				});
			});
		}

		public static void hide()
		{
			if (!_isShown)
				return;

			Debug.Log("Hiding Main Page.");

			_pageTitle.destroy();
			_pageTitle = null;

			_backButton.destroy();
			_backButton = null;

			_levelsContainer.destroy();
			_levelsContainer = null;

			_isShown = false;
		}
	}

	private class OptionsPage
	{
		private static GUIImage _pageTitle;
		private static GUIButton _backButton;

		private static GUIImage _optionsContainer;

		private static bool _isShown;

		public static void show()
		{
			if (_isShown)
				return;

			_isShown = true;

			Debug.Log("Showing Options Page.");

			// Epoch Escape Title ======================================================================================
			_pageTitle = new GUIImage((_obj) =>
			{
				GUIImage obj = _obj as GUIImage;

				obj.position(new Vector2(0, 0.7f));
				obj.size(new Vector2(800, 120));
				obj.image(_rm.sprite("Textures/GUI/MainMenu/EpochOptions"));
			});

			_backButton = new GUIButton((_obj) =>
			{
				GUIButton obj = _obj as GUIButton;

				obj.name("backButton");
				obj.position(new Vector2(-0.9f, 0.7f));
				obj.size(new Vector2(30, 30));
				obj.setBaseBackground(_rm.sprite("Textures/GUI/Buttons/LeftArrow"), Color.white);
				obj.setOverBackground(_rm.sprite("Textures/GUI/Buttons/LeftArrowHover"), Color.white);
				obj.setDownBackground(_rm.sprite("Textures/GUI/Buttons/LeftArrowHover"), Color.white);

				obj.addListener(EventTriggerType.PointerClick, (e) =>
				{
					MainPage.show();
					OptionsPage.hide();
				});
			});

			_optionsContainer = new GUIImage((_obj) =>
			{
				GUIImage obj = _obj as GUIImage;

				obj.name("optionsContainer");
				obj.position(new Vector2(0, -0.2f), GUIObject.POSITION_TYPE.PERCENT);
				obj.size(new Vector2(600, 400));
				obj.image(null);
				obj.imageColor(new Color(0, 0, 0, 0.1f));

				new GUI.GUIText((_creditsObj) =>
				{
					GUI.GUIText creditsObj = _creditsObj as GUI.GUIText;

					creditsObj.name("credits");
					obj.addChild(creditsObj);

					creditsObj.position(new Vector2(0, 0), GUIObject.POSITION_TYPE.RELATIVE);
					creditsObj.size(new Vector2(550, 300));
					creditsObj.font(Resources.Load<Font>("Fonts/Sans"));
					creditsObj.fontSize(26);
					creditsObj.text("Credits:\n" +
						"\tAdam Burkhalter: Lead Developer/Project Manager\n" +
						"\tTerry Rogers: Developer\n" +
						"\tMicah Lee: Developer\n" +
						"\tMichael Hsieh: Developer\n" +
						"\tCandelaria Herrera: Project Manager");
				});
			});
		}

		public static void hide()
		{
			if (!_isShown)
				return;

			Debug.Log("Hiding Main Page.");

			_pageTitle.destroy();
			_pageTitle = null;

			_backButton.destroy();
			_backButton = null;

			_optionsContainer.destroy();
			_optionsContainer = null;

			_isShown = false;
		}
	}

	public MainMenu()
	{
		MainPage.show();
	}
	public void destroy()
	{
		MainPage.hide();
	}
}

