using UnityEngine;
using System.Collections;
using System.IO;

public class LevelEditorRotatingWall : LevelEditorObject
{
	public const float LINE_WIDTH = 0.05f;
	public const float LINE_LENGTH = 0.5f;
	public const int TILES_FROM_PIVOT = 10;
	public const int NUMBER_OF_LIMBS = 4;
	public const int PIVOT_INDEX = 0;

	public enum Axis
	{
		East,
		North,
		West,
		South
	};

	private Axis m_growthAxis = Axis.East;
	private Axis m_rotationAxis = Axis.East;

	// Mouse
	private Vector3 m_mouseScreenPosition = Vector3.zero;
	private Vector3 m_mouseWorldPosition = Vector3.zero;

	// Line
	private LineRenderer m_lineRenderer = null;
	private Vector3 m_lineEnd = Vector3.zero;

	private bool m_isChangingGrowthAxis = false;
	private bool m_isChangingRotationAxis = false;

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

	// Transparent Sprite
	private Sprite m_transparentTile = null;

	public const int WALL_TILE_SIZE = 64; // pixels

	RotatingWall m_rotatingWallScript = null;

	Color m_growthAxisColor = Color.red;
	Color m_rotationAxisColor = Color.green;

	public void Start()
	{
		m_rotatingWallScript = GetComponent<RotatingWall>();

		InitSprites();
		InitTiles();
		InitLine();
	}

