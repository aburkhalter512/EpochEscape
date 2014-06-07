using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using S = SaveManager;

public class GameManager : UnitySingleton<GameManager>
{
	#region Skin
	public GUISkin EpochSkin = null;
	#endregion
	
	#region Game variables
	#region Levels
	public int currentLevel = 0;
	#endregion

	#region Characters
	public int m_currentCharacter;
	public const int CAVEGIRL = 0;
	public const int KNIGHT = 1;
	public const int NINJA = 2;
	public const int ASTRONAUT = 3;
	public const int MUMMY = 4;
	public const int ROBOT = 5;
	public bool caveUnlocked = true;
	public bool knightUnlocked = true;
	public bool ninjaUnlocked = false;
	public bool astroUnlocked = false;
	public bool mumUnlocked = false;
	public bool robUnlocked = false;
	#endregion

	#region Game Items
	public const int MAX_MEMO = 5;
	public int ninjaMemo = 0;
	public int astroMemo = 0;
	public int mumMemo = 0;
	public int robMemo = 0;
	#endregion
	#endregion

	#region Popup Menus
	public bool popup = false;
	public List<string> messages = new List<string>();
	private int PopupPage = 0;
	private string PopupButtonText;
	#endregion
	
	#region Pause Menu
	public bool paused = false;

	public bool ShowPauseMenu = false;

	#region Tutorial
	public bool Tutorial = true;
	#endregion

	#region FPS
	public bool showFPS = false;
	private float accum = 0f;
	private int frames = 0;
	private float timeLeft;
	private const float updateInterval = 0.5f;
	GUIText FPS = null;
	#endregion
	
	#region Page
	public enum Page{
		Main, Options
	}
	private Page currentPage = Page.Main;
	#endregion
	
	#region Option variables
	private float optionsCenter = 275f;
	public int graphicsQuality = 4;
	#endregion
    #endregion

    SceneManager sceneManager;

	void Start ()
    {
		Application.targetFrameRate = 60;
		LoadFPS ();
		LoadOptions ();
		S.Load ();
	}
	
	void Update()
    {
		UpdateKeyboard ();
		ShowFPSText ();
	}
	void OnGUI() {
		Pause ();
		ShowPopupMessage ();
	}
	
	#region Update Methods
	private void UpdateKeyboard(){
		#region Escape
		if (Input.GetKeyDown (KeyCode.Escape) && !popup){
			if(Application.loadedLevelName != "MainMenu" && Application.loadedLevelName != "Loading" && !paused)
				PauseGame ();
			else if(paused && currentPage == Page.Main)
				UnpauseGame ();
			else if (Input.GetKeyDown (KeyCode.Escape) && currentPage != Page.Main){
				currentPage = Page.Main;
			}
		}
		#endregion
	}
	#endregion
	
	#region Pause Menu Methods
	void Pause(){
		if(paused && ShowPauseMenu)
		switch (currentPage) {
			case Page.Main: showPauseMenu(); break;
			case Page.Options: ShowOptions(); break;
		}
	}
	
	void showPauseMenu(){
		GUILayout.BeginArea (new Rect(Screen.width/2f - 175, Screen.height/2f - 200, 400, 400));
			#region Pause Menu Options
			GUILayout.BeginVertical ();
				if (GUILayout.Button ("Continue", EpochSkin.button))
						UnpauseGame ();
				GUILayout.Space (10);
				if (GUILayout.Button ("Main Menu", EpochSkin.button)) {
						S.Save ();
						Application.LoadLevel ("MainMenu");
						UnpauseGame ();
				}
				GUILayout.Space (10);
				if (GUILayout.Button ("Save Game", EpochSkin.button))
						S.Save ();
				GUILayout.Space (10);
				if (GUILayout.Button ("Options", EpochSkin.button))
						currentPage = Page.Options;
				GUILayout.Space (10);
				if (GUILayout.Button ("Restart Level", EpochSkin.button)) {
						UnpauseGame ();
						SceneManager.Load (Application.loadedLevelName);
				}
				GUILayout.Space (10);
				if (GUILayout.Button ("Quit Game", EpochSkin.button)) {
						S.Save ();
						Application.Quit ();
				}
			GUILayout.EndVertical ();
			#endregion
		GUILayout.EndArea ();
	}
	
