using UnityEngine;
using System.Collections;

public class HUDManager : MonoBehaviour {
	public Player playerManager;
	
	#region hero icon
	public Texture2D happy;
	public Texture2D worried;
	public Texture2D scared;
	public Texture2D currMood;
	private Vector2 iconPos = new Vector2 (0f,0f);
	private Vector2 iconSize = new Vector2 (100f, 100f);
	#endregion
	
	#region detection bar
	public Texture2D emptyBar;
	public Texture2D fullBar;
	private Vector2 barPos = new Vector2 (105f,5f);
	private Vector2 barSize = new Vector2 (80f, 20f);
	private float m_barProgress = 0f;
	#endregion
	
	#region inventory
	public Texture2D flask;
	public Texture2D specItem;
	public Texture2D potion;
	private int m_flaskCount = 0;
	private int m_potionCount = 0;
	private int m_specCount = 0;
	private Vector2 invPos = new Vector2 (105f, 25f);
	private Vector2 invSize = new Vector2 (80f, 75f);
	#endregion
	
	#region power core display
	public Texture2D empty;
	public Texture2D piece1;
	public Texture2D piece2;
	public Texture2D piece3;
	private Vector2 corePos = new Vector2 (Screen.width - 75f, 0f);
	private Vector2 coreSize = new Vector2(75f,75f);
	
	//private bool playAnimation = false;
	#endregion
	
	void Start() {
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		playerManager = player.GetComponent<Player>();
		
		happy = Resources.Load("Textures/GUI/HUD/CaveGirlHappy",typeof(Texture2D)) as Texture2D;
		worried = Resources.Load("Textures/GUI/HUD/CaveGirlWorried",typeof(Texture2D)) as Texture2D;
		scared = Resources.Load("Textures/GUI/HUD/CaveGirlScared",typeof(Texture2D)) as Texture2D;
		emptyBar = Resources.Load("Textures/GUI/HUD/HealthEmpty",typeof(Texture2D)) as Texture2D;
		fullBar = Resources.Load("Textures/GUI/HUD/HealthFull",typeof(Texture2D)) as Texture2D;
		flask = Resources.Load("Textures/Items/EmptyPotion",typeof(Texture2D)) as Texture2D;
		potion = Resources.Load ("Textures/Items/Potion", typeof(Texture2D)) as Texture2D;
		specItem = Resources.Load("Textures/GUI/HUD/CaveGirlSpec",typeof(Texture2D)) as Texture2D;
		empty = Resources.Load("Textures/GUI/HUD/EmptyCore", typeof(Texture2D)) as Texture2D;
		piece1 = Resources.Load("Textures/GUI/HUD/PowerCore1", typeof(Texture2D)) as Texture2D;
		piece2 = Resources.Load("Textures/GUI/HUD/PowerCore2",typeof(Texture2D)) as Texture2D;
		piece3 = Resources.Load("Textures/GUI/HUD/PowerCoreFullFrame",typeof(Texture2D)) as Texture2D;
		
		currMood = happy;
	}
	
	void OnGUI() {
		#region hero icon
		GUI.DrawTexture (new Rect (iconPos.x, iconPos.y, iconSize.x, iconSize.y), currMood);
		#endregion
		
		#region detection bar
		GUI.BeginGroup(new Rect(barPos.x, barPos.y, barSize.x, barSize.y));
		GUI.DrawTexture(new Rect(0f,0f, barSize.x, barSize.y), emptyBar);
		
		GUI.BeginGroup(new Rect(0f,0f, barSize.x * m_barProgress, barSize.y));
		GUI.DrawTexture(new Rect(0f,0f, barSize.x, barSize.y), fullBar);
		GUI.EndGroup();	
		
		GUI.EndGroup();
		#endregion
		
		#region inventory
		GUI.BeginGroup (new Rect(invPos.x,invPos.y,invSize.x,invSize.y));
		GUI.DrawTexture (new Rect(0f,0f,25f,25f),flask);
		GUI.Label(new Rect(28f, 12.5f, 20f, 20f), m_flaskCount.ToString());
		GUI.DrawTexture(new Rect(38f,0f,25f,25f), potion);
		GUI.Label (new Rect(66f,12.5f,20f,20f),m_potionCount.ToString());
		GUI.DrawTexture (new Rect(0f,35f,25f,25f),specItem);
		GUI.Label(new Rect(28f, 47.5f, 20f, 20f), m_specCount.ToString ());
		GUI.EndGroup();
		#endregion
		
		#region power cores
		GUI.BeginGroup(new Rect(corePos.x,corePos.y,coreSize.x,coreSize.y));
		
		if(playerManager != null)
		{
			if (playerManager.currentCores == 0)
			{
				GUI.DrawTexture(new Rect(0f, 0f, coreSize.x, coreSize.y), empty);
			}
			else if (playerManager.currentCores == 1)
			{
				GUI.DrawTexture(new Rect(0f, 0f, coreSize.x, coreSize.y), piece1);
			}
			else if (playerManager.currentCores == 2)
			{
				GUI.DrawTexture(new Rect(0f, 0f, coreSize.x, coreSize.y), piece2);
			}
			else
			{
				GUI.DrawTexture(new Rect(0f, 0f, coreSize.x, coreSize.y), piece3);
			}
		}
		
		GUI.EndGroup();
		#endregion
	}
	
	void Update() {
		if(playerManager == null)
			return;
		
		#region hero icon
		if (playerManager.m_detectionLevel < Player.MAX_DETECTION_LEVEL * .33f) {
			currMood = happy;
		} else if (playerManager.m_detectionLevel < Player.MAX_DETECTION_LEVEL * .66f) {
			currMood = worried;
		} else {
			currMood = scared;
		}
		#endregion
		
		#region hero icon
		m_barProgress = playerManager.m_detectionLevel / Player.MAX_DETECTION_LEVEL;
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
		#endregion
	}
}