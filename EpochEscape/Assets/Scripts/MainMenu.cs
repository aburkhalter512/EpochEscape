using UnityEngine;
using System.Collections;
using G = GameManager;
using S = SaveManager;

public class MainMenu : MonoBehaviour
{
    #region Levels
    public int MAX_LEVELS = 8;
    #endregion
    #region Styles
    public GUISkin EpochSkin = null;
    #endregion
    #region Options
    private string mName;
    #endregion
    #region Audio
    public AudioSource BackSound;
    public AudioSource ClickSound;
    #endregion
    #region Character Select
    private bool fromLoad = false;
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
        G.SetGUIMatrix();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.Find("Background Image").GetComponent<GUITexture>().texture = Resources.Load("Textures/GUI/Backgrounds/Title Menu") as Texture;
            currentPage = Page.Main;
        }
        switch (currentPage) {
            case Page.Main: ShowMain (); break;
            case Page.Options: ShowOptions(); break;
            case Page.ResetWarning: ResetWarning(); break;
            case Page.Character:
                GameObject.Find("Background Image").GetComponent<GUITexture>().texture = Resources.Load("Textures/GUI/Backgrounds/Title Menu Clear") as Texture;
                SelectMenu(); 
                break;
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
        GUILayout.BeginArea (new Rect (500, 260, 400, 350));
            #region Main Menu Buttons
            if (GUILayout.Button ("Load Game", EpochSkin.GetStyle ("Top Button"))){
                ClickSound.Play ();	
                currentPage = Page.LoadSelect;
            }
            G.getInstance().PlayHover(0);
            GUILayout.Space (10);
            if (GUILayout.Button ("New Game", EpochSkin.GetStyle ("Top Middle"))) {
                    ClickSound.Play ();
                    currentBackdrop = Backdrops[0];
                    characterNum = -1;
                    currentPage = Page.Character;
            }
            G.getInstance().PlayHover(1);
            GUILayout.Space (10);
            if (GUILayout.Button ("Options", EpochSkin.GetStyle ("Middle"))){
                ClickSound.Play ();	
                currentPage = Page.Options;
            }
            G.getInstance().PlayHover(2);
            GUILayout.Space (10);
            if (GUILayout.Button ("Credits", EpochSkin.GetStyle("Bottom Middle"))){
                ClickSound.Play ();
                creditPage = 0;
                currentPage = Page.Credits;
            }
            G.getInstance().PlayHover(3);
            GUILayout.Space (10);
            if (GUILayout.Button ("Quit Game", EpochSkin.GetStyle ("Bottom"))) {
                ClickSound.Play ();	
                S.Save ();
                Application.Quit ();
            }
            G.getInstance().PlayHover(4);
            #endregion
        GUILayout.EndArea ();
        GUILayout.BeginArea (new Rect (425, 185, 550, 500));
        GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
        GUILayout.EndArea ();
    }
    #endregion

    #region Options
    void ShowOptions(){
        GUILayout.BeginArea (new Rect(500, 250, 400, 300));
            #region Options List
            GUILayout.BeginVertical ();
                GUILayout.BeginHorizontal ();
                    GUILayout.Space(125);
                    GUILayout.Label ("Volume", EpochSkin.label);
                GUILayout.EndHorizontal ();

            VolumeControl ();
            GUILayout.Space (10);

            GUILayout.BeginHorizontal ();
                GUILayout.Space (115);
                GUILayout.Label ("Graphics", EpochSkin.label);
            GUILayout.EndHorizontal ();

            GraphicControl ();

            GUILayout.BeginHorizontal ();
                GUILayout.Space (100);
                GUILayout.Label ("Show FPS", EpochSkin.label);
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
        GUILayout.BeginArea (new Rect(900, 695 , 110, 50));
            if(GUILayout.Button ("Save", EpochSkin.GetStyle ("Small Button"))){
                ClickSound.Play ();	
                G.getInstance().SaveOptions ();
                currentPage = Page.Main;
            }
            G.getInstance().PlayHover(0);
        GUILayout.EndArea ();
        GUILayout.BeginArea (new Rect(575, 550, 250, 50));
            if (GUILayout.Button ("Reset Game", EpochSkin.button)) {
                ClickSound.Play ();	
                currentPage = Page.ResetWarning;
            }
            G.getInstance().PlayHover(1);

        GUILayout.EndArea ();
        GUILayout.BeginArea (new Rect (425, 185, 550, 500));
        GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
        GUILayout.EndArea ();
    }

    private void ResetWarning(){
        GUILayout.BeginArea (new Rect(500, 375, 400, 100));
            GUILayout.BeginVertical ();
                GUILayout.BeginHorizontal ();
                    GUILayout.Label ("Reset Progress?", EpochSkin.label);
                GUILayout.EndHorizontal ();

                GUILayout.BeginHorizontal ();
                    if(GUILayout.Button ("Yes", EpochSkin.button)){
                        ClickSound.Play ();	
                        S.ResetGame ();
                        currentPage = Page.Options;
                    }
                    G.getInstance().PlayHover(0);
                    GUILayout.Space (10);
                    if(GUILayout.Button ("No", EpochSkin.button)){
                        ClickSound.Play ();	
                        currentPage = Page.Options;
                    }
                    G.getInstance().PlayHover(1);
                GUILayout.EndHorizontal ();

            GUILayout.EndVertical ();
        GUILayout.EndArea ();
        GUILayout.BeginArea(new Rect(425, 185, 550, 500));
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
            GUILayout.Space(125);
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
                    GUILayout.Space(125);
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
                    GUILayout.Space(10);
        GUILayout.EndVertical ();
    }
    #endregion

    #region Character Selection
    void SelectMenu(){
        GUILayout.BeginArea (new Rect(0, 0, 1024, 768));
        GUILayout.Box (currentBackdrop, EpochSkin.GetStyle ("Backdrop"));
        GUILayout.EndArea ();

        GUILayout.BeginArea (new Rect(0, 200, 1024, 200));
        GUILayout.BeginHorizontal();
        GUILayout.Space(60);
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
        G.getInstance().PlayHover(0);
        GUILayout.Space(35);
        if (GUILayout.Button("", EpochSkin.GetStyle("Knight"))) {
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
        G.getInstance().PlayHover(1);
        GUILayout.Space(30);
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
        G.getInstance().PlayHover(2);
        GUILayout.Space(30);
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
        G.getInstance().PlayHover(3);
        GUILayout.Space(30);
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
        G.getInstance().PlayHover(4);
        GUILayout.Space(35);
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
        G.getInstance().PlayHover(5);
        GUILayout.EndHorizontal();
        GUILayout.EndArea ();

        #region Select Button
        GUILayout.BeginArea(new Rect(775, 695, 110, 50));
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
            G.getInstance().PlayHover(6);
        GUILayout.EndArea ();
        #endregion
        #region Home Button
        GUILayout.BeginArea(new Rect(900, 695, 110, 50));
        if(GUILayout.Button ("Home", EpochSkin.GetStyle("Small Button"))){
            GameObject.Find("Background Image").GetComponent<GUITexture>().texture = Resources.Load("Textures/GUI/Backgrounds/Title Menu") as Texture;
            BackSound.Play ();
            currentPage = Page.Main;
            ResetSelections ();
        }
        G.getInstance().PlayHover(7);
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
     
        GUILayout.BeginArea(new Rect (Screen.width /2.25f, Screen.height/3.5f, Screen.width/2, Screen.height/1.5f));
        GUILayout.Box (creditPages[creditPage], EpochSkin.GetStyle ("Credits"));
        EndPage ();

        GUILayout.BeginArea(new Rect (Screen.width /1.5f, Screen.height/1.35f, Screen.width/2, Screen.height/1.5f));
        if (GUILayout.Button(buttonMessage, EpochSkin.GetStyle("Small Button")))
        {
            ClickSound.Play();
            if (creditPage == 1)
                creditPage--;
            else if (creditPage == 0)
                creditPage++;
        }
        G.getInstance().PlayHover(0);
        GUILayout.EndArea();
    }
    #endregion

    #region Load Selection
    void ShowLoad(){
        GUILayout.BeginArea (new Rect (500, 275, 400, 300));
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
                G.getInstance().PlayHover(0);
                GUILayout.Space (10);
                if (GUILayout.Button ("Delete", EpochSkin.GetStyle ("Small Button"))) {
                    ClickSound.Play ();	
                    S.ChooseSave (0);
                    if(S.SaveTaken ())
                        currentPage = Page.DeleteWarning;
                }
                G.getInstance().PlayHover(1);
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
                G.getInstance().PlayHover(2);
                GUILayout.Space (10);
                if (GUILayout.Button ("Delete", EpochSkin.GetStyle ("Small Button"))) {
                    ClickSound.Play ();
                    S.ChooseSave (1);
                    if(S.SaveTaken())
                        currentPage = Page.DeleteWarning;
                }
                G.getInstance().PlayHover(3);
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
                G.getInstance().PlayHover(4);
                GUILayout.Space (10);
                if (GUILayout.Button ("Delete", EpochSkin.GetStyle ("Small Button"))) {
                    ClickSound.Play ();
                    S.ChooseSave (2);
                    if(S.SaveTaken ())
                        currentPage = Page.DeleteWarning;
                }
                G.getInstance().PlayHover(5);
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
            G.getInstance().PlayHover(6);
            GUILayout.Space (10);
            if (GUILayout.Button ("Delete", EpochSkin.GetStyle ("Small Button"))) {
                ClickSound.Play ();
                S.ChooseSave (3);
                if(S.SaveTaken ())
                    currentPage = Page.DeleteWarning;
            }
            G.getInstance().PlayHover(7);
            GUILayout.EndHorizontal ();
            #endregion
            GUILayout.EndVertical ();
        EndPage ();
        GUILayout.BeginArea(new Rect(425, 185, 550, 500));
        GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
        GUILayout.EndArea ();
    }

    void DeleteWarning(){
        GUILayout.BeginArea(new Rect(500, 375, 400, 100));
            GUILayout.BeginVertical ();
                GUILayout.Label ("Really Delete?", EpochSkin.label);
                    #region Buttons
                    GUILayout.BeginHorizontal ();
                    if (GUILayout.Button ("Yes", EpochSkin.button)) {
                        ClickSound.Play ();
                        S.DeleteSave ();
                        currentPage = Page.LoadSelect;
                    }
                    G.getInstance().PlayHover(0);
                    GUILayout.Space (10);
                    if (GUILayout.Button ("No", EpochSkin.button)) {
                        ClickSound.Play ();
                        currentPage = Page.LoadSelect;
                    }
                    G.getInstance().PlayHover(1);
                    GUILayout.EndHorizontal ();
                    #endregion
            GUILayout.EndVertical ();
        GUILayout.EndArea ();
        GUILayout.BeginArea(new Rect(425, 185, 550, 500));
        GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
        GUILayout.EndArea ();
    }

    void ShowLoadWarning(){
        GUILayout.BeginArea(new Rect(500, 325, 400, 150));
            GUILayout.BeginVertical ();
                GUILayout.Label ("No Save File! \nStart Save?", EpochSkin.label);
                    #region Buttons
                    GUILayout.BeginHorizontal ();
                    if (GUILayout.Button ("Yes", EpochSkin.button)) {
                        ClickSound.Play ();
                        fromLoad = true;
                        currentPage = Page.Character;
                    }
                    G.getInstance().PlayHover(0);
                    GUILayout.Space (10);
                    if (GUILayout.Button ("No", EpochSkin.button)) {
                        ClickSound.Play ();
                        currentPage = Page.LoadSelect;
                    }
                    G.getInstance().PlayHover(1);
                    GUILayout.EndHorizontal ();
                    #endregion
            GUILayout.EndVertical ();
        GUILayout.EndArea ();
        GUILayout.BeginArea(new Rect(425, 185, 550, 500));
        GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
        GUILayout.EndArea ();
    }
    #endregion

    #region Save Selection
    void ShowSaveSelect(){
        prevPage = Page.Character;
        GUILayout.BeginArea(new Rect(500, 275, 400, 300));
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
            G.getInstance().PlayHover(0);
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
            G.getInstance().PlayHover(1);
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
            G.getInstance().PlayHover(2);
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
            G.getInstance().PlayHover(3);
            GUILayout.EndVertical ();
            #endregion
        EndPage ();
        ShowBackButton ();
        GUILayout.BeginArea(new Rect(425, 185, 550, 500));
        GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
        GUILayout.EndArea ();
    }

    void ShowSaveWarning (){
        GUILayout.BeginArea(new Rect(500, 325, 400, 150));
            GUILayout.BeginVertical ();
                GUILayout.Label ("Save File Exists! Overwrite?", EpochSkin.label);
                GUILayout.BeginHorizontal ();
                    if(GUILayout.Button ("Yes", EpochSkin.button)){
                        ClickSound.Play ();
                        S.ResetOnNew ();
                        prevPage = Page.SaveSelect;
                        currentPage = Page.SaveName;
                    }
                    G.getInstance().PlayHover(0);
                    GUILayout.Space (10);
                    if(GUILayout.Button ("No", EpochSkin.button)){
                        ClickSound.Play ();
                        currentPage = Page.SaveSelect;
                    }
                    G.getInstance().PlayHover(1);
                GUILayout.EndHorizontal ();
            GUILayout.EndVertical ();
        GUILayout.EndArea ();
        GUILayout.BeginArea(new Rect(425, 185, 550, 500));
        GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
        GUILayout.EndArea ();
    }
    #endregion

    #region Save Name Page
    void ShowTypeName(){
        #region Key presses
        if (Event.current.isKey && Event.current.keyCode == KeyCode.Return) {
            ClickSound.Play ();
            if (mName != "")
                S.SetSaveName (mName);
            else
                S.SetSaveName (" ");
            S.ResetOnNew ();
            SceneManager.Load("Story");
        } 
        else if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
            currentPage = Page.Main;
        #endregion
        GUILayout.BeginArea (new Rect(500, 350, 400, 150));
            GUILayout.BeginVertical ();
                GUILayout.BeginHorizontal ();
                GUILayout.Label ("Enter A Name", EpochSkin.label);
                GUILayout.EndHorizontal ();
                GUI.SetNextControlName ("SaveName");
                mName = GUILayout.TextField (mName.Replace ("\n", "").Replace ("\r", ""), 12, EpochSkin.textField);
                GUI.FocusControl ("SaveName");
                GUILayout.Space (10);
                GUILayout.BeginHorizontal ();
                    GUILayout.FlexibleSpace ();
                    if (GUILayout.Button ("Enter", EpochSkin.GetStyle ("Small Button"))) {
                        ClickSound.Play ();
                        if(mName != "")
                            S.SetSaveName (mName);
                        else
                            S.SetSaveName (" ");
                        S.ResetOnNew();
                        SceneManager.Load("Story");
                    }
                    G.getInstance().PlayHover(0);
                GUILayout.EndHorizontal ();
            GUILayout.EndVertical ();
        EndPage ();
        ShowBackButton ();
        GUILayout.BeginArea(new Rect(425, 185, 550, 500));
        GUILayout.Box ("", EpochSkin.GetStyle ("Tablet Wide"));
        GUILayout.EndArea ();
    }
    #endregion

    #region default Buttons
    void ShowBackButton(){
        GUILayout.BeginArea (new Rect(775, 695, 110, 50));
        if (GUILayout.Button ("Back", EpochSkin.GetStyle("Small Button"))){
            BackSound.Play ();
            mName = "";
            currentPage = prevPage;
        }
        G.getInstance().PlayHover(8);
        GUILayout.EndArea ();
    }

    void ShowHomeButton(){
        GUILayout.BeginArea (new Rect(900, 695 , 110, 50));
        if(GUILayout.Button ("Home", EpochSkin.GetStyle("Small Button"))){
            BackSound.Play ();
            currentPage = Page.Main;
            ResetVariables();
        }
        G.getInstance().PlayHover(9);
        GUILayout.EndArea ();
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
        mName = "";
        characterNum = 0;
        fromLoad = false;
    }
    #endregion
}
