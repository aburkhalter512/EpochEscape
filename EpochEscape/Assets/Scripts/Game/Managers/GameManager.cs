using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using S = MapEditor.SaveManager;

namespace Game
{
    public class GameManager : Manager<GameManager>
    {
        #region Aspect Ratio
        public const float LAYOUT_WIDTH = 1024f;
        public const float LAYOUT_HEIGHT = 768f;
        #endregion

        #region Skin
        public GUISkin EpochSkin = null;
        #endregion

        #region Audio
        public AudioSource ClickSound;
        public AudioSource PopUp;
        public AudioSource PopUpExit;
        public AudioSource BackSound;
        public AudioSource HoverSound;
        public bool playPopUpSound = false;
        public bool[] PlayHoverSound;
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
        public enum Page
        {
            Main, Options
        }
        private Page currentPage = Page.Main;
        #endregion

        #region Option variables
        public int graphicsQuality = 4;
        #endregion
        #endregion

        #region Class Constants
        public static readonly float DEFAULT_DELAY_TIME = 0.1f;
        #endregion

        protected override void Awaken()
        {
        }

        protected override void Initialize()
        {
            Application.targetFrameRate = 60;
            LoadFPS();
            LoadOptions();
            //S.Load();
            PlayHoverSound = new bool[10];
        }

        void Update()
        {
            UpdateKeyboard();
            ShowFPSText();
        }

        public static void SetGUIMatrix()
        {
            Vector3 scale = new Vector3(Screen.width / LAYOUT_WIDTH, Screen.height / LAYOUT_HEIGHT, 1f);
            UnityEngine.GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
        }

        void OnGUI()
        {
            SetGUIMatrix();
            Pause();
            ShowPopupMessage();
        }

