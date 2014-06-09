using UnityEngine;
using System.Collections;

// THIS HUDMANAGER WILL ONLY WORK FOR 4:3 RESOLUTIONS

public class HUDManager : MonoBehaviour {
	public Player playerManager;
	public GUISkin EpochSkin;
	
	#region scaling
	private const float origWidth = 1024f;
	private const float origHeight = 768f;
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
	private Vector2 iconPos = new Vector2(0f,10f);
	private Vector2 iconSize = new Vector2 (135f, 135f);
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
	private int m_specCount = 0;
	private Vector2 redPos = new Vector2 (0f,683f);
	private Vector2 greenPos = new Vector2 (80f,683f);
	private Vector2 potionSize = new Vector2 (71f,85f);

	#endregion
	
	#region power core display
	public Texture2D coreJar;
	public Texture2D empty;
	public Texture2D piece1;
	public Texture2D piece2;
	public Texture2D piece3;
	public Texture2D currCore;
	private Vector2 corePos = new Vector2 (921f, 616f);
	private Vector2 jarSize = new Vector2(103f,152f);
	private Vector2 coreSize = new Vector2(67.7f,134f);
	#endregion
	
	void Start() {
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		playerManager = player.GetComponent<Player>();
		EpochSkin = Resources.Load ("Prefabs/GUI/EpochStyle",typeof(GUISkin)) as GUISkin;
		iconFrame = Resources.Load ("Textures/GUI/HUD/IconFrame", typeof(Texture2D)) as Texture2D;
		detectionBar = Resources.Load ("Textures/GUI/HUD/DetectionBar", typeof(Texture2D)) as Texture2D;
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
		GUI.BeginGroup(new Rect(iconPos.x,iconPos.y,iconSize.x,iconSize.y));
		GUI.DrawTexture(new Rect(0f,89f,iconSize.x,-79f * barFill),detectionBar);
		GUI.DrawTexture(new Rect(0f,0f,iconSize.x,iconSize.y),iconFrame);
		GUI.DrawTexture(new Rect(22.8f,14f,76f,77f),currMood);
		if (playerManager.m_hasSpecialItem)
			GUI.DrawTexture(new Rect(20f,92f,30f,30f),specItem);
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
		for (int i = 0; i < Inventory.UNIQUE_ITEMS; i++) {
			Item currItem = playerManager.inventory.inventory[i];
			if (currItem != null) {
				if (currItem.gameObject.tag == "EmptyFlask") {
					m_flaskCount = playerManager.inventory.inventoryCount[i];
					flaskFound = true;
				}
				if (currItem.gameObject.tag == "Red Potion") {
					m_potionCount = playerManager.inventory.inventoryCount[i];
					potionFound = true;
				}
			}
		}
		if (!flaskFound)
			m_flaskCount = 0;
		if (!potionFound)
			m_potionCount = 0;
		
		m_specCount = playerManager.m_specItems;
		
		switch (playerManager.m_selectedSlot) {
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
		switch(playerManager.currentCores) {
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
}