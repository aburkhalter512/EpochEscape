using UnityEngine;
using System.Collections.Generic;
using G = GameManager;

public class CameraBehavior : MonoBehaviour
{
	public const float LERP_SPEED = 5f;

	public Vector3 m_currentTarget;

	private Vector3 m_initialMapPosition;
	private float m_initialCameraSize;

	private GameObject m_player;
	private float m_mapWidth;
	private float m_mapHeight;
	private GameObject m_star;

	public Stack<GameObject> m_lerpTargets = null;

	public GameObject m_floor = null;

	public bool m_displayCowbell = false;
	private GameObject m_cowbell = null;

	private Dimension m_displayDimension;
	private float m_aspectRatio;

	public enum State
	{
		DISPLAY_MAP,
		FOLLOW_PLAYER,
		LERP_TO_PLAYER,
		LERP_TO_TARGET,
		LERP_REST,
		PLAYER_CAUGHT
	};

	public enum Dimension
	{
		WIDTH,
		HEIGHT
	};
	
	public State m_currentState = State.FOLLOW_PLAYER;

	public void Start()
	{
		if (m_floor != null)
			m_initialMapPosition = m_floor.transform.position;

		m_initialCameraSize = camera.orthographicSize;
		m_player = null;
		m_displayDimension = Dimension.HEIGHT;
		m_aspectRatio = (float)Screen.width / Screen.height;

		m_lerpTargets = new Stack<GameObject>();

		CalculateFloorDimensions();
		CreateStar();
		CreateCowbell();
	}
	
	public void Update()
	{
		CheckForPlayer();

		if(m_player == null) return;
		
		if(m_lerpTargets != null && m_lerpTargets.Count != 0 && m_currentState != State.LERP_REST)
			m_currentState = State.LERP_TO_TARGET;

		UpdateInput();
		UpdateCurrentState();
		LerpCameraSize();
	}

	private void UpdateCurrentState()
	{
		switch(m_currentState)
		{
		case State.DISPLAY_MAP:
			DisplayMap();
			break;

		case State.FOLLOW_PLAYER:
			FollowPlayer();
			break;

		case State.LERP_TO_PLAYER:
			LerpToPlayer();
			break;

		case State.LERP_TO_TARGET:
			LerpToTarget();
			break;
		}
	}

	private void DisplayMap()
	{
		if(m_floor == null) return;

		if(!G.getInstance().paused)
			G.getInstance().PauseMovement();

		if(m_displayDimension == Dimension.WIDTH)
		{
			m_currentTarget.x = m_initialMapPosition.x - m_mapWidth;
			m_currentTarget.y = m_floor.transform.position.y;

			m_star.transform.position = new Vector3(m_player.transform.position.x - m_mapWidth, m_player.transform.position.y, m_star.transform.position.z);
		}
		else
		{
			m_currentTarget.x = m_floor.transform.position.x;
			m_currentTarget.y = m_initialMapPosition.y - m_mapHeight;

			m_star.transform.position = new Vector3(m_player.transform.position.x, m_player.transform.position.y - m_mapHeight, m_star.transform.position.z);
		}

		m_floor.transform.position = new Vector3(m_currentTarget.x, m_currentTarget.y, m_floor.transform.position.z);
		m_star.SetActive(true);

		transform.position = new Vector3(m_currentTarget.x, m_currentTarget.y, transform.position.z);
	}

	private void FollowPlayer()
	{
        if (m_player != null)
        {
            m_currentTarget.x = m_player.transform.position.x;
            m_currentTarget.y = m_player.transform.position.y;

            transform.position = new Vector3(m_currentTarget.x, m_currentTarget.y, transform.position.z);
        }
	}

	private void LerpToPlayer()
	{
		Vector3 targetCameraPosition = new Vector3(m_currentTarget.x, m_currentTarget.y, transform.position.z);
		
		transform.position = Vector3.Lerp(transform.position, targetCameraPosition, LERP_SPEED * Time.deltaTime);
		
		if(Utilities.IsApproximately(transform.position.x, targetCameraPosition.x) && Utilities.IsApproximately(transform.position.y, targetCameraPosition.y))
		{
			transform.position = targetCameraPosition;

			m_currentState = State.FOLLOW_PLAYER;

			if(G.getInstance().paused)
				G.getInstance().UnpauseMovement();
		}
	}

	private void LerpToTarget()
	{
		if(m_lerpTargets.Count == 0)
		{
			m_currentTarget = m_player.transform.position;
			m_currentState = State.LERP_TO_PLAYER;

			return;
		}

		if(!G.getInstance().paused)
			G.getInstance().PauseMovement();

		m_currentTarget = m_lerpTargets.Peek().transform.position;

		Vector3 targetCameraPosition = new Vector3(m_currentTarget.x, m_currentTarget.y, transform.position.z);
		
		transform.position = Vector3.Lerp(transform.position, targetCameraPosition, LERP_SPEED * Time.deltaTime);

		if(Utilities.IsApproximately(transform.position.x, targetCameraPosition.x) && Utilities.IsApproximately(transform.position.y, targetCameraPosition.y))
		{
			if(m_lerpTargets.Peek().tag == "WallPivot")
			{
				int numChildren = m_lerpTargets.Peek().transform.childCount;

				for(int i = 0; i < numChildren; i++)
					m_lerpTargets.Peek().transform.GetChild(i).GetComponent<DynamicWall>().currentState = DynamicWall.STATES.TO_CHANGE;

				m_lerpTargets.Pop();
			}
			else
				m_lerpTargets.Pop().GetComponent<DynamicWall>().currentState = DynamicWall.STATES.TO_CHANGE;

			transform.position = targetCameraPosition;

			m_currentState = State.LERP_REST;
		}
	}

