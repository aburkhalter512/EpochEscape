using UnityEngine;
using System.Collections;

public class LevelEditorPivot : MonoBehaviour
{
	public const float LINE_WIDTH = 0.05f;
	public const float LINE_LENGTH = 0.5f;
	public const int TILES_FROM_PIVOT = 5;
	public const int NUMBER_OF_LIMBS = 4;
	public const int PIVOT_INDEX = 0;

	public enum Axis
	{
		East,
		North,
		West,
		South
	};

	private Axis m_currentAxis = Axis.East;

	// Mouse
	private Vector3 m_mouseScreenPosition = Vector3.zero;
	private Vector3 m_mouseWorldPosition = Vector3.zero;

	// Line
	private LineRenderer m_lineRenderer = null;
	private Vector3 m_lineEnd = Vector3.zero;

	private bool m_isChangingAxis = false;

	private GameObject[] m_tiles = null;
	private int[] m_tileCounts = null;

	private float m_tileWidth = 0f;

	private SpriteRenderer m_pivotRenderer = null;

	// Pivot Sprites
	private Sprite m_pivotSingle = null;
	private Sprite m_pivotIntersection = null;

	// Straight Sprites
	private Sprite m_horizontalStraight = null;
	private Sprite m_verticalStraight = null;

	// Cap Sprites
	private Sprite m_eastEndCap = null;
	private Sprite m_northEndCap = null;
	private Sprite m_westEndCap = null;
	private Sprite m_southEndCap = null;

	// T Sprites
	private Sprite m_eastNorthWestT = null;
	private Sprite m_eastNorthSouthT = null;
	private Sprite m_eastWestSouthT = null;
	private Sprite m_northWestSouthT = null;

	// Corner Sprites
	private Sprite m_eastNorthCorner = null;
	private Sprite m_eastSouthCorner = null;
	private Sprite m_northWestCorner = null;
	private Sprite m_westSouthCorner = null;

	public void Start()
	{
		InitSprites();
		InitTiles();
		InitLine();
	}

	private void InitSprites()
	{
		// Pivot Sprites
		m_pivotSingle = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Wall - Single Unit");
		m_pivotIntersection = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Wall - Intersection");

		// Straight Sprites
		m_horizontalStraight = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Wall - HorizontalStraight");
		m_verticalStraight = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Wall - VerticalStraight");

		// Cap Sprites
		m_eastEndCap = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Wall - East End Cap");
		m_northEndCap = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Wall - North End Cap");
		m_westEndCap = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Wall - West End Cap");
		m_southEndCap = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Wall - South End Cap");

		// T Sprites
		m_eastNorthWestT = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Walls - East North West T");
		m_eastNorthSouthT = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Walls - East North South T");
		m_eastWestSouthT = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Walls - East West South T");
		m_northWestSouthT = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Walls - North West South T");

		// Corner Sprites
		m_eastNorthCorner = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Walls - BottomLeftCorner");
		m_eastSouthCorner = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Walls - TopLeftCorner");
		m_northWestCorner = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Walls - BottomRightCorner");
		m_westSouthCorner = Resources.Load<Sprite>("Textures/Tiles/Walls/200/Walls - TopRightCorner");
	}

	private void InitTiles()
	{
		m_tiles = new GameObject[TILES_FROM_PIVOT * NUMBER_OF_LIMBS + 1];

		for(int i = 1; i < TILES_FROM_PIVOT * NUMBER_OF_LIMBS + 1; i++)
			m_tiles[i] = null;

		m_tiles[PIVOT_INDEX] = gameObject;

		// ---

		m_tileCounts = new int[NUMBER_OF_LIMBS];

		for(int i = 0; i < NUMBER_OF_LIMBS; i++)
			m_tileCounts[i] = 0;

		// ---

		m_pivotRenderer = gameObject.GetComponent<SpriteRenderer>();

		if(m_pivotRenderer != null)
			m_tileWidth = m_pivotRenderer.sprite.bounds.size.x;
	}

	private void InitLine()
	{
		m_lineRenderer = gameObject.AddComponent<LineRenderer>();
		
		if(m_lineRenderer != null)
		{
			m_lineRenderer.SetVertexCount(2);
			m_lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
			m_lineRenderer.SetColors(Color.red, Color.red);
			m_lineRenderer.SetWidth(LINE_WIDTH, LINE_WIDTH);
			m_lineRenderer.enabled = false;
		}
	}

	public void Update()
	{
		UpdateInput();
		DrawLine();
	}

	private void UpdateInput()
	{
		UpdateKeyboard();
		UpdateMouse();
		UpdateAxis();
	}