	private void InitSprites()
	{
		// Pivot Sprites
		m_pivotSingle = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/SingleUnit");
		m_pivotIntersection = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/Intersection");

		// Straight Sprites
		m_horizontalStraight = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/HorizontalStraight");
		m_verticalStraight = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/VerticalStraight");

		// Cap Sprites
		m_eastEndCap = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/EastEndCap");
		m_northEndCap = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/NorthEndCap");
		m_westEndCap = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/WestEndCap");
		m_southEndCap = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/SouthEndCap");

		// T Sprites
		m_eastNorthWestT = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/EastNorthWestT");
		m_eastNorthSouthT = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/EastNorthSouthT");
		m_eastWestSouthT = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/EastWestSouthT");
		m_northWestSouthT = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/NorthWestSouthT");

		// Corner Sprites
		m_eastNorthCorner = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/BottomLeftCorner");
		m_eastSouthCorner = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/TopLeftCorner");
		m_northWestCorner = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/BottomRightCorner");
		m_westSouthCorner = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/TopRightCorner");

		// Transparent Tile
		m_transparentTile = Resources.Load<Sprite>("Textures/LevelEditor/Tiles/RotatingWall/Transparent");
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
			Material lineMaterial = new Material("Shader \"Lines/Colored Blended\" { " +
			                              "SubShader { Pass { " +
			                              "Blend SrcAlpha OneMinusSrcAlpha " +
			                              "ZWrite Off Cull Off Fog { Mode Off } " +
			                              "BindChannels { Bind \"vertex\", vertex Bind \"color\", color } " +
			                              "} } }");
			
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

			m_lineRenderer.SetVertexCount(2);
			m_lineRenderer.material = lineMaterial;
			m_lineRenderer.SetWidth(LINE_WIDTH, LINE_WIDTH);
			m_lineRenderer.enabled = false;
		}
	}

	public void Update()
	{
		DrawLine();
	}

	public override void UpdateInput()
	{
		UpdateKeyboard();
		UpdateMouse();
		UpdateGrowthAxis();
		UpdateRotationAxis();
	}

	private void UpdateKeyboard()
	{
		if(Input.GetKeyUp(KeyCode.A))
		{
			m_isChangingGrowthAxis = false;
			m_lineRenderer.enabled = false;
		}

		if(Input.GetKeyUp(KeyCode.R))
		{
			m_isChangingRotationAxis = false;
			m_lineRenderer.enabled = false;
		}

		if(Input.GetKeyDown(KeyCode.C))
			Clear();

		if(!(m_isChangingGrowthAxis || m_isChangingRotationAxis))
		{
			// Prevent any manipulation while the wall is rotated.
			// Most, if not all, calculations are performed under the assumption that the object has not been rotated.
			if(Utilities.IsApproximately(gameObject.transform.eulerAngles.z, 0f))
			{
				if(Input.GetKeyDown(KeyCode.A))
				{
					m_isChangingGrowthAxis = true;
					m_lineRenderer.enabled = true;
				}
				
				if(Input.GetKeyDown(KeyCode.R))
				{
					m_isChangingRotationAxis = true;
					m_lineRenderer.enabled = true;
				}

				if(Input.GetKeyDown(KeyCode.KeypadPlus))
					GrowAxis();

				if(Input.GetKeyDown(KeyCode.KeypadMinus))
					ShrinkAxis();

				if(Input.GetKeyDown(KeyCode.RightArrow))
					m_growthAxis = Axis.East;
				
				if(Input.GetKeyDown(KeyCode.UpArrow))
					m_growthAxis = Axis.North;
				
				if(Input.GetKeyDown(KeyCode.LeftArrow))
					m_growthAxis = Axis.West;
				
				if(Input.GetKeyDown(KeyCode.DownArrow))
					m_growthAxis = Axis.South;
			}

			if(Input.GetKeyDown(KeyCode.S))
				SaveImage();

			if(Input.GetKeyDown(KeyCode.Space) && m_rotatingWallScript != null)
				m_rotatingWallScript.currentState = DynamicWall.STATES.TO_CHANGE;
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
		gameObject.transform.eulerAngles = new Vector3(0f, 0f, 0f);

		m_growthAxis = Axis.East;
		m_rotationAxis = Axis.East;

		m_rotatingWallScript.rotationAngles[1] = 0f;
	}

	private void GrowAxis()
	{
		if(m_tileCounts[(int)m_growthAxis] < TILES_FROM_PIVOT)
		{
			int tileCount = ++m_tileCounts[(int)m_growthAxis];
			float x = gameObject.transform.position.x;
			float y = gameObject.transform.position.y;
			Sprite sprite = null;
			SpriteRenderer tempRenderer = null;

			switch(m_growthAxis)
			{
			case Axis.East:
				x += tileCount * m_tileWidth;
				sprite = m_eastEndCap;
				break;
			
			case Axis.North:
				y += tileCount * m_tileWidth;
				sprite = m_northEndCap;
				break;
			
			case Axis.West:
				x -= tileCount * m_tileWidth;
				sprite = m_westEndCap;
				break;
			
			case Axis.South:
				y -= tileCount * m_tileWidth;
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
				tempRenderer = m_tiles[TILES_FROM_PIVOT * (int)m_growthAxis + tileCount - 1].GetComponent<SpriteRenderer>();

				if(tempRenderer != null)
				{
					if(m_growthAxis == Axis.East || m_growthAxis == Axis.West)
						tempRenderer.sprite = m_horizontalStraight;
					else if(m_growthAxis == Axis.North || m_growthAxis == Axis.South)
						tempRenderer.sprite = m_verticalStraight;
				}
			}
			
			m_tiles[TILES_FROM_PIVOT * (int)m_growthAxis + tileCount] = newTile;
			
			UpdatePivotTexture();
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
		if(m_tileCounts[(int)m_growthAxis] > 0)
		{
			int tileCount = m_tileCounts[(int)m_growthAxis];
			float x = m_tiles[TILES_FROM_PIVOT * (int)m_growthAxis + tileCount].transform.position.x;
			float y = m_tiles[TILES_FROM_PIVOT * (int)m_growthAxis + tileCount].transform.position.y;

			switch(m_growthAxis)
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
			{
				Destroy(m_tiles[TILES_FROM_PIVOT * (int)m_growthAxis + tileCount]);

				m_tiles[TILES_FROM_PIVOT * (int)m_growthAxis + tileCount] = null;
			}
			else
			{
				Destroy(m_tiles[TILES_FROM_PIVOT * (int)m_growthAxis + tileCount - 1]);

				m_tiles[TILES_FROM_PIVOT * (int)m_growthAxis + tileCount].transform.position = new Vector3(x, y, 0f);
				m_tiles[TILES_FROM_PIVOT * (int)m_growthAxis + tileCount - 1] = m_tiles[TILES_FROM_PIVOT * (int)m_growthAxis + tileCount];
				m_tiles[TILES_FROM_PIVOT * (int)m_growthAxis + tileCount] = null;
			}

			m_tileCounts[(int)m_growthAxis]--;

			UpdatePivotTexture();
		}
	}

	private void SaveImage()
	{
		Texture2D saveTexture = new Texture2D(WALL_TILE_SIZE * (m_tileCounts[(int)Axis.East] + m_tileCounts[(int)Axis.West] + 1), WALL_TILE_SIZE * (m_tileCounts[(int)Axis.North] + m_tileCounts[(int)Axis.South] + 1), TextureFormat.ARGB32, false);

		Color[] pixelsTemp = null;

		// Pivot
		pixelsTemp = m_pivotRenderer.sprite.texture.GetPixels();

		saveTexture.SetPixels(m_tileCounts[(int)Axis.West] * WALL_TILE_SIZE, m_tileCounts[(int)Axis.South] * WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, pixelsTemp);

		// East Axis
		for(int i = 0; i < m_tileCounts[(int)Axis.East]; i++)
		{
			if(i == m_tileCounts[(int)Axis.East] - 1)
				pixelsTemp = m_eastEndCap.texture.GetPixels();
			else
				pixelsTemp = m_horizontalStraight.texture.GetPixels();

			saveTexture.SetPixels(m_tileCounts[(int)Axis.West] * WALL_TILE_SIZE + WALL_TILE_SIZE + WALL_TILE_SIZE * i, m_tileCounts[(int)Axis.South] * WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, pixelsTemp);
		}

		// North Axis
		for(int i = 0; i < m_tileCounts[(int)Axis.North]; i++)
		{
			if(i == m_tileCounts[(int)Axis.North] - 1)
				pixelsTemp = m_northEndCap.texture.GetPixels();
			else
				pixelsTemp = m_verticalStraight.texture.GetPixels();
			
			saveTexture.SetPixels(m_tileCounts[(int)Axis.West] * WALL_TILE_SIZE, m_tileCounts[(int)Axis.South] * WALL_TILE_SIZE + WALL_TILE_SIZE + WALL_TILE_SIZE * i, WALL_TILE_SIZE, WALL_TILE_SIZE, pixelsTemp);
		}

		// West Axis
		for(int i = m_tileCounts[(int)Axis.West] - 1; i >= 0; i--)
		{
			if(i == 0)
				pixelsTemp = m_westEndCap.texture.GetPixels();
			else
				pixelsTemp = m_horizontalStraight.texture.GetPixels();
			
			saveTexture.SetPixels(i * WALL_TILE_SIZE, m_tileCounts[(int)Axis.South] * WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, pixelsTemp);
		}

		// South Axis
		for(int i = m_tileCounts[(int)Axis.South] - 1; i >= 0; i--)
		{
			if(i == 0)
				pixelsTemp = m_southEndCap.texture.GetPixels();
			else
				pixelsTemp = m_verticalStraight.texture.GetPixels();
			
			saveTexture.SetPixels(m_tileCounts[(int)Axis.West] * WALL_TILE_SIZE, i * WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, pixelsTemp);
		}

		// Transparency
		if(m_transparentTile != null)
			pixelsTemp = m_transparentTile.texture.GetPixels();

		// West/South Transparency
		for(int y = 0; y < m_tileCounts[(int)Axis.South]; y++)
		{
			for(int x = 0; x < m_tileCounts[(int)Axis.East]; x++)
			    saveTexture.SetPixels(x * WALL_TILE_SIZE + m_tileCounts[(int)Axis.West] * WALL_TILE_SIZE + WALL_TILE_SIZE, y * WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, pixelsTemp);

			for(int x = 0; x < m_tileCounts[(int)Axis.West]; x++)
			    saveTexture.SetPixels(x * WALL_TILE_SIZE, y * WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, pixelsTemp);
		}

		// North/West Transparency
		for(int y = 0; y < m_tileCounts[(int)Axis.North]; y++)
		{
			for(int x = 0; x < m_tileCounts[(int)Axis.East]; x++)
				saveTexture.SetPixels(x * WALL_TILE_SIZE + m_tileCounts[(int)Axis.West] * WALL_TILE_SIZE + WALL_TILE_SIZE, y * WALL_TILE_SIZE + m_tileCounts[(int)Axis.South] * WALL_TILE_SIZE + WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, pixelsTemp);

			for(int x = 0; x < m_tileCounts[(int)Axis.West]; x++)
				saveTexture.SetPixels(x * WALL_TILE_SIZE, y * WALL_TILE_SIZE + m_tileCounts[(int)Axis.South] * WALL_TILE_SIZE + WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, pixelsTemp);
		}

		File.WriteAllBytes("rotatingWall.png", saveTexture.EncodeToPNG());
		
		Destroy(saveTexture);
	}

	private void UpdateMouse()
	{
		m_mouseScreenPosition.x = Input.mousePosition.x;
		m_mouseScreenPosition.y = Input.mousePosition.y;
		m_mouseScreenPosition.z = -Camera.main.transform.position.z;
		
		m_mouseWorldPosition = Camera.main.ScreenToWorldPoint(m_mouseScreenPosition);
	}

	private void UpdateGrowthAxis()
	{
		if(m_isChangingGrowthAxis)
		{
			Vector3 dir = m_mouseWorldPosition - transform.position;
			dir.Normalize();

			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

			if(angle < 0f)
				angle += 360f;

			angle = Mathf.Round(angle / 90f) * 90f;
			angle %= 360f;

			if(Utilities.IsApproximately(angle, 0f))
				m_growthAxis = Axis.East;
			else if(Utilities.IsApproximately(angle, 90f))
				m_growthAxis = Axis.North;
			else if(Utilities.IsApproximately(angle, 180f))
				m_growthAxis = Axis.West;
			else if(Utilities.IsApproximately(angle, 270f))
				m_growthAxis = Axis.South;
		}
	}

	private void UpdateRotationAxis()
	{
		if(m_isChangingRotationAxis)
		{
			Vector3 dir = m_mouseWorldPosition - transform.position;
			dir.Normalize();
			
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			
			if(angle < 0f)
				angle += 360f;
			
			angle = Mathf.Round(angle / 90f) * 90f;
			angle %= 360f;

			if(m_rotatingWallScript != null && m_rotatingWallScript.rotationAngles.Length == 2)
			{
				if(Utilities.IsApproximately(angle, 0f))
				{
					m_rotationAxis = Axis.East;
					m_rotatingWallScript.rotationAngles[1] = 0f;
				}
				else if(Utilities.IsApproximately(angle, 90f))
				{
					m_rotationAxis = Axis.North;
					m_rotatingWallScript.rotationAngles[1] = 90f;
				}
				else if(Utilities.IsApproximately(angle, 180f))
				{
					m_rotationAxis = Axis.West;
					m_rotatingWallScript.rotationAngles[1] = 180f;
				}
				else if(Utilities.IsApproximately(angle, 270f))
				{
					m_rotationAxis = Axis.South;
					m_rotatingWallScript.rotationAngles[1] = 270f;
				}
			}
		}
	}

	private void DrawLine()
	{
		if(m_lineRenderer != null && (m_isChangingGrowthAxis || m_isChangingRotationAxis))
		{
			Color lineColor = m_isChangingGrowthAxis ? m_growthAxisColor : m_rotationAxisColor;

			m_lineRenderer.SetColors(lineColor, lineColor);
			m_lineRenderer.SetPosition(0, gameObject.transform.position);
			m_lineEnd = transform.position;

			switch(m_isChangingGrowthAxis ? m_growthAxis : m_rotationAxis)
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
