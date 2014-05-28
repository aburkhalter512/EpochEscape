using UnityEngine;
using System.Collections.Generic;
using G = GameManager;

public class CameraBehavior : MonoBehaviour
{
	public const float LERP_SPEED = 5f;

	public Vector3 m_currentTarget;

	private float m_initialMapYPosition;
	private float m_initialCameraSize;

	private GameObject m_player;
	private float m_mapHeight;
	private GameObject m_star;

	public Stack<GameObject> m_lerpTargets = null;

	public GameObject m_floor = null;

	public enum State
	{
		DISPLAY_MAP,
		FOLLOW_PLAYER,
		LERP_TO_PLAYER,
		LERP_TO_TARGET,
		LERP_REST
	};
	
	public State m_currentState = State.FOLLOW_PLAYER;

	public void Start()
	{
		if(m_floor != null)
			m_initialMapYPosition = m_floor.transform.position.y;

		m_initialCameraSize = camera.orthographicSize;
		m_player = null;

		m_lerpTargets = new Stack<GameObject>();

		CalculateMapHeight();
		CreateStar();
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

		m_currentTarget.x = m_floor.transform.position.x;
		m_currentTarget.y = m_initialMapYPosition - m_mapHeight;

		m_floor.transform.position = new Vector3(m_currentTarget.x, m_currentTarget.y, m_floor.transform.position.z);

		m_star.transform.position = new Vector3(m_player.transform.position.x, m_player.transform.position.y - m_mapHeight, m_star.transform.position.z);
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

	private void CalculateMapHeight()
	{
		m_mapHeight = 0f;

		if(m_floor == null) return;

		SpriteRenderer mapRenderer = m_floor.GetComponent<SpriteRenderer>();

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
		{
			m_player = GameObject.FindWithTag("Player");

			if(m_player != null)
				m_currentTarget = m_player.transform.position;
		}
	}

	private void UpdateInput()
	{
		if(m_floor == null) return;

		if(Input.GetKeyUp(KeyCode.M))
		{
			GameObject walls = GameObject.Find("DynamicWalls");
			GameObject doors = GameObject.Find("Doors");

			if(m_currentState == State.FOLLOW_PLAYER)
			{
				m_currentState = State.DISPLAY_MAP;
				
				if(walls != null)
					walls.transform.position = new Vector3(walls.transform.position.x, walls.transform.position.y - m_mapHeight, walls.transform.position.z);
				
				if(doors != null)
					doors.transform.position = new Vector3(doors.transform.position.x, doors.transform.position.y - m_mapHeight, doors.transform.position.z);

				if(!G.getInstance().paused)
					G.getInstance().PauseMovement();
			}
			else if(m_currentState == State.DISPLAY_MAP)
			{
				m_floor.transform.position = new Vector3(m_floor.transform.position.x, m_initialMapYPosition, m_floor.transform.position.z);
				m_currentState = State.FOLLOW_PLAYER;
				m_star.SetActive(false);

				if(walls != null)
					walls.transform.position = new Vector3(walls.transform.position.x, walls.transform.position.y + m_mapHeight, walls.transform.position.z);
				
				if(doors != null)
					doors.transform.position = new Vector3(doors.transform.position.x, doors.transform.position.y + m_mapHeight, doors.transform.position.z);

				if(G.getInstance().paused)
					G.getInstance().UnpauseMovement();
			}
		}
	}

	private void LerpCameraSize()
	{
		float targetCameraSize = m_currentState == State.DISPLAY_MAP ? m_mapHeight / 2 : m_initialCameraSize;

		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetCameraSize, 2 * LERP_SPEED * Time.deltaTime);
	}
}
