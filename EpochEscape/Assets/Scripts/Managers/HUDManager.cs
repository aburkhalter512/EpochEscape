using UnityEngine;
using System.Collections;

public class HUDManager : MonoBehaviour {
	public Player playerManager;
	public GUISkin EpochSkin;
	
	#region hero icon + detection bar
	public Texture2D[] detectionBar;
	public Texture2D currMeter;
	public Texture2D happy;
	public Texture2D worried;
	public Texture2D scared;
	public Texture2D currMood;
	private Vector2 iconPos = new Vector2(0f,0f);
	private Vector2 iconSize = new Vector2 (135f, 135f);
	#endregion
	
	#region inventory
	public Texture2D potion;
	public Texture2D potionSelected;
	public Texture2D currPotion;
	public Texture2D flask;
	public Texture2D flaskSelected;
	public Texture2D currFlask;
	public Texture2D specItem;
	private int m_flaskCount = 0;
	private int m_potionCount = 0;
	private int m_specCount = 0;
	private Vector2 potionPos = new Vector2 (0f,100f);
	private Vector2 potionSize = new Vector2 (65.1f,81.9f);
	private Vector2 flaskPos = new Vector2 (0f,150f);
	private Vector2 flaskSize = new Vector2(65.1f,81.9f);
	#endregion
	
	#region power core display
	public Texture2D frame;
	public Texture2D empty;
	public Texture2D piece1;
	public Texture2D piece2;
	public Texture2D piece3;
	public Texture2D currCore;
	private Vector2 corePos = new Vector2 (Screen.width - 100f, Screen.height - 100f);
	private Vector2 frameSize = new Vector2(100f,100f);
	private Vector2 coreSize = new Vector2(75f,75f);
	#endregion
	
	void Start() {
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		playerManager = player.GetComponent<Player>();
		EpochSkin = Resources.Load ("Prefabs/GUI/EpochStyle",typeof(GUISkin)) as GUISkin;
		detectionBar = new Texture2D[13];

		detectionBar[0] = Resources.Load ("Textures/GUI/HUD/MeterEmpty",typeof(Texture2D)) as Texture2D;
		detectionBar[1] = Resources.Load ("Textures/GUI/HUD/Meter1",typeof(Texture2D)) as Texture2D;
		detectionBar[2] = Resources.Load ("Textures/GUI/HUD/Meter2",typeof(Texture2D)) as Texture2D;
		detectionBar[3] = Resources.Load ("Textures/GUI/HUD/Meter3",typeof(Texture2D)) as Texture2D;
		detectionBar[4] = Resources.Load ("Textures/GUI/HUD/Meter4",typeof(Texture2D)) as Texture2D;
		detectionBar[5] = Resources.Load ("Textures/GUI/HUD/Meter5",typeof(Texture2D)) as Texture2D;
		detectionBar[6] = Resources.Load ("Textures/GUI/HUD/Meter6",typeof(Texture2D)) as Texture2D;
		detectionBar[7] = Resources.Load ("Textures/GUI/HUD/Meter7",typeof(Texture2D)) as Texture2D;
		detectionBar[8] = Resources.Load ("Textures/GUI/HUD/Meter8",typeof(Texture2D)) as Texture2D;
		detectionBar[9] = Resources.Load ("Textures/GUI/HUD/Meter9",typeof(Texture2D)) as Texture2D;
		detectionBar[10] = Resources.Load ("Textures/GUI/HUD/Meter10",typeof(Texture2D)) as Texture2D;
		detectionBar[11] = Resources.Load ("Textures/GUI/HUD/Meter11",typeof(Texture2D)) as Texture2D;
		detectionBar[12] = Resources.Load ("Textures/GUI/HUD/Meter12",typeof(Texture2D)) as Texture2D;
		happy = Resources.Load("Textures/GUI/HUD/CaveGirlHappy",typeof(Texture2D)) as Texture2D;
		worried = Resources.Load("Textures/GUI/HUD/CaveGirlWorried",typeof(Texture2D)) as Texture2D;
		scared = Resources.Load("Textures/GUI/HUD/CaveGirlScared",typeof(Texture2D)) as Texture2D;
		potion = Resources.Load ("Textures/GUI/HUD/PotionNormal", typeof(Texture2D)) as Texture2D;
		potionSelected = Resources.Load ("Textures/GUI/HUD/PotionSelected", typeof(Texture2D)) as Texture2D;
		flask = Resources.Load("Textures/GUI/HUD/FlaskNormal",typeof(Texture2D)) as Texture2D;
		flaskSelected = Resources.Load ("Textures/GUI/HUD/FlaskSelected", typeof(Texture2D)) as Texture2D;
		specItem = Resources.Load("Textures/GUI/HUD/CaveGirlSpec",typeof(Texture2D)) as Texture2D;
		frame = Resources.Load("Textures/GUI/HUD/HUDPlate", typeof(Texture2D)) as Texture2D;
		empty = Resources.Load("Textures/GUI/HUD/EmptyCore", typeof(Texture2D)) as Texture2D;
		piece1 = Resources.Load("Textures/GUI/HUD/PowerCore1", typeof(Texture2D)) as Texture2D;
		piece2 = Resources.Load("Textures/GUI/HUD/PowerCore2",typeof(Texture2D)) as Texture2D;
		piece3 = Resources.Load("Textures/GUI/HUD/PowerCoreFullFrame",typeof(Texture2D)) as Texture2D;

		currMeter = detectionBar[0];
		currMood = happy;
		currPotion = potion;
		currFlask = flask;
	}
	