	void ShowBackButton(){
		GUILayout.BeginArea (new Rect(Screen.width - 150, Screen.height - 75 , 110, 50));
		if(GUILayout.Button ("Back", EpochSkin.GetStyle ("Small Button")))
			currentPage = Page.Main;
		GUILayout.EndArea ();
	}
	
	private void EndPage() {
		GUILayout.EndArea();
		if (currentPage != Page.Main) {
			ShowBackButton();
		}
	}
	#region Pause Methods
	public void UnpauseGame ()
	{
			paused = false;
			Time.timeScale = 1f;
	}

	public void PauseGame ()
	{
			paused = true;
			Time.timeScale = 0f;
			ShowPauseMenu = true;
	}

	public void PauseMovement ()
	{
			paused = true;
			ShowPauseMenu = false;
	}

	public void UnpauseMovement ()
	{
			paused = false;
	}

	public void PauseMovementTS ()
	{
			paused = true;
			ShowPauseMenu = false;
			Time.timeScale = 0f;
	}

	public void UnpauseMovementTS ()
	{
			paused = false;
			Time.timeScale = 1f;
	}
	#endregion

	#region Options
	void ShowOptions(){
		GUILayout.BeginArea (new Rect(Screen.width/2f - 350, Screen.height/2 - 200, 700, 400));
			#region Options Menu
			GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ();
					GUILayout.Space (290);
					GUILayout.Label ("Volume", EpochSkin.label);
				GUILayout.EndHorizontal ();

				VolumeControl ();
				GUILayout.Space (25);

				GUILayout.BeginHorizontal ();
					GUILayout.Space (210);
					GUILayout.Label ("Graphics Quality", EpochSkin.label);
				GUILayout.EndHorizontal ();

				GraphicControl ();

				GUILayout.BeginHorizontal ();
					GUILayout.Space (25);
					GUILayout.Label ("Show FPS", EpochSkin.label);
					showFPS = GUILayout.Toggle (showFPS, "", EpochSkin.toggle);
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
					GUILayout.Space (25);
					GUILayout.Label ("Tutorial On", EpochSkin.label);
					Tutorial = GUILayout.Toggle (Tutorial, "", EpochSkin.toggle);
				GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			#endregion 
		GUILayout.EndArea ();

		GUILayout.BeginArea (new Rect(Screen.width - 150f, Screen.height - 75f, 110, 50));
		if(GUILayout.Button ("Save", EpochSkin.GetStyle ("Small Button"))){
			SaveOptions ();
			currentPage = Page.Main;
		}
		GUILayout.EndArea ();
	}
	
	private void VolumeControl(){
		GUILayout.BeginHorizontal ();
			GUILayout.Label ("Off", EpochSkin.textArea);
			AudioListener.volume = GUILayout.HorizontalSlider (AudioListener.volume, 0f, 1f, EpochSkin.horizontalSlider, EpochSkin.horizontalSliderThumb);
			GUILayout.Label ("Max", EpochSkin.textArea);
		GUILayout.EndHorizontal ();

		GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
				GUILayout.Space (270);
				GUILayout.Label ((int)(AudioListener.volume * 100) + "%", EpochSkin.textArea);
			GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
	}
	
