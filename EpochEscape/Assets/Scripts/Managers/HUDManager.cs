using UnityEngine;
using System.Collections;

public class HUDManager : Manager<HUDManager>
{
    #region Instance Variables
    private GUISkin EpochSkin;

    private Texture2D coreJar;
    private Texture2D empty;
    private Texture2D piece1;
    private Texture2D piece2;
    private Texture2D piece3;
    private Texture2D currCore;

    private Texture2D iconFrame;
    private Texture2D detectionBar;
    private Texture2D happy;
    private Texture2D worried;
    private Texture2D scared;
    private Texture2D currMood;
    private Texture2D specItem;
    private Texture2D cooldownBar;

    private Player mPlayer;
    private Vector2 corePos = new Vector2(1479f, 748f);
    private Vector2 jarSize = new Vector2(103f, 152f);
    private Vector2 coreSize = new Vector2(67.7f, 134f);

    private const float origWidth = 1600f;
    private const float origHeight = 900f;
    private Vector3 scale;
    private Vector2 iconPos = new Vector2(0f, 10f);
    private Vector2 iconSize = new Vector2(175f, 175f);
    private float barFill;

    private TimerDoorFrame mTimerDoor = null;
    #endregion

    protected override void Initialize()
    {
        mPlayer = Player.Get();

        EpochSkin = Resources.Load("Prefabs/GUI/EpochStyle", typeof(GUISkin)) as GUISkin;
        iconFrame = Resources.Load("Textures/GUI/HUD/IconFrame", typeof(Texture2D)) as Texture2D;
        detectionBar = Resources.Load("Textures/GUI/HUD/DetectionBar", typeof(Texture2D)) as Texture2D;
        coreJar = Resources.Load("Textures/GUI/HUD/PowerCoreJar", typeof(Texture2D)) as Texture2D;
        empty = Resources.Load("Textures/GUI/HUD/PowerCoreEmpty", typeof(Texture2D)) as Texture2D;
        piece1 = Resources.Load("Textures/GUI/HUD/PowerCore1", typeof(Texture2D)) as Texture2D;
        piece2 = Resources.Load("Textures/GUI/HUD/PowerCore2", typeof(Texture2D)) as Texture2D;
        piece3 = Resources.Load("Textures/GUI/HUD/PowerCore3", typeof(Texture2D)) as Texture2D;

        switch (GameManager.Get().m_currentCharacter)
        {
            case 0:
                happy = Resources.Load("Textures/GUI/HUD/CaveGirlHappy", typeof(Texture2D)) as Texture2D;
                worried = Resources.Load("Textures/GUI/HUD/CaveGirlWorried", typeof(Texture2D)) as Texture2D;
                scared = Resources.Load("Textures/GUI/HUD/CaveGirlScared", typeof(Texture2D)) as Texture2D;
                specItem = Resources.Load("Textures/GUI/HUD/CaveGirlSpec", typeof(Texture2D)) as Texture2D;
                break;

            case 1:
                happy = Resources.Load("Textures/GUI/HUD/KnightHappy", typeof(Texture2D)) as Texture2D;
                worried = Resources.Load("Textures/GUI/HUD/KnightWorried", typeof(Texture2D)) as Texture2D;
                scared = Resources.Load("Textures/GUI/HUD/KnightScared", typeof(Texture2D)) as Texture2D;
                specItem = Resources.Load("Textures/GUI/HUD/KnightSpec", typeof(Texture2D)) as Texture2D;
                break;
        }

        currMood = happy;
        currCore = empty;

        scale.z = 1f;
        barFill = 0f;
    }
    
    void OnGUI()
    {
        scale.x = Screen.width / HUDManager.origWidth;
        scale.y = Screen.height / HUDManager.origHeight;
        Matrix4x4 svMat = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, scale);
        
        #region hero icon + detectionbar
        GUI.BeginGroup(new Rect(iconPos.x,iconPos.y,iconSize.x + 200.0f,iconSize.y));
        GUI.DrawTexture(new Rect(0f,115f,iconSize.x,-95f * barFill),detectionBar);
        GUI.DrawTexture(new Rect(0f,0f,iconSize.x,iconSize.y),iconFrame);
        GUI.DrawTexture(new Rect(30f,21f,100f,100f),currMood);
		GUI.DrawTexture(new Rect(20f,120f,30f,30f),specItem);
		
        GUI.EndGroup();
        #endregion
        
        #region power cores
        GUI.BeginGroup(new Rect(corePos.x,corePos.y,jarSize.x,jarSize.y));
        GUI.DrawTexture (new Rect(18f, 4f, coreSize.x, coreSize.y),currCore);
        GUI.DrawTexture(new Rect(0f, 0f, jarSize.x, jarSize.y), coreJar);
        GUI.EndGroup();
        #endregion

        updateTimer();
        
        GUI.matrix = svMat;
    }
    
    void Update()
    {
        if(mPlayer == null)
            return;
        
        #region hero icon
        if (mPlayer.getCurrentDetection() < .33f)
            currMood = happy;
        else if (mPlayer.getCurrentDetection() < .66f)
            currMood = worried;
        else
            currMood = scared;
        #endregion
        
        barFill = mPlayer.getCurrentDetection();
        
        switch(mPlayer.getCurrentCores()) {
        case 0:
            currCore = empty;
            break;
        case 1:
            currCore = piece1;
            break;
        case 2:
            currCore = piece2;
            break;
        case 3:
            currCore = piece3;
            break;
        }
    }

    #region Interface Methods
    public static void Hide()
    {
        HUDManager.Get()._Hide();
    }

    public static void Show()
    {
        HUDManager.Get()._Show();
    }

    public static void SetTimer(TimerDoorFrame timerDoor)
    {
        HUDManager.Get()._SetTimer(timerDoor);
    }
    #endregion

    #region Instance Methods
    private void _Hide()
    {
        gameObject.SetActive(false);
    }

    private void _Show()
    {
        gameObject.SetActive(true);
    }

    private void _SetTimer(TimerDoorFrame timerDoor)
    {
        mTimerDoor = timerDoor;
    }

    private void updateTimer()
    {
        if (mTimerDoor == null)
            return;

        if (!mTimerDoor.isTiming())
        {
            mTimerDoor = null;
            return;
        }

        GUI.Label(
            new Rect(Screen.width / 2 - 50f, 30f, 100f, 100f), 
            mTimerDoor.getTimeRemaining().ToString(), 
            EpochSkin.GetStyle("Timer"));
    }
    #endregion
}