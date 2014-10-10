using UnityEngine;
using System.Collections;

public class HUDManager : Manager<HUDManager> {
    public Player playerManager;
    public GUISkin EpochSkin;

    protected override void Initialize()
    {
        // First time initialization.
    }
    
    #region scaling
    private const float origWidth = 1600f;
    private const float origHeight = 900f;
    private Vector3 scale;
    #endregion
    
    #region hero icon + detection bar
    public Texture2D iconFrame;
    public Texture2D detectionBar;
    public Texture2D happy;
    public Texture2D worried;
    public Texture2D scared;
    public Texture2D currMood;
    public Texture2D specItem;
    public Texture2D cooldownBar;
    private Vector2 iconPos = new Vector2(0f,10f);
    private Vector2 iconSize = new Vector2 (175f, 175f);
    private float barFill;
    #endregion
    
    #region inventory
    public Texture2D redPot;
    public Texture2D redSelected;
    public Texture2D currRed;
    public Texture2D greenPot;
    public Texture2D greenSelected;
    public Texture2D currGreen;
    private int m_flaskCount = 0;
    private int m_potionCount = 0;
    private Vector2 redPos = new Vector2 (0f,800f);
    private Vector2 greenPos = new Vector2 (93f,800f);
    private Vector2 potionSize = new Vector2 (85.2f,100f);
    
    #endregion
    
    #region power core display
    public Texture2D coreJar;
    public Texture2D empty;
    public Texture2D piece1;
    public Texture2D piece2;
    public Texture2D piece3;
    public Texture2D currCore;
    private Vector2 corePos = new Vector2 (1479f, 748f);
    private Vector2 jarSize = new Vector2(103f,152f);
    private Vector2 coreSize = new Vector2(67.7f,134f);
    #endregion
    
    void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerManager = player.GetComponent<Player>();
        EpochSkin = Resources.Load ("Prefabs/GUI/EpochStyle",typeof(GUISkin)) as GUISkin;
        iconFrame = Resources.Load ("Textures/GUI/HUD/IconFrame", typeof(Texture2D)) as Texture2D;
        detectionBar = Resources.Load ("Textures/GUI/HUD/DetectionBar", typeof(Texture2D)) as Texture2D;
        cooldownBar = Resources.Load<Texture2D>("Textures/GUI/HUD/cooldown");
        redPot = Resources.Load ("Textures/GUI/HUD/RedPot", typeof(Texture2D)) as Texture2D;
        redSelected = Resources.Load("Textures/GUI/HUD/RedSelected",typeof(Texture2D)) as Texture2D;
        greenPot = Resources.Load ("Textures/GUI/HUD/GreenPot", typeof(Texture2D)) as Texture2D;
        greenSelected = Resources.Load ("Textures/GUI/HUD/GreenSelected",typeof(Texture2D)) as Texture2D;
        coreJar = Resources.Load("Textures/GUI/HUD/PowerCoreJar", typeof(Texture2D)) as Texture2D;
        empty = Resources.Load("Textures/GUI/HUD/PowerCoreEmpty", typeof(Texture2D)) as Texture2D;
        piece1 = Resources.Load("Textures/GUI/HUD/PowerCore1", typeof(Texture2D)) as Texture2D;
        piece2 = Resources.Load("Textures/GUI/HUD/PowerCore2",typeof(Texture2D)) as Texture2D;
        piece3 = Resources.Load("Textures/GUI/HUD/PowerCore3",typeof(Texture2D)) as Texture2D;
        
        switch (GameManager.getInstance ().m_currentCharacter) {
        case 0:
            happy = Resources.Load("Textures/GUI/HUD/CaveGirlHappy",typeof(Texture2D)) as Texture2D;
            worried = Resources.Load("Textures/GUI/HUD/CaveGirlWorried",typeof(Texture2D)) as Texture2D;
            scared = Resources.Load("Textures/GUI/HUD/CaveGirlScared",typeof(Texture2D)) as Texture2D;
            specItem = Resources.Load("Textures/GUI/HUD/CaveGirlSpec",typeof(Texture2D)) as Texture2D;
            break;
            
        case 1:
            happy = Resources.Load ("Textures/GUI/HUD/KnightHappy",typeof(Texture2D)) as Texture2D;
            worried = Resources.Load ("Textures/GUI/HUD/KnightWorried",typeof(Texture2D)) as Texture2D;
            scared = Resources.Load ("Textures/GUI/HUD/KnightScared",typeof(Texture2D)) as Texture2D;
            specItem = Resources.Load ("Textures/GUI/HUD/KnightSpec",typeof(Texture2D)) as Texture2D;
            break;
        }
        
