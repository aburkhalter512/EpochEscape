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
	private bool characterSelected = false;
	private const int MAX_CHARS = 5;
	private int characterNum = 0;
	private bool caveSelect = false;
	private bool knightSelect = false;
	private bool ninjaSelect = false;
	private bool astronautSelect = false;
	private bool mummySelect = false;
	private bool robotSelect = false;
	public Texture2D[] charTex;
	public Texture2D[] charSel;
	public Texture2D[] Backdrops;
	public Texture2D currentBackdrop;
	#endregion
    #region Credits
    private int creditPage = 0;
    public Texture2D[] creditPages;
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
		ResetSelections ();
	}

	void Update(){
        backdropImage();
	}

//	private void LoadCharTex(){
//		charTex[0] = Resources.Load("Textures/GUI/Select/CaveGirlSelect", typeof(Texture)) as Texture;
//		charTex[1] = Resources.Load("Textures/GUI/Select/KnightSelect", typeof(Texture)) as Texture;
//		if(G.getInstance().ninjaUnlocked)
//			charTex[2] = Resources.Load("Textures/GUI/Select/NinjaSelect", typeof(Texture)) as Texture;
//		else
//			charTex[2] = Resources.Load ("Textures/GUI/Select/NinjaLocked", typeof(Texture)) as Texture;
//		if(G.getInstance().astroUnlocked)
//			charTex[3] = Resources.Load("Textures/GUI/Select/AstronautSelect", typeof(Texture)) as Texture;
//		else
//			charTex[3] = Resources.Load ("Textures/GUI/Select/AstronautLocked", typeof(Texture)) as Texture;
//		if(G.getInstance().mumUnlocked)
//			charTex[4] = Resources.Load("Textures/GUI/Select/MummySelect", typeof(Texture)) as Texture;
//		else
//			charTex[4] = Resources.Load ("Textures/GUI/Select/MummyLocked", typeof(Texture)) as Texture;
//		if(G.getInstance().robUnlocked)
//			charTex[5] = Resources.Load("Textures/GUI/Select/RobotSelect", typeof(Texture)) as Texture;
//		else
//			charTex[5] = Resources.Load ("Textures/GUI/Select/RobotLocked", typeof(Texture)) as Texture;
//	}

	#region Characters
	void backdropImage(){
		if (NoCharHover())
			currentBackdrop = Backdrops[0];
		else if(caveSelect)
			currentBackdrop = Backdrops[1];
		else if(knightSelect)
			currentBackdrop = Backdrops[2];
		else if(ninjaSelect)
			currentBackdrop = Backdrops[3];
		else if(mummySelect)
			currentBackdrop = Backdrops[4];
		else if(astronautSelect)
			currentBackdrop = Backdrops[5];
		else if(robotSelect)
			currentBackdrop = Backdrops[6];
	}

	bool NoCharHover(){
		return caveSelect && knightSelect && ninjaSelect && astronautSelect && mummySelect && robotSelect;
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
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 225, Screen.height/2 - 100, 350f, 350));
			#region Main Menu Buttons
			if (GUILayout.Button ("Load Game", EpochSkin.GetStyle ("Top Button"))){
				ClickSound.Play ();	
				currentPage = Page.LoadSelect;
			}
			GUILayout.Space (10);
			if (GUILayout.Button ("New Game", EpochSkin.GetStyle ("Top Middle"))) {
					ClickSound.Play ();
                    GameObject.Find("Background Image").GetComponent<GUITexture>().texture = Resources.Load("Textures/GUI/Backgrounds/Title Menu Clear") as Texture;
                    currentBackdrop = Backdrops[0];
					characterNum = -1;
					currentPage = Page.Character;
			}
			GUILayout.Space (10);
			if (GUILayout.Button ("Options", EpochSkin.GetStyle ("Middle"))){
				ClickSound.Play ();	
				currentPage = Page.Options;
			}
			GUILayout.Space (10);
			if (GUILayout.Button ("Credits", EpochSkin.GetStyle("Bottom Middle"))){
				ClickSound.Play ();
                creditPage = 0;
				currentPage = Page.Credits;
			}
			GUILayout.Space (10);
			if (GUILayout.Button ("Quit Game", EpochSkin.GetStyle ("Bottom"))) {
				ClickSound.Play ();	
				S.Save ();
				Application.Quit ();
			}
			#endregion
		GUILayout.EndArea ();
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 300, Screen.height/2 - 175, 500f, 500));
		GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
		GUILayout.EndArea ();
	}
	#endregion

	#region Options
	void ShowOptions(){
		GUILayout.BeginArea (new Rect(Screen.width/1.5f - 210, Screen.height/2 - 115, 325, 400));
			#region Options List
			GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ();
					GUILayout.Space (95);
					GUILayout.Label ("Volume", EpochSkin.label);
				GUILayout.EndHorizontal ();

			VolumeControl ();
			GUILayout.Space (10);

			GUILayout.BeginHorizontal ();
				GUILayout.Space (85);
				GUILayout.Label ("Graphics", EpochSkin.label);
			GUILayout.EndHorizontal ();

			GraphicControl ();

			GUILayout.BeginHorizontal ();
				GUILayout.Space (115);
				GUILayout.Label ("FPS", EpochSkin.label);
				G.getInstance().showFPS = GUILayout.Toggle (G.getInstance().showFPS, "", EpochSkin.toggle);				
			GUILayout.EndHorizontal ();

//			GUILayout.BeginHorizontal ();
//				GUILayout.Space (25);
//				GUILayout.Label ("Tutorial On", EpochSkin.label);
//				G.getInstance().Tutorial = GUILayout.Toggle (G.getInstance ().Tutorial, "", EpochSkin.toggle);
//			GUILayout.EndHorizontal ();
			#endregion

		GUILayout.EndArea ();
		GUILayout.EndVertical ();
		GUILayout.BeginArea (new Rect(Screen.width - 130f, Screen.height - 75, 110, 50));
			if(GUILayout.Button ("Save", EpochSkin.GetStyle ("Small Button"))){
				ClickSound.Play ();	
				G.getInstance().SaveOptions ();
				currentPage = Page.Main;
			}
		GUILayout.EndArea ();
		GUILayout.BeginArea (new Rect(Screen.width/2 + 25, Screen.height/2 + 175, 200, 50));
			if (GUILayout.Button ("Reset Game", EpochSkin.button)) {
				ClickSound.Play ();	
				currentPage = Page.ResetWarning;
			}

		GUILayout.EndArea ();
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 300, Screen.height/2 - 175, 500f, 500));
		GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
		GUILayout.EndArea ();
	}

	private void ResetWarning(){
		GUILayout.BeginArea (new Rect(Screen.width /1.5f - 235, Screen.height/2, 375, 100));
			GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ();
					GUILayout.Label ("Reset All Progress?", EpochSkin.label);
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
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 300, Screen.height/2 - 175, 500f, 500));
		GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
		GUILayout.EndArea ();
	}

	private void VolumeControl(){
		GUILayout.BeginHorizontal ();
			//GUILayout.Label ("Off", EpochSkin.textArea);
			AudioListener.volume = GUILayout.HorizontalSlider (AudioListener.volume, 0f, 1f, EpochSkin.horizontalSlider, EpochSkin.horizontalSliderThumb);
			//GUILayout.Label ("Max", EpochSkin.textArea);
		GUILayout.EndHorizontal ();

		GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
				GUILayout.Space (100);
				GUILayout.Label ((int)(AudioListener.volume * 100) + "%", EpochSkin.textArea);
			GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
	}

	private void GraphicControl(){
		GUILayout.BeginHorizontal ();
			//GUILayout.Label ("Fastest", EpochSkin.textArea);
			G.getInstance().graphicsQuality = (int)GUILayout.HorizontalSlider ((float)G.getInstance().graphicsQuality, 1.0f, 6.0f, EpochSkin.horizontalSlider, EpochSkin.horizontalSliderThumb);
			//GUILayout.Label ("Fantastic", EpochSkin.textArea);
		GUILayout.EndHorizontal ();
		GUILayout.BeginVertical ();
					#region Quality Labels
					GUILayout.BeginHorizontal ();
					GUILayout.Space (90);
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
		GUILayout.BeginArea (new Rect(-10, 0, Screen.width + 15, Screen.height - 150));
		GUILayout.Box (currentBackdrop, EpochSkin.GetStyle ("Backdrop"));
		GUILayout.EndArea ();

		GUILayout.BeginArea (new Rect(50, 175, 130, 150));
		if (GUILayout.Button ("", EpochSkin.GetStyle ("CaveGirl"))) {
			ClickSound.Play ();
			#region reset bools
			caveSelect = true;
			knightSelect = false;
			ninjaSelect = false;
			astronautSelect = false;
			mummySelect = false;
			robotSelect = false;
			#endregion
			characterNum = G.CAVEGIRL;
			#region reset normals
			EpochSkin.GetStyle ("CaveGirl").normal.background = charSel[0];
			EpochSkin.GetStyle ("Knight").normal.background = charTex[1];
			EpochSkin.GetStyle ("Ninja").normal.background = charTex[2];
			EpochSkin.GetStyle ("Mummy").normal.background = charTex[3];
			EpochSkin.GetStyle ("Astronaut").normal.background = charTex[4];
			EpochSkin.GetStyle ("Robot").normal.background = charTex[5];
			#endregion
		}
		GUILayout.EndArea ();

        GUILayout.BeginArea(new Rect(215, 175, 130, 150));
        if (GUILayout.Button("", EpochSkin.GetStyle("Knight")))
        {
            ClickSound.Play();
            #region reset bools
            caveSelect = false;
            knightSelect = true;
            ninjaSelect = false;
            astronautSelect = false;
            mummySelect = false;
            robotSelect = false;
            #endregion
            characterNum = G.KNIGHT;
            #region reset normals
            EpochSkin.GetStyle("CaveGirl").normal.background = charTex[0];
            EpochSkin.GetStyle("Knight").normal.background = charSel[1];
            EpochSkin.GetStyle("Ninja").normal.background = charTex[2];
            EpochSkin.GetStyle("Mummy").normal.background = charTex[3];
            EpochSkin.GetStyle("Astronaut").normal.background = charTex[4];
            EpochSkin.GetStyle("Robot").normal.background = charTex[5];
            #endregion
        }
        GUILayout.EndArea();

        GUILayout.BeginArea (new Rect(375, 185, 110, 140));
		if (GUILayout.Button ("", EpochSkin.GetStyle ("Ninja"))) {
			ClickSound.Play ();
			#region reset bools
			caveSelect = false;
			knightSelect = false;
			ninjaSelect = true;
			astronautSelect = false;
			mummySelect = false;
			robotSelect = false;
			#endregion
			characterNum = G.NINJA;
			#region reset normals
			EpochSkin.GetStyle ("CaveGirl").normal.background = charTex[0];
			EpochSkin.GetStyle ("Knight").normal.background = charTex[1];
			EpochSkin.GetStyle ("Ninja").normal.background = charSel[2];
			EpochSkin.GetStyle ("Mummy").normal.background = charTex[3];
			EpochSkin.GetStyle ("Astronaut").normal.background = charTex[4];
			EpochSkin.GetStyle ("Robot").normal.background = charTex[5];
			#endregion
		}
        GUILayout.EndArea();

		GUILayout.BeginArea (new Rect(535, 175, 110, 150));
		if (GUILayout.Button ("", EpochSkin.GetStyle ("Mummy"))) {
			ClickSound.Play ();
			#region reset bools
			caveSelect = false;
			knightSelect = false;
			ninjaSelect = false;
			astronautSelect = false;
			mummySelect = true;
			robotSelect = false;
			#endregion
			characterNum = G.MUMMY;
			#region reset normals
			EpochSkin.GetStyle ("CaveGirl").normal.background = charTex[0];
			EpochSkin.GetStyle ("Knight").normal.background = charTex[1];
			EpochSkin.GetStyle ("Ninja").normal.background = charTex[2];
			EpochSkin.GetStyle ("Mummy").normal.background = charSel[3];
			EpochSkin.GetStyle ("Astronaut").normal.background = charTex[4];
			EpochSkin.GetStyle ("Robot").normal.background = charTex[5];
			#endregion
		}
		GUILayout.EndArea ();

        GUILayout.BeginArea(new Rect(695, 175, 110, 150));
        if (GUILayout.Button("", EpochSkin.GetStyle("Astronaut")))
        {
            ClickSound.Play();
            #region reset bools
            caveSelect = false;
            knightSelect = false;
            ninjaSelect = false;
            astronautSelect = true;
            mummySelect = false;
            robotSelect = false;
            #endregion
            characterNum = G.ASTRONAUT;
            #region reset normals
            EpochSkin.GetStyle("CaveGirl").normal.background = charTex[0];
            EpochSkin.GetStyle("Knight").normal.background = charTex[1];
            EpochSkin.GetStyle("Ninja").normal.background = charTex[2];
            EpochSkin.GetStyle("Mummy").normal.background = charTex[3];
            EpochSkin.GetStyle("Astronaut").normal.background = charSel[4];
            EpochSkin.GetStyle("Robot").normal.background = charTex[5];
            #endregion
        }
        GUILayout.EndArea();

		GUILayout.BeginArea (new Rect(860, 175, 90, 150));
		if (GUILayout.Button ("", EpochSkin.GetStyle ("Robot"))) {
			ClickSound.Play ();
			#region reset bools
			caveSelect = false;
			knightSelect = false;
			ninjaSelect = false;
			astronautSelect = false;
			mummySelect = false;
			robotSelect = true;
			#endregion
			characterNum = G.ROBOT;
			#region reset normals
			EpochSkin.GetStyle ("CaveGirl").normal.background = charTex[0];
			EpochSkin.GetStyle ("Knight").normal.background = charTex[1];
			EpochSkin.GetStyle ("Ninja").normal.background = charTex[2];
			EpochSkin.GetStyle ("Mummy").normal.background = charTex[3];
			EpochSkin.GetStyle ("Astronaut").normal.background = charTex[4];
			EpochSkin.GetStyle ("Robot").normal.background = charSel[5];
			#endregion
		}
		GUILayout.EndArea ();

		#region Select Button
		GUILayout.BeginArea (new Rect(Screen.width - 250, Screen.height - 75 , 110, 100));
			if (GUILayout.Button ("Select", EpochSkin.GetStyle ("Small Button"))) {
				ClickSound.Play ();	
				if(CharIsUnlocked ()){
                    GameObject.Find("Background Image").GetComponent<GUITexture>().texture = Resources.Load("Textures/GUI/Backgrounds/Title Menu") as Texture;
					ResetSelections ();
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
		#endregion
		#region Home Button
		GUILayout.BeginArea (new Rect(Screen.width - 125, Screen.height - 75 , 110, 50));
		if(GUILayout.Button ("Home", EpochSkin.GetStyle("Small Button"))){
            GameObject.Find("Background Image").GetComponent<GUITexture>().texture = Resources.Load("Textures/GUI/Backgrounds/Title Menu") as Texture;
			BackSound.Play ();
			currentPage = Page.Main;
			ResetSelections ();
		}
		GUILayout.EndArea ();
		#endregion
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
				if (G.getInstance().ninjaUnlocked)
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
	#endregion

	#region Credits Page
	void ShowCredits(){
        string buttonMessage = (creditPage + 1) + "/2";
     
        GUILayout.BeginArea(new Rect(Screen.width / 1.5f - 300, Screen.height / 2 - 275, 500f, 700));
		GUILayout.Box (creditPages[creditPage], EpochSkin.GetStyle ("Credits"));
		EndPage ();

        GUILayout.BeginArea(new Rect(Screen.width /1.5f - 100, Screen.height / 2 + 175, 110f, 50));
        if (GUILayout.Button(buttonMessage, EpochSkin.GetStyle("Small Button")))
        {
            if (creditPage == 1)
                creditPage--;
            else if (creditPage == 0)
                creditPage++;
        }
        GUILayout.EndArea();
	}
	#endregion

	#region Load Selection
	void ShowLoad(){
		GUILayout.BeginArea (new Rect (Screen.width/1.5f - 225, Screen.height/2 - 90, 350, 300));
			GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ();
					GUILayout.Label ("Choose Your Slot", EpochSkin.label);
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
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 300, Screen.height/2 - 175, 500f, 500));
		GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
		GUILayout.EndArea ();
	}

	void DeleteWarning(){
		GUILayout.BeginArea (new Rect(Screen.width/1.5f - 200f, Screen.height/2, 300, 100));
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
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 300, Screen.height/2 - 175, 500f, 500));
		GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
		GUILayout.EndArea ();
	}

	void ShowLoadWarning(){
		GUILayout.BeginArea (new Rect(Screen.width/1.5f - 200f, Screen.height/2, 300, 150));
			GUILayout.BeginVertical ();
				GUILayout.Label ("No Save File! \nStart Save?", EpochSkin.label);
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
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 300, Screen.height/2 - 175, 500f, 500));
		GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
		GUILayout.EndArea ();
	}
	#endregion

	#region Save Selection
	void ShowSaveSelect(){
		prevPage = Page.Character;
		GUILayout.BeginArea (new Rect (Screen.width/1.5f - 225, Screen.height/2 - 90, 350, 300));
			#region buttons
			GUILayout.BeginVertical ();
			GUILayout.Label ("Choose Your Slot", EpochSkin.label);
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
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 300, Screen.height/2 - 175, 500f, 500));
		GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
		GUILayout.EndArea ();
	}

	void ShowSaveWarning (){
		GUILayout.BeginArea (new Rect(Screen.width/1.5f - 200f, Screen.height/2, 300, 150));
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
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 300, Screen.height/2 - 175, 500f, 500));
		GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
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
			SceneManager.Load("Story");
		} 
		else if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
			currentPage = Page.Main;
		#endregion
		GUILayout.BeginArea (new Rect(Screen.width/1.5f - 200, Screen.height/2, 300, 150));
			GUILayout.BeginVertical ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Enter A Name", EpochSkin.label);
				GUILayout.EndHorizontal ();
				GUI.SetNextControlName ("SaveName");
				name = GUILayout.TextField (name.Replace ("\n", "").Replace ("\r", ""), 12, EpochSkin.textField);
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
						SceneManager.Load("Story");
					}
				GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
		EndPage ();
		ShowBackButton ();
		GUILayout.BeginArea (new Rect (Screen.width /1.5f - 300, Screen.height/2 - 175, 500f, 500));
		GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
		GUILayout.EndArea ();
	}
	#endregion

	#region default Buttons
	void ShowBackButton(){
		GUILayout.BeginArea (new Rect(Screen.width - 125, Screen.height - 135, 110, 50));
		if (GUILayout.Button ("Back", EpochSkin.GetStyle("Small Button"))){
			BackSound.Play ();
			name = "";
			currentPage = prevPage;
		}
		GUILayout.EndArea ();
	}

	void ShowHomeButton(){
		GUILayout.BeginArea (new Rect(Screen.width - 125, Screen.height - 75 , 110, 50));
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
	private void ResetSelections(){
		caveSelect = false;
		EpochSkin.GetStyle ("CaveGirl").normal.background = charTex [0];
		ninjaSelect = false;
		EpochSkin.GetStyle ("Knight").normal.background = charTex [1];
		knightSelect = false;
		EpochSkin.GetStyle ("Ninja").normal.background = charTex [2];
		mummySelect = false;
		EpochSkin.GetStyle ("Mummy").normal.background = charTex [3];
		astronautSelect = false;
		EpochSkin.GetStyle ("Astronaut").normal.background = charTex [4];
		robotSelect = false;
		EpochSkin.GetStyle ("Robot").normal.background = charTex [5];
        currentBackdrop = Backdrops[0];
	}
	private void ResetVariables(){
		name = "";
		characterNum = 0;
		fromLoad = false;
	}
	#endregion
}