	void OnGUI() {
		#region hero icon + detectionbar
		GUI.BeginGroup(new Rect(iconPos.x,iconPos.y,iconSize.x,iconSize.y));
		GUI.DrawTexture(new Rect(0f,0f,iconSize.x,iconSize.y),currMeter);
		GUI.DrawTexture(new Rect(22f,22f,75f,75f),currMood);
		GUI.EndGroup();
		#endregion
		
		#region inventory
		GUI.BeginGroup(new Rect(potionPos.x,potionPos.y,potionSize.x,potionSize.y));
		GUI.DrawTexture(new Rect(0f,0f,potionSize.x,potionSize.y),currPotion);
		GUI.Label(new Rect(36f,27f,20f,20f), m_potionCount.ToString(),EpochSkin.GetStyle("HudText"));
		GUI.EndGroup();
		GUI.BeginGroup(new Rect(flaskPos.x,flaskPos.y,flaskSize.x,flaskSize.y));
		GUI.DrawTexture(new Rect(0f,0f,potionSize.x,potionSize.y),currFlask);
		GUI.Label(new Rect(36f,27f,20f,20f), m_flaskCount.ToString(),EpochSkin.GetStyle("HudText"));
		GUI.EndGroup();
		#endregion
		
		#region power cores
		GUI.BeginGroup(new Rect(corePos.x,corePos.y,frameSize.x,frameSize.y));
		GUI.DrawTexture(new Rect(0f, 0f, frameSize.x, frameSize.y), frame);
		GUI.DrawTexture (new Rect(12.5f,12.5f,coreSize.x,coreSize.y),currCore);
		GUI.EndGroup();
		#endregion
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
		if (currDetection < 0.33f)
			currMeter = detectionBar[0];
		else if (currDetection < 8.33f)
			currMeter = detectionBar[1];
		else if (currDetection < 16.66f)
			currMeter = detectionBar[2];
		else if (currDetection < 25f)
			currMeter = detectionBar[3];
		else if (currDetection < 33.33f)
			currMeter = detectionBar[4];
		else if (currDetection < 41.66f)
			currMeter = detectionBar[5];
		else if (currDetection < 50f)
			currMeter = detectionBar[6];
		else if (currDetection < 58.33f)
			currMeter = detectionBar[7];
		else if (currDetection < 66.66f)
			currMeter = detectionBar[8];
		else if (currDetection < 75f)
			currMeter = detectionBar[9];
		else if (currDetection < 83.33f)
			currMeter = detectionBar[10];
		else if (currDetection < 91.66f)
			currMeter = detectionBar[11];
		else
			currMeter = detectionBar[12];
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
			currPotion = potionSelected;
			currFlask = flask;
			break;
		case 1:
			currPotion = potion;
			currFlask = flaskSelected;
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