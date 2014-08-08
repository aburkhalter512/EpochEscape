using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;

public class LevelEditorMenu : MonoBehaviour
{
	public const float ANGLE_OFFSET = 90f;
	public const int MENU_LAYER = 11;
	
	private GameObject m_mainMenu = null;
	private LevelEditorMenuItem m_currentHoverTarget = null;
	private bool m_isMenuShown = false;

	private SpriteRenderer m_tempSpriteRenderer = null;
	private Sprite m_menuItemSprite = null;
	private Sprite m_menuItemHoverSprite = null;

	private GameObject m_menuTextGameObject = null;
	private GUIText m_menuText = null;

	private GameObject m_currentMenu = null;

	public void Start()
	{
		Init();
		ConstructMenu();
	}

	public void Update()
	{
		UpdateInput();
	}

	public void Init()
	{
		m_menuItemSprite = Resources.Load<Sprite>("Textures/LevelEditor/MenuItem");
		m_menuItemHoverSprite = Resources.Load<Sprite>("Textures/LevelEditor/MenuItemHover");

		m_menuTextGameObject = GameObject.Find("MenuText");

		if(m_menuTextGameObject != null)
		{
			m_menuText = m_menuTextGameObject.GetComponent<GUIText>();
			m_menuTextGameObject.SetActive(false);
		}
	}

	private void ConstructMenu()
	{
		if(File.Exists("menu.json"))
		{
			string menuDataJSON = File.ReadAllText("menu.json");

			Dictionary<string, object> menuData = Json.Deserialize(menuDataJSON) as Dictionary<string, object>;

			List<object> mainMenuData = menuData["mainMenu"] as List<object>;

			m_mainMenu = new GameObject();
			m_mainMenu.name = "MainMenu";
			m_mainMenu.layer = MENU_LAYER;

			ConstructMenu(mainMenuData, m_mainMenu);

			m_mainMenu.SetActive(false);
		}
	}

	private void ConstructMenu(List<object> data, GameObject parentMenu)
	{
		if(data != null && data.Count > 0)
		{
			Dictionary<string, object> menuDict = null;
			string title = string.Empty;
			string name = string.Empty;
			bool hasSubMenu = false;
			List<object> subMenuData = null;
			string prefabPath = string.Empty;

			GameObject menuGameObject = null;

			float m_angleStep = 360f / data.Count;
			
			GameObject tempMenuItem = null;
			LevelEditorMenuItem tempMenuItemScript = null;
			float tempItemAngle = ANGLE_OFFSET;
			
			for(int i = 0; i < data.Count; i++, tempItemAngle += m_angleStep)
			{
				tempItemAngle %= 360f;

				menuDict = data[i] as Dictionary<string, object>;
				title = menuDict["title"] as string;
				name = menuDict["name"] as string;
				hasSubMenu = (bool)menuDict["hasSubMenu"];

				tempMenuItem = Resources.Load("Prefabs/LevelEditor/MenuItem") as GameObject;

				if(tempMenuItem != null)
				{
					tempMenuItem = Instantiate(tempMenuItem) as GameObject;

					if(tempMenuItem != null)
					{
						tempMenuItem.name = name;
						tempMenuItem.layer = MENU_LAYER;
						tempMenuItem.transform.position = new Vector3(Mathf.Cos(tempItemAngle * Mathf.Deg2Rad) * 2, Mathf.Sin(tempItemAngle * Mathf.Deg2Rad) * 2, 0f);
						tempMenuItem.transform.parent = parentMenu.transform;

						tempMenuItemScript = tempMenuItem.GetComponent<LevelEditorMenuItem>();

						if(tempMenuItemScript != null)
							tempMenuItemScript.Title = title;
					}
				}

				if(hasSubMenu)
				{
					subMenuData = menuDict["menuItems"] as List<object>;
					menuGameObject = new GameObject();
					menuGameObject.name = name;
					menuGameObject.layer = MENU_LAYER;

					if(tempMenuItemScript != null)
					{
						tempMenuItemScript.HasSubMenu = true;
						tempMenuItemScript.SubMenu = menuGameObject;
					}

					ConstructMenu(subMenuData, menuGameObject);

					menuGameObject.SetActive(false);
				}
				else
				{
					prefabPath = menuDict["prefabPath"] as string;

					if(tempMenuItemScript != null)
						tempMenuItemScript.PrefabPath = prefabPath;
				}
			}
		}
	}

	private void UpdateInput()
	{
		if(Input.GetKeyDown(KeyCode.M))
		{
			if(m_isMenuShown)
			{
				m_isMenuShown = false;

				if(m_currentHoverTarget != null)
					m_currentHoverTarget.ResetScale();

				if(m_currentMenu != null)
					m_currentMenu.SetActive(false);

				if(m_menuText != null)
					m_menuText.text = string.Empty;
				
				if(m_menuTextGameObject != null)
					m_menuTextGameObject.SetActive(false);

				if(m_currentHoverTarget != null)
					SetHoverTarget(null);
			}
			else
			{
				m_isMenuShown = true;
				m_currentMenu = m_mainMenu;

				if(m_mainMenu != null)
					m_mainMenu.SetActive(true);
			}
		}
	}

	public void SetHoverTarget(LevelEditorMenuItem target)
	{
		if(target == null)
		{
			m_tempSpriteRenderer = m_currentHoverTarget.GetComponent<SpriteRenderer>();
			m_currentHoverTarget = null;

			if(m_menuText != null)
				m_menuText.text = string.Empty;

			if(m_menuTextGameObject != null)
				m_menuTextGameObject.SetActive(false);

			if(m_tempSpriteRenderer != null)
				m_tempSpriteRenderer.sprite = m_menuItemSprite;
		}
		else
		{
			m_currentHoverTarget = target;
			m_tempSpriteRenderer = target.GetComponent<SpriteRenderer>();

			if(m_menuText != null)
				m_menuText.text = target.Title;

			if(m_menuTextGameObject != null)
				m_menuTextGameObject.SetActive(true);

			if(m_tempSpriteRenderer != null)
				m_tempSpriteRenderer.sprite = m_menuItemHoverSprite;
		}
	}

	public GameObject MenuText
	{
		get { return m_menuTextGameObject; }
	}

	public bool IsMenuShown
	{
		get { return m_isMenuShown; }
		set { m_isMenuShown = value; }
	}

	public GameObject CurrentMenu
	{
		get { return m_currentMenu; }
		set { m_currentMenu = value; }
	}
}
