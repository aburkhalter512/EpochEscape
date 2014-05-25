using UnityEngine;
using System.Collections;

public class MapBehavior : MonoBehaviour
{	
	public bool m_isMapDisplayed = false;

	private float m_initialMapYPosition;
	private float m_initialCameraSize;

	private GameObject m_player;
	private float m_mapHeight;
	private GameObject m_star;

	public void Start()
	{
		m_initialMapYPosition = transform.position.y;
		m_initialCameraSize = Camera.main.orthographicSize;
		m_player = null;

		CalculateMapHeight();
		CreateStar();
	}
	
	public void Update()
	{
		CheckForPlayer();

		if(m_player == null) return;

		UpdateInput();
		UpdateCameraPosition();
	}

	private void CalculateMapHeight()
	{
		SpriteRenderer mapRenderer = GetComponent<SpriteRenderer>();

		m_mapHeight = 0f;

		if(mapRenderer != null)
			m_mapHeight = mapRenderer.bounds.size.y;
	}

	private void CreateStar()
	{
		m_star = Resources.Load("Prefabs/Star") as GameObject;
		m_star = Instantiate(m_star) as GameObject;
		m_star.SetActive(false);
	}

	private void CheckForPlayer()
	{
		if(m_player == null)
			m_player = GameObject.FindWithTag("Player");
	}

	private void UpdateInput()
	{
		if(Input.GetKeyUp(KeyCode.M))
		{
			m_isMapDisplayed = !m_isMapDisplayed;

			Camera.main.orthographicSize = m_isMapDisplayed ? m_mapHeight / 2 : m_initialCameraSize;
		}
	}

	private void UpdateCameraPosition()
	{
		float newCameraXPosition = 0f;
		float newCameraYPosition = 0f;

		if(m_isMapDisplayed)
		{
			newCameraXPosition = transform.position.x;
			newCameraYPosition = m_initialMapYPosition - m_mapHeight;

			transform.position = new Vector3(newCameraXPosition, newCameraYPosition, transform.position.z);

			m_star.transform.position = new Vector3(m_player.transform.position.x, m_player.transform.position.y - m_mapHeight, m_star.transform.position.z);
			m_star.SetActive(true);
		}
		else if(m_player != null)
		{
			newCameraXPosition = m_player.transform.position.x;
			newCameraYPosition = m_player.transform.position.y;

			transform.position = new Vector3(transform.position.x, m_initialMapYPosition, transform.position.z);

			m_star.SetActive(false);
		}

		Camera.main.transform.position = new Vector3(newCameraXPosition, newCameraYPosition, Camera.main.transform.position.z);
	}
}