	private void CalculateFloorDimensions()
	{
		m_mapWidth = 0f;
		m_mapHeight = 0f;

		if(m_floor == null) return;

		SpriteRenderer mapRenderer = m_floor.GetComponent<SpriteRenderer>();

		if(mapRenderer != null)
		{
			m_mapWidth = mapRenderer.bounds.size.x;
			m_mapHeight = mapRenderer.bounds.size.y;

			if(m_mapWidth / m_mapHeight > m_aspectRatio)
				m_displayDimension = Dimension.WIDTH;
			else
				m_displayDimension = Dimension.HEIGHT;
		}
	}

	private void CreateStar()
	{
		m_star = Resources.Load("Prefabs/Star") as GameObject;
		m_star = Instantiate(m_star) as GameObject;
		m_star.SetActive(false);
	}

	private void CreateCowbell()
	{
		m_cowbell = Resources.Load("Prefabs/Cowbell/MoreCowbell") as GameObject;
		m_cowbell = Instantiate(m_cowbell) as GameObject;

		if(m_cowbell != null)
		{
			m_cowbell.transform.position = new Vector3(m_cowbell.transform.position.x, m_cowbell.transform.position.y - m_mapHeight, m_cowbell.transform.position.z);
			m_cowbell.SetActive(false);
		}
	}

	private void CheckForPlayer()
	{
		if(m_player == null)
		{
			m_player = GameObject.FindWithTag("Player");

			if(m_player != null)
				m_currentTarget = m_player.transform.position;
		}
	}

	private void UpdateInput()
	{
		if(m_floor == null || m_currentState == State.PLAYER_CAUGHT) return;

		if(Input.GetKeyUp(KeyCode.M))
		{
			GameObject walls = GameObject.Find("DynamicWalls");
			GameObject doors = GameObject.Find("Doors");
			GameObject obstacles = GameObject.Find("Obstacles");

			if(m_currentState == State.FOLLOW_PLAYER)
			{
				m_currentState = State.DISPLAY_MAP;
				
				if(walls != null) ShiftObjects(walls);
				if(doors != null) ShiftObjects(doors);
				if(obstacles != null) ShiftObjects(obstacles);

				for(int i = 0; i < walls.transform.childCount; i++)
				{
					for(int j = 0; j < walls.transform.GetChild(i).transform.childCount; j++)
						ShiftObjects(walls.transform.GetChild(i).transform.GetChild(j).transform.gameObject, true);
				}

				if(m_cowbell != null && m_displayCowbell)
					m_cowbell.SetActive(true);

				if(!G.getInstance().paused)
					G.getInstance().PauseMovement();
			}
			else if(m_currentState == State.DISPLAY_MAP)
			{
				m_floor.transform.position = m_initialMapPosition;
				m_currentState = State.FOLLOW_PLAYER;
				m_star.SetActive(false);

				if(walls != null) ShiftObjects(walls, true);
				if(doors != null) ShiftObjects(doors, true);
				if(obstacles != null) ShiftObjects(obstacles, true);

				for(int i = 0; i < walls.transform.childCount; i++)
				{
					for(int j = 0; j < walls.transform.GetChild(i).transform.childCount; j++)
						ShiftObjects(walls.transform.GetChild(i).transform.GetChild(j).transform.gameObject);
				}

				if(m_cowbell != null)
					m_cowbell.SetActive(false);

				if(G.getInstance().paused)
					G.getInstance().UnpauseMovement();
			}
		}
	}

	private void ShiftObjects(GameObject objects, bool restore = false)
	{
		if(objects == null) return;

		Vector3 offset = Vector3.zero;

		if(m_displayDimension == Dimension.WIDTH)
			offset = new Vector3(m_mapWidth, 0f, 0f);
		else
			offset = new Vector3(0f, m_mapHeight, 0f);

		objects.transform.position -= restore ? -offset : offset;
	}

	private void LerpCameraSize()
	{
		if(m_currentState == State.PLAYER_CAUGHT)
		{
			camera.orthographicSize = m_initialCameraSize;

			return;
		}
		
		float targetCameraSize = 0f;

		if(m_currentState == State.DISPLAY_MAP)
		{
			if(m_displayDimension == Dimension.WIDTH)
				targetCameraSize = (m_mapWidth / m_aspectRatio) / 2f;
			else
				targetCameraSize = m_mapHeight / 2f;
		}
		else
			targetCameraSize = m_initialCameraSize;

		//Debug.Log (targetCameraSize);

		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetCameraSize, 2 * LERP_SPEED * Time.deltaTime);
	}
}