        #region Update Methods
        private void UpdateKeyboard()
        {
            #region Escape
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && !popup)
            {
                if (Application.loadedLevelName != "MainMenu" && Application.loadedLevelName != "Loading" && Application.loadedLevelName != "Story" && !paused)
                    PauseGame();
                else if (paused && currentPage == Page.Main)
                    UnpauseGame();
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && currentPage != Page.Main)
                {
                    currentPage = Page.Main;
                }
            }
            #endregion
        }
        #endregion

        #region Pause Menu Methods
        void Pause()
        {
            if (paused && ShowPauseMenu)
                switch (currentPage)
                {
                    case Page.Main: showPauseMenu(); break;
                    case Page.Options: ShowOptions(); break;
                }
        }

        void showPauseMenu()
        {
            GUILayout.BeginArea(new Rect(300, 200, 450, 350));
            #region Pause Menu Options
            GUILayout.BeginVertical();
            if (GUILayout.Button("Continue", EpochSkin.GetStyle("Top Button")))
            {
                ClickSound.Play();
                UnpauseGame();
            }
            PlayHover(0);
            GUILayout.Space(10);
            if (GUILayout.Button("Main Menu", EpochSkin.GetStyle("Top Middle")))
            {
                ClickSound.Play();
                //S.Save();
                Application.LoadLevel("MainMenu");
                UnpauseGame();
            }
            PlayHover(1);
            GUILayout.Space(10);
            if (GUILayout.Button("Save Game", EpochSkin.GetStyle("Middle")))
            {
                ClickSound.Play();
                //S.Save();
            }
            PlayHover(2);
            GUILayout.Space(10);
            if (GUILayout.Button("Options", EpochSkin.GetStyle("Bottom Middle")))
            {
                ClickSound.Play();
                currentPage = Page.Options;
            }
            PlayHover(3);
            GUILayout.Space(10);
            if (GUILayout.Button("Restart Level", EpochSkin.GetStyle("Bottom")))
            {
                ClickSound.Play();
                UnpauseGame();

                LevelManager.RestartLevel();
            }
            PlayHover(4);
            GUILayout.Space(10);
            if (GUILayout.Button("Quit Game", EpochSkin.GetStyle("Bottom")))
            {
                ClickSound.Play();
                //S.Save();
                Application.Quit();
            }
            PlayHover(5);
            GUILayout.EndVertical();
            #endregion
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(250, 125, 550, 500));
            GUILayout.Box("", EpochSkin.GetStyle("Tablet Wide"));
            GUILayout.EndArea();
        }

        #region Pause Methods
        public void UnpauseGame()
        {
            paused = false;
            Time.timeScale = 1f;
        }

        public void PauseGame()
        {
            paused = true;
            Time.timeScale = 0f;
            ShowPauseMenu = true;
        }

        public void PauseMovement()
        {
            paused = true;
            ShowPauseMenu = false;
        }

        public void UnpauseMovement()
        {
            paused = false;
        }

        public void PauseMovementTS()
        {
            paused = true;
            ShowPauseMenu = false;
            Time.timeScale = 0f;
        }

        public void UnpauseMovementTS()
        {
            paused = false;
            Time.timeScale = 1f;
        }
        #endregion

        #region Options
        void ShowOptions()
        {
            #region Options List
            GUILayout.BeginArea(new Rect(325, 200, 400, 300));
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(125);
            GUILayout.Label("Volume", EpochSkin.label);
            GUILayout.EndHorizontal();

            VolumeControl();

            GUILayout.BeginHorizontal();
            GUILayout.Space(125);
            GUILayout.Label("Graphics", EpochSkin.label);
            GUILayout.EndHorizontal();

            GraphicControl();

            GUILayout.BeginHorizontal();
            GUILayout.Space(115);
            GUILayout.Label("Show FPS", EpochSkin.label);
            showFPS = GUILayout.Toggle(showFPS, "", EpochSkin.toggle);
            GUILayout.EndHorizontal();

            // GUILayout.BeginHorizontal ();
            // GUILayout.Space (25);
            // GUILayout.Label ("Tutorial On", EpochSkin.label);
            // G.getInstance().Tutorial = GUILayout.Toggle (G.getInstance ().Tutorial, "", EpochSkin.toggle);
            // GUILayout.EndHorizontal ();
            #endregion

            GUILayout.EndArea();
            GUILayout.EndVertical();
            GUILayout.BeginArea(new Rect(475, 500, 125, 50));
            if (GUILayout.Button("Save", EpochSkin.GetStyle("Small Button")))
            {
                ClickSound.Play();
                SaveOptions();
                currentPage = Page.Main;
            }
            PlayHover(0);
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(250, 125, 550, 500));
            GUILayout.Box("", EpochSkin.GetStyle("Tablet Wide"));
            GUILayout.EndArea();
        }

        private void VolumeControl()
        {
            GUILayout.BeginHorizontal();
            //GUILayout.Label ("Off", EpochSkin.textArea);
            AudioListener.volume = GUILayout.HorizontalSlider(AudioListener.volume, 0f, 1f, EpochSkin.horizontalSlider, EpochSkin.horizontalSliderThumb);
            //GUILayout.Label ("Max", EpochSkin.textArea);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(125);
            GUILayout.Label((int)(AudioListener.volume * 100) + "%", EpochSkin.textArea);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void GraphicControl()
        {
            GUILayout.BeginHorizontal();
            //GUILayout.Label ("Fastest", EpochSkin.textArea);
            graphicsQuality = (int)GUILayout.HorizontalSlider((float)graphicsQuality, 1.0f, 6.0f, EpochSkin.horizontalSlider, EpochSkin.horizontalSliderThumb);
            //GUILayout.Label ("Fantastic", EpochSkin.textArea);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            #region Quality Labels
            GUILayout.BeginHorizontal();
            GUILayout.Space(125);
            switch (graphicsQuality)
            {
                case 1:
                    QualitySettings.SetQualityLevel(1);
                    GUILayout.Label("Fastest", EpochSkin.textArea);
                    break;
                case 2:
                    QualitySettings.SetQualityLevel(2);
                    GUILayout.Label("Fast", EpochSkin.textArea);
                    break;
                case 3:
                    QualitySettings.SetQualityLevel(3);
                    GUILayout.Label("Simple", EpochSkin.textArea);
                    break;
                case 4:
                    QualitySettings.SetQualityLevel(4);
                    GUILayout.Label("Good", EpochSkin.textArea);
                    break;
                case 5:
                    QualitySettings.SetQualityLevel(5);
                    GUILayout.Label("Beautiful", EpochSkin.textArea);
                    break;
                case 6:
                    QualitySettings.SetQualityLevel(6);
                    GUILayout.Label("Fantastic", EpochSkin.textArea);
                    break;
            }
            GUILayout.EndHorizontal();
            #endregion
            GUILayout.EndVertical();
        }
        #region FPS
        private void LoadFPS()
        {
            if (FPS == null)
            {
                GameObject go = new GameObject();
                go.name = "FPS";

                FPS = (GUIText)go.AddComponent(typeof(GUIText));
                FPS.anchor = TextAnchor.UpperRight;
                FPS.transform.position = new Vector3(1f, 1f, 0f);
                FPS.text = "";
            }
        }

        private void ShowFPSText()
        {
            LoadFPS();
            if (showFPS)
            {
                timeLeft -= Time.deltaTime;
                accum += Time.timeScale / Time.deltaTime;
                ++frames;
                FPSControl();
            }
            else
                FPS.text = "";
        }

        private void FPSControl()
        {
            if (showFPS)
            {
                if (timeLeft <= 0.0f)
                {
                    float fps = accum / frames;
                    string fpsFormat = System.String.Format("{0:F2} FPS", fps);
                    FPS.text = fpsFormat;
                    if (fps < 30)
                        FPS.material.color = Color.yellow;
                    else if (fps < 10)
                        FPS.material.color = Color.red;
                    else
                        FPS.material.color = Color.green;
                    timeLeft = updateInterval;
                    accum = 0.0f;
                    frames = 0;
                }
            }
        }

        public void SaveOptions()
        {
            PlayerPrefs.SetFloat("Volume", AudioListener.volume);
            PlayerPrefs.SetInt("Graphics", graphicsQuality);
            PlayerPrefs.SetInt("FPS", showFPS == true ? 1 : 0);
        }

        private void LoadOptions()
        {
            graphicsQuality = PlayerPrefs.GetInt("Graphics");
            if (!PlayerPrefs.HasKey("Graphics"))
                graphicsQuality = 4;
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
            if (!PlayerPrefs.HasKey("Volume"))
                AudioListener.volume = 1f;
            showFPS = PlayerPrefs.GetInt("FPS") == 1 ? true : false;
        }
        #endregion
        #endregion
        #endregion

        #region Popups
        public void ShowPopupMessage()
        {
            if (popup && Tutorial)
            {
                if (playPopUpSound)
                {
                    PopUp.Play();
                    playPopUpSound = false;
                }
                if (PopupPage + 1 < messages.Count)
                    PopupButtonText = "Next";
                else
                    PopupButtonText = "OK";
                PauseMovementTS();
                #region Key Presses
                if (Event.current.type == EventType.KeyUp && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Space))
                {
                    ClickSound.Play();
                    if (PopupPage + 1 == messages.Count)
                    {
                        PopUpExit.Play();
                        popup = false;
                        //messages.Clear();
                        PopupPage = 0;
                        UnpauseMovementTS();
                        playPopUpSound = true;
                    }
                    else
                    {
                        PopUp.Play();
                        PopupPage++;
                    }
                }
                #endregion
                GUILayout.BeginArea(new Rect(Screen.width / 2f - 350, Screen.height * .6f, 700, 150));
                GUILayout.Box(messages[PopupPage], EpochSkin.GetStyle("Message"));
                #region Button
                GUILayout.BeginArea(new Rect(600, 100, 300, 100));
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                if (GUILayout.Button(PopupButtonText, EpochSkin.GetStyle("Popup Button")))
                {
                    ClickSound.Play();
                    if (PopupPage + 1 == messages.Count)
                    {
                        PopUpExit.Play();
                        popup = false;
                        PopupPage = 0;
                        UnpauseMovementTS();
                        playPopUpSound = true;
                    }
                    else
                    {
                        PopUp.Play();
                        PopupPage++;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndArea();
                #endregion
                GUILayout.EndArea();
            }
        }
        #endregion

        #region Audio
        public void PlayHover(int i)
        {
            if (MouseOver() && PlayHoverSound[i])
            {
                HoverSound.Play();
                PlayHoverSound[i] = false;
            }
            else if (MouseNotOver() && !PlayHoverSound[i])
            {
                PlayHoverSound[i] = true;
            }
        }

        public bool MouseOver()
        {
            return Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
        }

        public bool MouseNotOver()
        {
            return Event.current.type == EventType.Repaint && !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
        }
        #endregion

        #region Interface Methods
        public void delayFunction(Action method)
        {
            StartCoroutine(_delayFunction(method, DEFAULT_DELAY_TIME));
        }

        public void delayFunction(Action method, float delayTime)
        {
            StartCoroutine(_delayFunction(method, delayTime));
        }
        #endregion

        #region Instance Methods
        private IEnumerator _delayFunction(Action method, float delayTime)
        {
            PauseMovement();

            method();
            yield return new WaitForSeconds(delayTime);

            UnpauseMovement();
        }
        #endregion
    }
}