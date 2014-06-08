using UnityEngine;
using System.Collections;
using G = GameManager;
using S = SaveManager;

public class MainMenu : MonoBehaviour {
	#region Levels
	public int MAX_LEVELS = 8;
	#endregion
	#region Styles
	public GUISkin EpochSkin = null;
	#endregion
	#region Options
	private string name = "";
	public AudioSource BackSound;
	public AudioSource ClickSound;
	private bool playSound = true;
	#endregion
	#region Character Select
	private bool fromLoad = false;
	private const int MAX_CHARS = 5;
	private int characterNum = 0;
	private Texture[] charTex;
	#endregion
	#region Page
	public enum Page{
		Main, Character, Options, ResetWarning, Credits, LoadSelect, SaveSelect, 
		SaveWarning, LoadWarning, DeleteWarning, SaveName, //Tutorial
	}
	private Page prevPage = Page.Main;
	private Page currentPage = Page.Main;
	#endregion

	void Start(){
		charTex = new Texture[6];
		LoadCharTex ();
	}
	#region Load Characters
	private void LoadCharTex(){
		charTex[0] = Resources.Load("Textures/GUI/Select/CaveGirlSelect", typeof(Texture)) as Texture;
		charTex[1] = Resources.Load("Textures/GUI/Select/KnightSelect", typeof(Texture)) as Texture;
		if(G.getInstance().ninjaUnlocked)
			charTex[2] = Resources.Load("Textures/GUI/Select/NinjaSelect", typeof(Texture)) as Texture;
		else
			charTex[2] = Resources.Load ("Textures/GUI/Select/NinjaLocked", typeof(Texture)) as Texture;
		if(G.getInstance().astroUnlocked)
			charTex[3] = Resources.Load("Textures/GUI/Select/AstronautSelect", typeof(Texture)) as Texture;
		else
			charTex[3] = Resources.Load ("Textures/GUI/Select/AstronautLocked", typeof(Texture)) as Texture;
		if(G.getInstance().mumUnlocked)
			charTex[4] = Resources.Load("Textures/GUI/Select/MummySelect", typeof(Texture)) as Texture;
		else
			charTex[4] = Resources.Load ("Textures/GUI/Select/MummyLocked", typeof(Texture)) as Texture;
		if(G.getInstance().robUnlocked)
			charTex[5] = Resources.Load("Textures/GUI/Select/RobotSelect", typeof(Texture)) as Texture;
		else
			charTex[5] = Resources.Load ("Textures/GUI/Select/RobotLocked", typeof(Texture)) as Texture;
	}
	#endregion

	void OnGUI() {
		if (Input.GetKeyDown (KeyCode.Escape))
			currentPage = Page.Main;
		switch (currentPage) {
			case Page.Main: ShowMain (); break;
			case Page.Options: ShowOptions(); break;
			case Page.ResetWarning: ResetWarning(); break;
			case Page.Character: SelectMenu(); break;
			case Page.Credits: ShowCredits(); break;
			case Page.LoadSelect: ShowLoad(); break;
			case Page.SaveSelect: ShowSaveSelect (); break;
			case Page.SaveWarning: ShowSaveWarning(); break;
			case Page.LoadWarning: ShowLoadWarning(); break;
			case Page.DeleteWarning: DeleteWarning (); break;
			case Page.SaveName: ShowTypeName(); break;
			//case Page.Tutorial: AskTutorial(); break;
		}
	}