	private void UpdateKeyboard()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			m_isChangingAxis = true;
			m_lineRenderer.enabled = true;
		}

		if(Input.GetKeyUp(KeyCode.A))
		{
			m_isChangingAxis = false;
			m_lineRenderer.enabled = false;
		}

		if(Input.GetKeyDown(KeyCode.C))
			Clear();

		if(!m_isChangingAxis)
		{
			if(Input.GetKeyDown(KeyCode.KeypadPlus))
				GrowAxis();

			if(Input.GetKeyDown(KeyCode.KeypadMinus))
				ShrinkAxis();

			if(Input.GetKeyDown(KeyCode.RightArrow))
				m_currentAxis = Axis.East;

			if(Input.GetKeyDown(KeyCode.UpArrow))
				m_currentAxis = Axis.North;

			if(Input.GetKeyDown(KeyCode.LeftArrow))
				m_currentAxis = Axis.West;

			if(Input.GetKeyDown(KeyCode.DownArrow))
				m_currentAxis = Axis.South;
		}
	}

	private void Clear()
	{
		for(int i = 1; i < TILES_FROM_PIVOT * NUMBER_OF_LIMBS + 1; i++)
		{
			Destroy(m_tiles[i]);

			m_tiles[i] = null;
		}

		for(int i = 0; i < NUMBER_OF_LIMBS; i++)
			m_tileCounts[i] = 0;

		m_pivotRenderer.sprite = m_pivotSingle;
	}

	private void GrowAxis()
	{
		if(m_tileCounts[(int)m_currentAxis] < TILES_FROM_PIVOT)
		{
			int tileCount = ++m_tileCounts[(int)m_currentAxis];
			float x = 0f;
			float y = 0f;
			Sprite sprite = null;
			SpriteRenderer tempRenderer = null;

			switch(m_currentAxis)
			{
			case Axis.East:
				x = tileCount * m_tileWidth;
				sprite = m_eastEndCap;
				break;
			
			case Axis.North:
				y = tileCount * m_tileWidth;
				sprite = m_northEndCap;
				break;
			
			case Axis.West:
				x = tileCount * -m_tileWidth;
				sprite = m_westEndCap;
				break;
			
			case Axis.South:
				y = tileCount * -m_tileWidth;
				sprite = m_southEndCap;
				break;
			}
			
			GameObject newTile = new GameObject();
			newTile.transform.position = new Vector3(x, y, 0f);
			newTile.name = "Limb";
			newTile.transform.parent = gameObject.transform;

			tempRenderer = newTile.AddComponent<SpriteRenderer>();
			tempRenderer.sprite = sprite;

			if(tileCount > 1)
			{
				tempRenderer = m_tiles[TILES_FROM_PIVOT * (int)m_currentAxis + tileCount - 1].GetComponent<SpriteRenderer>();

				if(m_currentAxis == Axis.East || m_currentAxis == Axis.West)
					tempRenderer.sprite = m_horizontalStraight;
				else if(m_currentAxis == Axis.North || m_currentAxis == Axis.South)
					tempRenderer.sprite = m_verticalStraight;
			}
			
			m_tiles[TILES_FROM_PIVOT * (int)m_currentAxis + tileCount] = newTile;
			
			UpdatePivotTexture();

			/*
			if(newTile != null)
			{
				newTile = Instantiate(newTile) as GameObject;
				
				if(newTile != null)
				{

				}
			}*/
		}
	}

	private void UpdatePivotTexture()
	{
		if(m_pivotRenderer != null)
		{
			bool isEast = m_tileCounts[(int)Axis.East] > 0;
			bool isNorth = m_tileCounts[(int)Axis.North] > 0;
			bool isWest = m_tileCounts[(int)Axis.West] > 0;
			bool isSouth = m_tileCounts[(int)Axis.South] > 0;

			if(isEast && isNorth && isWest && isSouth)
				m_pivotRenderer.sprite = m_pivotIntersection;
			else if(isEast && !(isNorth || isWest || isSouth))
				m_pivotRenderer.sprite = m_westEndCap;
			else if(isNorth && !(isEast || isWest || isSouth))
				m_pivotRenderer.sprite = m_southEndCap;
			else if(isWest && !(isEast || isNorth || isSouth))
				m_pivotRenderer.sprite = m_eastEndCap;
			else if(isSouth && !(isEast || isNorth || isWest))
				m_pivotRenderer.sprite = m_northEndCap;
			else if(!(isEast || isNorth || isWest || isSouth))
				m_pivotRenderer.sprite = m_pivotSingle;
			else if(isEast && isNorth && isWest && !isSouth)
				m_pivotRenderer.sprite = m_eastNorthWestT;
			else if(isEast && isNorth && isSouth && !isWest)
				m_pivotRenderer.sprite = m_eastNorthSouthT;
			else if(isEast && isWest && isSouth && !isNorth)
				m_pivotRenderer.sprite = m_eastWestSouthT;
			else if(isNorth && isWest && isSouth && !isEast)
				m_pivotRenderer.sprite = m_northWestSouthT;
			else if(isEast && isNorth && !(isWest || isSouth))
				m_pivotRenderer.sprite = m_eastNorthCorner;
			else if(isEast && isSouth && !(isNorth || isWest))
				m_pivotRenderer.sprite = m_eastSouthCorner;
			else if(isNorth && isWest && !(isEast || isSouth))
				m_pivotRenderer.sprite = m_northWestCorner;
			else if(isWest && isSouth && !(isEast || isNorth))
				m_pivotRenderer.sprite = m_westSouthCorner;
			else if(isEast && isWest && !(isNorth || isSouth))
				m_pivotRenderer.sprite = m_horizontalStraight;
			else if(isNorth && isSouth && !(isEast || isWest))
				m_pivotRenderer.sprite = m_verticalStraight;
		}
	}

	private void ShrinkAxis()
	{
		if(m_tileCounts[(int)m_currentAxis] > 0)
		{
			int tileCount = m_tileCounts[(int)m_currentAxis];
			float x = m_tiles[TILES_FROM_PIVOT * (int)m_currentAxis + tileCount].transform.position.x;
			float y = m_tiles[TILES_FROM_PIVOT * (int)m_currentAxis + tileCount].transform.position.y;

			switch(m_currentAxis)
			{
			case Axis.East:
				x -= m_tileWidth;
				break;
				
			case Axis.North:
				y -= m_tileWidth;
				break;
				
			case Axis.West:
				x += m_tileWidth;
				break;
				
			case Axis.South:
				y += m_tileWidth;
				break;
			}

			if(tileCount == 1)
				Destroy(m_tiles[TILES_FROM_PIVOT * (int)m_currentAxis + tileCount]);
			else
			{
				Destroy(m_tiles[TILES_FROM_PIVOT * (int)m_currentAxis + tileCount - 1]);

				m_tiles[TILES_FROM_PIVOT * (int)m_currentAxis + tileCount].transform.position = new Vector3(x, y, 0f);
				m_tiles[TILES_FROM_PIVOT * (int)m_currentAxis + tileCount - 1] = m_tiles[TILES_FROM_PIVOT * (int)m_currentAxis + tileCount];
				m_tiles[TILES_FROM_PIVOT * (int)m_currentAxis + tileCount] = null;
			}

			m_tileCounts[(int)m_currentAxis]--;

			UpdatePivotTexture();
		}
	}

	private void UpdateMouse()
	{
		m_mouseScreenPosition.x = Input.mousePosition.x;
		m_mouseScreenPosition.y = Input.mousePosition.y;
		m_mouseScreenPosition.z = -Camera.main.transform.position.z;
		
		m_mouseWorldPosition = Camera.main.ScreenToWorldPoint(m_mouseScreenPosition);
	}

	private void UpdateAxis()
	{
		if(m_isChangingAxis)
		{
			Vector3 dir = m_mouseWorldPosition - transform.position;
			dir.Normalize();

			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

			if(angle < 0f)
				angle += 360f;

			angle = Mathf.Round(angle / 90f) * 90f;
			angle %= 360f;

			if(Utilities.IsApproximately(angle, 0f))
				m_currentAxis = Axis.East;
			else if(Utilities.IsApproximately(angle, 90f))
				m_currentAxis = Axis.North;
			else if(Utilities.IsApproximately(angle, 180f))
				m_currentAxis = Axis.West;
			else if(Utilities.IsApproximately(angle, 270f))
				m_currentAxis = Axis.South;
		}
	}

	private void DrawLine()
	{
		if(m_lineRenderer != null && m_isChangingAxis)
		{
			m_lineRenderer.SetPosition(0, gameObject.transform.position);
			m_lineEnd = transform.position;

			switch(m_currentAxis)
			{
			case Axis.East:
				m_lineEnd.x += LINE_LENGTH;
				break;
			
			case Axis.North:
				m_lineEnd.y += LINE_LENGTH;
				break;
			
			case Axis.West:
				m_lineEnd.x -= LINE_LENGTH;
				break;
			
			case Axis.South:
				m_lineEnd.y -= LINE_LENGTH;
				break;
			}

			m_lineRenderer.SetPosition(1, m_lineEnd);
		}
	}
}