	private void GraphicControl(){	
		GUILayout.BeginHorizontal ();
			GUILayout.Label ("Fastest", EpochSkin.textArea);
			graphicsQuality = (int)GUILayout.HorizontalSlider ((float)graphicsQuality, 1.0f, 6.0f, EpochSkin.horizontalSlider, EpochSkin.horizontalSliderThumb);
			GUILayout.Label ("Fantastic", EpochSkin.textArea);
		GUILayout.EndHorizontal ();

		GUILayout.BeginVertical ();
			#region Quality Labels
			GUILayout.BeginHorizontal ();
				GUILayout.Space (270);
				switch (graphicsQuality) {
				case 1:
						QualitySettings.SetQualityLevel (1);
						GUILayout.Label ("Fastest", EpochSkin.textArea);
						break;
				case 2:
						QualitySettings.SetQualityLevel (2);
						GUILayout.Label ("Fast", EpochSkin.textArea);
						break;
				case 3:
						QualitySettings.SetQualityLevel (3);
						GUILayout.Label ("Simple", EpochSkin.textArea);
						break;
				case 4:
						QualitySettings.SetQualityLevel (4);
						GUILayout.Label ("Good", EpochSkin.textArea);
						break;
				case 5:
						QualitySettings.SetQualityLevel (5);
						GUILayout.Label ("Beautiful", EpochSkin.textArea);
						break;
				case 6:
						QualitySettings.SetQualityLevel (6);
						GUILayout.Label ("Fantastic", EpochSkin.textArea);
						break;
				}
			GUILayout.EndHorizontal ();
			#endregion
		GUILayout.EndVertical ();
	}
	#region FPS
	private void LoadFPS(){
		if(FPS == null){
			GameObject go = new GameObject();
			go.name = "FPS";

			FPS = (GUIText) go.AddComponent(typeof(GUIText));
			//FPS.transform.position = new Vector3 (.92f, .98f, 0);

			FPS.anchor = TextAnchor.UpperRight;
			FPS.transform.position = new Vector3 (1f, 1f, 0f);
			FPS.text = "";
		}
	}
	
	private void ShowFPSText(){
		LoadFPS ();
		if(showFPS){
			timeLeft -= Time.deltaTime;
			accum += Time.timeScale / Time.deltaTime;
			++frames;
			FPSControl ();
		}
		else
			FPS.text = "";
	}
	
	private void FPSControl(){
		if (showFPS) {
			if (timeLeft <= 0.0f) {
				float fps = accum/frames;
				string fpsFormat = System.String.Format ("{0:F2} FPS", fps);
				FPS.text = fpsFormat;
				if(fps < 30)
					FPS.material.color = Color.yellow;
				else if(fps < 10)
					FPS.material.color = Color.red;
				else
					FPS.material.color = Color.green;
				timeLeft = updateInterval;
				accum = 0.0f;
				frames = 0;
			}
		}
	}

	public void SaveOptions(){
		PlayerPrefs.SetFloat ("Volume", AudioListener.volume);
		PlayerPrefs.SetInt ("Graphics", graphicsQuality);
		PlayerPrefs.SetInt ("FPS", showFPS == true ? 1 : 0);
	}

	private void LoadOptions(){
		graphicsQuality = PlayerPrefs.GetInt ("Graphics");
		if(!PlayerPrefs.HasKey ("Graphics"))
			graphicsQuality = 4;
		AudioListener.volume = PlayerPrefs.GetFloat ("Volume");
		if(!PlayerPrefs.HasKey ("Volume"))
			AudioListener.volume = 1f;
		showFPS = PlayerPrefs.GetInt ("FPS") == 1 ? true : false;
	}
	#endregion
	#endregion
	#endregion

	#region Popups
	public void ShowPopupMessage(){
		if(popup && Tutorial){
			if(PopupPage + 1 < messages.Count)
				PopupButtonText = "Next";
			else
				PopupButtonText = "OK";
			PauseMovementTS();
			#region Key Presses
			if (Event.current.type == EventType.KeyUp && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Space)) {
				if(PopupPage + 1 == messages.Count){
					popup = false;
					//messages.Clear();
					PopupPage = 0;
					UnpauseMovementTS();
				}
				else{
					PopupPage++;
				}
			}
			#endregion
			GUILayout.BeginArea (new Rect(Screen.width/2f - 350, Screen.height * .6f, 700, 150));
				GUILayout.Box (messages[PopupPage], EpochSkin.GetStyle ("Message"));
			#region Button 
				GUILayout.BeginArea (new Rect(600, 100, 300, 100));
					GUILayout.BeginHorizontal ();
						GUILayout.BeginVertical ();
						if(GUILayout.Button (PopupButtonText, EpochSkin.GetStyle ("Popup Button"))){
							if(PopupPage + 1 == messages.Count){
								popup = false;
								PopupPage = 0;
								UnpauseMovementTS();
							}
							else{
								PopupPage++;
							}
						}
						GUILayout.EndHorizontal ();
					GUILayout.EndVertical ();
				GUILayout.EndArea ();
			#endregion
			GUILayout.EndArea ();
		}
	}
	#endregion
}
