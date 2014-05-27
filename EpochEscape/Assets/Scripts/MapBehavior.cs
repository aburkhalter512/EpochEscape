using UnityEngine;
using System.Collections.Generic;
using G = GameManager;

public class MapBehavior : MonoBehaviour
{
	public const float LERP_SPEED = 10f;

	public Vector3 m_currentTarget;

	private float m_initialMapYPosition;
	private float m_initialCameraSize;

	private GameObject m_player;
	private float m_mapHeight;
	private GameObject m_star;

	public Stack<GameObject> m_lerpTargets = null;
	public Stack<GameObject> m_toActuate = null;

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
		m_initialMapYPosition = transform.position.y;
		m_initialCameraSize = Camera.main.orthographicSize;
		m_player = null;

		m_lerpTargets = new Stack<GameObject>();
		m_toActuate = new Stack<GameObject>();

		CalculateMapHeight();
		CreateStar();
	}
	
	public void Update()
	{
		CheckForPlayer();

		if(m_player == null) return;
		
		if(m_lerpTargets.Count != 0 && m_currentState != State.LERP_REST)
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
		if(!G.getInstance().paused)
			G.getInstance().PauseMovement();

		m_currentTarget.x = transform.position.x;
		m_currentTarget.y = m_initialMapYPosition - m_mapHeight;

		transform.position = new Vector3(m_currentTarget.x, m_currentTarget.y, transform.position.z);

		m_star.transform.position = new Vector3(m_player.transform.position.x, m_player.transform.position.y - m_mapHeight, m_star.transform.position.z);
		m_star.SetActive(true);

		Camera.main.transform.position = new Vector3(m_currentTarget.x, m_currentTarget.y, Camera.main.transform.position.z);
	}

	private void FollowPlayer()
	{
		if(m_player != null)
		{
			m_currentTarget.x = m_player.transform.position.x;
			m_currentTarget.y = m_player.transform.position.y;

			transform.position = new Vector3(transform.position.x, m_initialMapYPosition, transform.position.z);

			Camera.main.transform.position = new Vector3(m_currentTarget.x, m_currentTarget.y, Camera.main.transform.position.z);
		}
	}

	private void LerpToPlayer()
	{
		Vector3 currentCameraPosition = Camera.main.transform.position;
		Vector3 targetCameraPosition = new Vector3(m_currentTarget.x, m_currentTarget.y, currentCameraPosition.z);
		
		Camera.main.transform.position = Vector3.Lerp(currentCameraPosition, targetCameraPosition, LERP_SPEED * Time.deltaTime);
		
		if(Utilities.IsApproximately(currentCameraPosition.x, targetCameraPosition.x) && Utilities.IsApproximately(currentCameraPosition.y, targetCameraPosition.y))
		{
			currentCameraPosition = targetCameraPosition;

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

		Vector3 currentCameraPosition = Camera.main.transform.position;
		Vector3 targetCameraPosition = new Vector3(m_currentTarget.x, m_currentTarget.y, currentCameraPosition.z);
		
		Camera.main.transform.position = Vector3.Lerp(currentCameraPosition, targetCameraPosition, LERP_SPEED * Time.deltaTime);

		if(Utilities.IsApproximately(currentCameraPosition.x, targetCameraPosition.x) && Utilities.IsApproximately(currentCameraPosition.y, targetCameraPosition.y))
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

			//m_toActuate.Push(m_lerpTargets.Pop());

			currentCameraPosition = targetCameraPosition;

			/*
			if(m_lerpTargets.Count == 0)
			{
				m_currentState = State.LERP_REST;

				while(m_toActuate.Count != 0)
					m_toActuate.Pop().GetComponent<DynamicWall>().currentState = DynamicWall.STATES.TO_CHANGE;
			}*/

			m_currentState = State.LERP_REST;
		}
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
		{
			m_player = GameObject.FindWithTag("Player");

			if(m_player != null)
				m_currentTarget = m_player.transform.position;
		}
	}

	private void UpdateInput()
	{
		if(Input.GetKeyUp(KeyCode.M))
		{
			if(m_currentState == State.FOLLOW_PLAYER)
			{
				m_currentState = State.DISPLAY_MAP;

				if(!G.getInstance().paused)
					G.getInstance().PauseMovement();
			}
			else if(m_currentState == State.DISPLAY_MAP)
			{
				m_currentState = State.FOLLOW_PLAYER;
				m_star.SetActive(false);

				if(G.getInstance().paused)
					G.getInstance().UnpauseMovement();
			}
		}
	}

	private void LerpCameraSize()
	{
		float targetCameraSize = m_currentState == State.DISPLAY_MAP ? m_mapHeight / 2 : m_initialCameraSize;

		Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetCameraSize, 10f * Time.deltaTime);
	}
}