        currMood = happy;
        currRed = redPot;
        currGreen = greenPot;
        currCore = empty;
        
        scale.z = 1f;
        barFill = 0f;
    }
    
    void OnGUI() {
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
		
		GUI.DrawTexture(new Rect(175f,115f,40f,-100f * playerManager.inventory.getPercentSpecialStamina()), cooldownBar);
        GUI.EndGroup();
        #endregion
        
        #region inventory
        GUI.BeginGroup(new Rect(redPos.x,redPos.y,potionSize.x,potionSize.y));
        GUI.DrawTexture(new Rect(0f,0f,potionSize.x,potionSize.y),currRed);
        GUI.Label(new Rect(10f,30f,40f,40f), m_potionCount.ToString(),EpochSkin.GetStyle("HudText"));
        GUI.EndGroup();
        
        GUI.BeginGroup(new Rect(greenPos.x,greenPos.y,potionSize.x,potionSize.y));
        GUI.DrawTexture(new Rect(0f,0f,potionSize.x,potionSize.y),currGreen);
        GUI.Label(new Rect(10f,30f,40f,40f), m_flaskCount.ToString(),EpochSkin.GetStyle("HudText"));
        GUI.EndGroup();
        #endregion
        
        #region power cores
        GUI.BeginGroup(new Rect(corePos.x,corePos.y,jarSize.x,jarSize.y));
        GUI.DrawTexture (new Rect(18f, 4f, coreSize.x, coreSize.y),currCore);
        GUI.DrawTexture(new Rect(0f, 0f, jarSize.x, jarSize.y), coreJar);
        GUI.EndGroup();
        #endregion
        
        GUI.matrix = svMat;
    }
    
    void Update() {
        if(playerManager == null)
            return;
        
        float currDetection = playerManager.m_detectionLevel;
        
        #region hero icon
        if (currDetection < Player.MAX_DETECTION_LEVEL * .33f) {
            currMood = happy;
        } else if (currDetection < Player.MAX_DETECTION_LEVEL * .66f) {
            currMood = worried;
        } else {
            currMood = scared;
        }
        #endregion
        
        #region detection bar
        barFill = currDetection / Player.MAX_DETECTION_LEVEL;
        #endregion
        
        #region inventory
        bool flaskFound = false;
        bool potionFound = false;
        for (int i = 0; i < Inventory.ACTIVE_ITEM_COUNT; i++)
        {
            Inventory.ActiveItemNode currItem = playerManager.inventory.activeItems[i];
            if (currItem != null)
            {
                if (currItem.data as RedPotion)
                {
                    m_potionCount = playerManager.inventory.activeItems[i].nodesAttached + 1;
                    potionFound = true;
                }
                else if (currItem.data as GreenPotion)
                {
                    m_flaskCount = playerManager.inventory.activeItems[i].nodesAttached + 1;
                    flaskFound = true;
                }
            }
        }

        if (!flaskFound)
            m_flaskCount = 0;
        if (!potionFound)
            m_potionCount = 0;
        
        switch (playerManager.getSelectedSlot()) {
        case 0:
            currRed = redSelected;
            currGreen = greenPot;
            break;
        case 1:
            currRed = redPot;
            currGreen = greenSelected;
            break;
        }
        #endregion
        
        #region powercores
        switch(playerManager.getCurrentCores()) {
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
        #endregion
    }

    #region Private Interfaces
    private void _Hide()
    {
        gameObject.SetActive(false);
    }

    private void _Show()
    {
        gameObject.SetActive(true);
    }
    #endregion

    #region Public Interfaces
    public static void Hide()
    {
        HUDManager.GetInstance()._Hide();
    }

    public static void Show()
    {
        HUDManager.GetInstance()._Show();
    }
    #endregion
}