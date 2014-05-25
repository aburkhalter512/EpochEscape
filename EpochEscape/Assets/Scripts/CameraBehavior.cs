using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour
{
	private bool m_isMapDisplayed;
	private GameObject m_player;
	private GameObject m_map;

	public void Start()
	{
		m_isMapDisplayed = false;
		m_player = null;
		m_map = GameObject.FindWithTag("Map");
	}
	
	public void Update()
	{
		CheckForPlayer();

		if(m_player == null) return;

		UpdateInput();
		UpdateCameraPosition();
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
			float newCameraSize = 0f;

			if(m_isMapDisplayed)
				newCameraSize = 1.375f;
			else
				newCameraSize = 2.6f;

			m_isMapDisplayed = !m_isMapDisplayed;

			Camera.main.orthographicSize = newCameraSize;
		}
	}

	private void UpdateCameraPosition()
	{
		float newXPosition = 0f;
		float newYPosition = 0f;

		if(m_isMapDisplayed && m_map != null)
		{
			newXPosition = m_map.transform.position.x;
			newYPosition = m_map.transform.position.y;
		}
		else if(m_player != null)
		{
			newXPosition = m_player.transform.position.x;
			newYPosition = m_player.transform.position.y;
		}

		Camera.main.transform.position = new Vector3(newXPosition, newYPosition, Camera.main.transform.position.z);
	}
}
