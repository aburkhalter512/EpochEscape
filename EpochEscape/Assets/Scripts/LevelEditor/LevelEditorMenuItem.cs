using UnityEngine;
using System.Collections;

public class LevelEditorMenuItem : MonoBehaviour
{
	public const float DEFAULT_SCALE = 1f;
	public const float HOVER_SCALE = 1.25f;
	public const float SCALE_SPEED = 7.5f;

	private LevelEditorMenu m_menu = null;
	private float m_targetScale = DEFAULT_SCALE;
	private bool m_isLerping = false;
	private float m_tempScale = 0f;

	private bool m_hasSubMenu = false;
	private GameObject m_subMenu = null; // If m_hasSubMenu == true.
	private string m_prefabPath = string.Empty; // If m_hasSubMenu == false.

	private string m_title = string.Empty;

	public void Start()
	{
		InitializeMenu();
	}

	private void InitializeMenu()
	{
		GameObject menuCameraGameObject = GameObject.FindWithTag("MenuCamera");
		Camera menuCamera = null;

		if(menuCameraGameObject != null)
		{
			menuCamera = menuCameraGameObject.GetComponent<Camera>();

			if(menuCamera != null)
				m_menu = menuCamera.GetComponent<LevelEditorMenu>();
		}
	}

	public void Update()
	{
		UpdateScale();
	}

	private void UpdateScale()
	{
		if(!m_isLerping)
			return;

		m_tempScale = Mathf.Lerp(transform.localScale.x, m_targetScale, SCALE_SPEED * Time.deltaTime);

		if(Utilities.IsApproximately(m_tempScale, m_targetScale))
		{
			m_isLerping = false;
			m_tempScale = m_targetScale;
		}

		transform.localScale = new Vector3(m_tempScale, m_tempScale, 0f);
	}

	public void OnMouseEnter()
	{
		m_targetScale = HOVER_SCALE;
		m_isLerping = true;

		if(m_menu != null)
			m_menu.SetHoverTarget(this);
	}
	
	public void OnMouseExit()
	{
		m_targetScale = DEFAULT_SCALE;
		m_isLerping = true;

		if(m_menu != null)
			m_menu.SetHoverTarget(null);
	}

	public void OnMouseDown()
	{
		ResetScale();

		gameObject.transform.parent.gameObject.SetActive(false);

		if(m_menu != null)
		{
			m_menu.MenuText.SetActive(false);
			m_menu.SetHoverTarget(null);
			m_menu.CurrentMenu = m_subMenu;
		}

		if(m_subMenu != null)
			m_subMenu.SetActive(true);

		if(!m_hasSubMenu)
		{
			m_menu.IsMenuShown = false;

			ConstructPrefab();
		}
	}

	private void ConstructPrefab()
	{
		GameObject constructedPrefab = Resources.Load(m_prefabPath) as GameObject;

		if(constructedPrefab != null)
		{
			constructedPrefab = Instantiate(constructedPrefab) as GameObject;

			if(constructedPrefab != null)
			{
				constructedPrefab.AddComponent<Placeable>();
				constructedPrefab.AddComponent<BoxCollider2D>();
				constructedPrefab.transform.position = Vector3.zero;
			}
		}
	}

	public void ResetScale()
	{
		m_targetScale = DEFAULT_SCALE;

		transform.localScale = new Vector3(DEFAULT_SCALE, DEFAULT_SCALE, 0f);
	}

	public string Title
	{
		get { return m_title; }
		set { m_title = value; }
	}

	public bool HasSubMenu
	{
		get { return m_hasSubMenu; }
		set { m_hasSubMenu = value; }
	}

	public GameObject SubMenu
	{
		get { return m_subMenu; }
		set { m_subMenu = value; }
	}

	public string PrefabPath
	{
		get { return m_prefabPath; }
		set { m_prefabPath = value; }
	}
}