	#region Main Menu
	void ShowMain () {
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 200, Screen.height/2 - 175, 400f, 500));
		GUILayout.Box ("", EpochSkin.GetStyle ("Tablet"));
		GUILayout.EndArea ();
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 200, Screen.height * .35f, 400f, 500));
			#region Main Menu Buttons
			if (GUILayout.Button ("Load Game", EpochSkin.button)){
				ClickSound.Play ();	
				currentPage = Page.LoadSelect;
			}
			GUILayout.Space (10);
			if (GUILayout.Button ("New Game", EpochSkin.button)) {
					ClickSound.Play ();	
					characterNum = 0;
					currentPage = Page.Character;
			}
			GUILayout.Space (10);
			if (GUILayout.Button ("Options", EpochSkin.button)){
				ClickSound.Play ();	
				currentPage = Page.Options;
			}
			GUILayout.Space (10);
			if (GUILayout.Button ("Credits", EpochSkin.button)){
				ClickSound.Play ();	
				currentPage = Page.Credits;
			}
			GUILayout.Space (10);
			if (GUILayout.Button ("Quit Game", EpochSkin.button)) {
				ClickSound.Play ();	
				S.Save ();
				Application.Quit ();
			}
			#endregion
		GUILayout.EndArea ();
	}
	#endregion

	#region Options
	void ShowOptions(){
		GUILayout.BeginArea (new Rect(Screen.width/1.5f - 350, Screen.height * .25f, 650, 410));
			#region Options List
			GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ();
					GUILayout.Space (270);
					GUILayout.Label ("Volume", EpochSkin.label);
				GUILayout.EndHorizontal ();

			VolumeControl ();
			GUILayout.Space (25);

			GUILayout.BeginHorizontal ();
				GUILayout.Space (180);
				GUILayout.Label ("Graphics Quality", EpochSkin.label);
			GUILayout.EndHorizontal ();

			GraphicControl ();

			GUILayout.BeginHorizontal ();
				GUILayout.Space (25);
				GUILayout.Label ("Show FPS", EpochSkin.label);
				G.getInstance().showFPS = GUILayout.Toggle (G.getInstance().showFPS, "", EpochSkin.toggle);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
				GUILayout.Space (25);
				GUILayout.Label ("Tutorial On", EpochSkin.label);
				G.getInstance().Tutorial = GUILayout.Toggle (G.getInstance ().Tutorial, "", EpochSkin.toggle);
			GUILayout.EndHorizontal ();

			if (GUILayout.Button ("Reset Game", EpochSkin.button)) {
				ClickSound.Play ();	
				currentPage = Page.ResetWarning;
			}
			GUILayout.EndVertical ();
			#endregion

		GUILayout.EndArea ();

		GUILayout.BeginArea (new Rect(Screen.width - 150f, Screen.height - 75, 110, 50));
			if(GUILayout.Button ("Save", EpochSkin.GetStyle ("Small Button"))){
				ClickSound.Play ();	
				G.getInstance().SaveOptions ();
				currentPage = Page.Main;
			}
		GUILayout.EndArea ();
	}

	private void ResetWarning(){
		GUILayout.BeginArea (new Rect(Screen.width /1.5f - 250, Screen.height * .35f, 500, 500));
			GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ();
					GUILayout.Space (80);
					GUILayout.Label ("Reset All Of Your Progress?", EpochSkin.textArea);
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
					GUILayout.Space (130);
					GUILayout.Label ("This Will Delete All Saves, And Unlocks", EpochSkin.textArea);
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
					if(GUILayout.Button ("Yes", EpochSkin.button)){
						ClickSound.Play ();	
						S.ResetGame ();
						currentPage = Page.Options;
					}
					GUILayout.Space (10);
					if(GUILayout.Button ("No", EpochSkin.button)){
						ClickSound.Play ();	
						currentPage = Page.Options;
					}
				GUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
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
				GUILayout.Space (250);
				GUILayout.Label ((int)(AudioListener.volume * 100) + "%", EpochSkin.textArea);
			GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
	}

	private void GraphicControl(){
		GUILayout.BeginHorizontal ();
			GUILayout.Label ("Fastest", EpochSkin.textArea);
			G.getInstance().graphicsQuality = (int)GUILayout.HorizontalSlider ((float)G.getInstance().graphicsQuality, 1.0f, 6.0f, EpochSkin.horizontalSlider, EpochSkin.horizontalSliderThumb);
			GUILayout.Label ("Fantastic", EpochSkin.textArea);
		GUILayout.EndHorizontal ();
		GUILayout.BeginVertical ();
					#region Quality Labels
					GUILayout.BeginHorizontal ();
					GUILayout.Space (250);
					switch (G.getInstance ().graphicsQuality) {
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
	#endregion

	#region Character Selection
	void SelectMenu(){
		GUILayout.BeginArea (new Rect(Screen.width/1.5f - 250, Screen.height * .22f, 500, 500));
			GUILayout.BeginVertical ();
				GUILayout.Label ("Choose Your Character", EpochSkin.GetStyle ("Title"));
				GUILayout.Label (CharacterName(), EpochSkin.label);
				if (GUILayout.Button ("", EpochSkin.GetStyle ("SelectUp"))) {
					ClickSound.Play ();	
					increaseSelect();
				}
				GUILayout.Space (5);
				GUILayout.Label (charTex[characterNum], EpochSkin.GetStyle ("imageSelect"));
				GUILayout.Space (5);
				if(GUILayout.Button("", EpochSkin.GetStyle ("SelectDown"))){
					ClickSound.Play ();	
					decreaseSelect();
				}
			GUILayout.EndVertical ();

		EndPage ();

		GUILayout.BeginArea (new Rect(Screen.width - 270, Screen.height - 75 , 110, 100));
			if (GUILayout.Button ("Select", EpochSkin.GetStyle ("Small Button"))) {
				ClickSound.Play ();	
				if(CharIsUnlocked ()){
					if(fromLoad){
						prevPage = Page.Character;
						currentPage = Page.SaveName;
					}
					else{
						prevPage = Page.Character;
						currentPage = Page.SaveSelect;
					}
					G.getInstance().m_currentCharacter = characterNum;
				}
			}
		GUILayout.EndArea ();

	}	

	private bool CharIsUnlocked(){
		bool unlocked = false;
		switch (characterNum) {
			case 0:
				unlocked = true;
				break;
			case 1:
				unlocked = true;
				break;
			case 2:
				if (G.getInstance().knightUnlocked)
					unlocked = true;	
				else
					unlocked = false;
				break;
			case 3:
				if (G.getInstance().astroUnlocked)
					unlocked = true;	
				else
					unlocked = false;
					break;
			case 4:
				if (G.getInstance().mumUnlocked)
					unlocked = true;	
				else
					unlocked = false;
					break;
			case 5:
				if (G.getInstance().robUnlocked)
					unlocked = true;	
				else
					unlocked = false;
				break;
		}
		return unlocked;
	}

	string CharacterName(){
		string name = "";
		switch (characterNum) {
		case 0:
			name = "The Cave Girl";
			break;
		case 1:
			name = "The Knight";
			break;
		case 2:
			if(G.getInstance().ninjaUnlocked)
				name = "The Ninja";
			else
				name = "Locked";
			break;
		case 3:
			if(G.getInstance().astroUnlocked)
				name = "The Astronaut";
			else
				name = "Locked";
			break;
		case 4:
			if(G.getInstance().mumUnlocked)
				name = "The Mummy";
			else
				name = "Locked";
			break;
		case 5:
			if(G.getInstance().robUnlocked)
				name = "The Robot";
			else
				name = "Locked";
			break;
		}
		return name;
	}

	private void increaseSelect(){
		characterNum++;
		if(characterNum > MAX_CHARS)
			characterNum = 0;
	}

	private void decreaseSelect(){
		characterNum--;
		if(characterNum < 0)
			characterNum = MAX_CHARS;
	}
	#endregion

	#region Credits Page
	void ShowCredits(){
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 300, Screen.height * .25f, 575, 450));
		GUILayout.Label ("", EpochSkin.GetStyle ("Credits"));
		EndPage ();
	}
	#endregion

	#region Load Selection
	void ShowLoad(){
		GUILayout.BeginArea (new Rect (Screen.width/1.5f - 275, Screen.height * .35f, 525, 500));
			GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ();
					GUILayout.Label ("Choose Your Save Slot", EpochSkin.label);
				GUILayout.EndHorizontal ();

				#region buttons
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (S.save1, EpochSkin.button)) {
					ClickSound.Play ();	
					S.ChooseSave (0);
					if(S.SaveTaken()){
						S.LoadGame ();
						SceneManager.Load (GenerateLevelName());
					}
					else
						currentPage = Page.LoadWarning;
				}
				GUILayout.Space (10);
				if (GUILayout.Button ("Delete", EpochSkin.GetStyle ("Small Button"))) {
					ClickSound.Play ();	
					S.ChooseSave (0);
					if(S.SaveTaken ())
						currentPage = Page.DeleteWarning;
				}
				GUILayout.EndHorizontal ();
				GUILayout.Space (10);
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (S.save2, EpochSkin.button)) {
					ClickSound.Play ();	
					S.ChooseSave (1);
					if(S.SaveTaken()){
						S.LoadGame ();
						SceneManager.Load (GenerateLevelName());
					}
					else
						currentPage = Page.LoadWarning;
				}
				GUILayout.Space (10);
				if (GUILayout.Button ("Delete", EpochSkin.GetStyle ("Small Button"))) {
					ClickSound.Play ();
					S.ChooseSave (1);
					if(S.SaveTaken())
						currentPage = Page.DeleteWarning;
				}
				GUILayout.EndHorizontal ();
				GUILayout.Space (10);
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (S.save3, EpochSkin.button)) {
					ClickSound.Play ();
					S.ChooseSave (2);
					if(S.SaveTaken()){
						S.LoadGame();
						SceneManager.Load (GenerateLevelName());
					}
					else
						currentPage = Page.LoadWarning;
				}
				GUILayout.Space (10);
				if (GUILayout.Button ("Delete", EpochSkin.GetStyle ("Small Button"))) {
					ClickSound.Play ();
					S.ChooseSave (2);
					if(S.SaveTaken ())
						currentPage = Page.DeleteWarning;
				}
			GUILayout.EndHorizontal ();
			GUILayout.Space (10);
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (S.save4, EpochSkin.button)) {
				ClickSound.Play ();
				S.ChooseSave (3);
				if(S.SaveTaken()){
					S.LoadGame();
					SceneManager.Load (GenerateLevelName());
				}
				else
					currentPage = Page.LoadWarning;
			}
			GUILayout.Space (10);
			if (GUILayout.Button ("Delete", EpochSkin.GetStyle ("Small Button"))) {
				ClickSound.Play ();
				S.ChooseSave (3);
				if(S.SaveTaken ())
					currentPage = Page.DeleteWarning;
			}
			
			GUILayout.EndHorizontal ();
			#endregion
			GUILayout.EndVertical ();
		EndPage ();
	}

	void DeleteWarning(){
		GUILayout.BeginArea (new Rect(Screen.width/1.5f - 250, Screen.height * .35f, 500, 500));
			GUILayout.BeginVertical ();
				GUILayout.Label ("Really Delete?", EpochSkin.label);
					#region Buttons
					GUILayout.BeginHorizontal ();
					if (GUILayout.Button ("Yes", EpochSkin.button)) {
						ClickSound.Play ();
						S.DeleteSave ();
						currentPage = Page.LoadSelect;
					}
					GUILayout.Space (10);
					if (GUILayout.Button ("No", EpochSkin.button)) {
						ClickSound.Play ();
						currentPage = Page.LoadSelect;
					}
					GUILayout.EndHorizontal ();
					#endregion
			GUILayout.EndVertical ();
		GUILayout.EndArea ();
	}

	void ShowLoadWarning(){
		GUILayout.BeginArea (new Rect(Screen.width/1.5f - 250, Screen.height * .35f, 500, 500));
			GUILayout.BeginVertical ();
				GUILayout.Label ("No Save File Found! Start Save?", EpochSkin.label);
					#region Buttons
					GUILayout.BeginHorizontal ();
					if (GUILayout.Button ("Yes", EpochSkin.button)) {
						ClickSound.Play ();
						fromLoad = true;
						currentPage = Page.Character;
					}
					GUILayout.Space (10);
					if (GUILayout.Button ("No", EpochSkin.button)) {
						ClickSound.Play ();
						currentPage = Page.LoadSelect;
					}
					GUILayout.EndHorizontal ();
					#endregion
			GUILayout.EndVertical ();
		GUILayout.EndArea ();
	}
	#endregion

	#region Save Selection
	void ShowSaveSelect(){
		prevPage = Page.Character;
		GUILayout.BeginArea (new Rect (Screen.width/1.5f - 200, Screen.height * .35f, 400, 500));
			#region buttons
			GUILayout.BeginVertical ();
			GUILayout.Label ("Choose Your Save Slot", EpochSkin.label);
			if (GUILayout.Button (S.save1, EpochSkin.button)) {
				ClickSound.Play ();
				S.ChooseSave (0);
				if(S.SaveTaken ()){
					currentPage = Page.SaveWarning;
				}
				else{
					prevPage = Page.SaveSelect;
					currentPage = Page.SaveName;
				}
			}
			GUILayout.Space (10);
			if (GUILayout.Button (S.save2, EpochSkin.button)) {
				ClickSound.Play ();
				S.ChooseSave (1);
				if(S.SaveTaken ()){
					currentPage = Page.SaveWarning;
				}
				else{
					prevPage = Page.SaveSelect;
					currentPage = Page.SaveName;
				}
			}
			GUILayout.Space (10);
			if (GUILayout.Button (S.save3, EpochSkin.button)) {
				ClickSound.Play ();
				S.ChooseSave (2);
				if(S.SaveTaken ()){
					currentPage = Page.SaveWarning;
				}
				else{
					prevPage = Page.SaveSelect;
					currentPage = Page.SaveName;
				}
			}
			GUILayout.Space (10);
			if (GUILayout.Button (S.save4, EpochSkin.button)) {
				ClickSound.Play ();
				S.ChooseSave (3);
				if(S.SaveTaken ()){
					currentPage = Page.SaveWarning;
				}
				else{
					prevPage = Page.Character;
					currentPage = Page.SaveName;
				}
			}

			GUILayout.EndVertical ();
			#endregion
		EndPage ();
		ShowBackButton ();
	}

	void ShowSaveWarning (){
		GUILayout.BeginArea (new Rect(Screen.width/1.5f - 250f, Screen.height * .35f, 500, 500));
			GUILayout.BeginVertical ();
				GUILayout.Label ("Save File Exists! Overwrite?", EpochSkin.label);
				GUILayout.BeginHorizontal ();
					if(GUILayout.Button ("Yes", EpochSkin.button)){
						ClickSound.Play ();
						S.ResetOnNew ();
						prevPage = Page.SaveSelect;
						currentPage = Page.SaveName;
					}
					GUILayout.Space (10);
					if(GUILayout.Button ("No", EpochSkin.button)){
						ClickSound.Play ();
						currentPage = Page.SaveSelect;
					}
				GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
		GUILayout.EndArea ();
	}
	#endregion

	#region Save Name Page
	void ShowTypeName(){
		#region Key presses
		if (Event.current.isKey && Event.current.keyCode == KeyCode.Return) {
			ClickSound.Play ();
			if (name != "")
				S.SetSaveName (name);
			else
				S.SetSaveName (" ");
			S.ResetOnNew ();
			SceneManager.Load(GenerateLevelName ());
		} 
		else if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
			currentPage = Page.Main;
		#endregion
		GUILayout.BeginArea (new Rect(Screen.width/1.5f - 240, Screen.height * .45f, 485, 150));
			GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Enter A Name For Your Save", EpochSkin.label);
				GUILayout.EndHorizontal ();
				GUI.SetNextControlName ("SaveName");
				name = GUILayout.TextField (name.Replace ("\n", "").Replace ("\r", ""), 20, EpochSkin.textField);
				GUI.FocusControl ("SaveName");
				GUILayout.Space (10);
				GUILayout.BeginHorizontal ();
					GUILayout.FlexibleSpace ();
					if (GUILayout.Button ("Enter", EpochSkin.GetStyle ("Small Button"))) {
						ClickSound.Play ();
						if(name != "")
							S.SetSaveName (name);
						else
							S.SetSaveName (" ");
						S.ResetOnNew();
						SceneManager.Load(GenerateLevelName ());
					}
				GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
		EndPage ();
		ShowBackButton ();
	}
	#endregion

	#region default Buttons
	void ShowBackButton(){
		GUILayout.BeginArea (new Rect(Screen.width - 270, Screen.height - 75, 110, 50));
		if (GUILayout.Button ("Back", EpochSkin.GetStyle("Small Button"))){
			BackSound.Play ();
			name = "";
			currentPage = prevPage;
		}
		GUILayout.EndArea ();
	}

	void ShowHomeButton(){
		GUILayout.BeginArea (new Rect(Screen.width - 150, Screen.height - 75 , 110, 50));
		if(GUILayout.Button ("Home", EpochSkin.GetStyle("Small Button"))){
			BackSound.Play ();
			currentPage = Page.Main;
			ResetVariables();
		}
		GUILayout.EndArea ();
	}

	bool MouseOver(){
		return Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect ().Contains (Event.current.mousePosition);
	}

	void PlayHover(){
		if(MouseOver ())
			if(playSound){
				//HoverSound.Play ();
				playSound = false;
			}
	}

	void ResetHover(){
		if(Event.current.type == EventType.Repaint && !GUILayoutUtility.GetLastRect ().Contains (Event.current.mousePosition))
			if(!playSound)
				playSound = true;
	}
	#endregion

	#region Generate Level Name
	private string GenerateLevelName(){
		string levelName = "Level";
		if(G.getInstance ().currentLevel > MAX_LEVELS)
			G.getInstance ().currentLevel = 1;
		levelName += G.getInstance ().currentLevel.ToString ();
		return levelName;
	}
	#endregion

	#region End page with home button
	private void EndPage() {
		GUILayout.EndArea();
		if (currentPage != Page.Main) {
			ShowHomeButton();
		}
	}
	#endregion

	#region Reset the scene
	private void ResetVariables(){
		name = "";
		characterNum = 0;
		fromLoad = false;
	}
	#endregion
}
