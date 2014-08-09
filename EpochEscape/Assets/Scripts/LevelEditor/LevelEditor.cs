using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using U = LevelEditorUtilities;

public class LevelEditor : MonoBehaviour
{
	public const float INITIAL_CAMERA_SIZE = 1.375f;
	public const float INITIAL_CAMERA_MOVE_SPEED = 0.1f;
	public const float CAMERA_ZOOM_INCREMENT = 1.5f;
	public const float CAMERA_ZOOM_MIN = 0.5f;
	public const float CAMERA_ZOOM_MAX = 7.5f;
	public const float CAMERA_ZOOM_SPEED = 10f;
	public const int DEFAULT_FLOOR_WIDTH = 20; // units
	public const int DEFAULT_FLOOR_HEIGHT = 20; //units
	
	public const int MIN_FLOOR_WIDTH = 2; // units
	public const int MIN_FLOOR_HEIGHT = 2; // units
	public const int MAX_FLOOR_WIDTH = 96; // units
	public const int MAX_FLOOR_HEIGHT = 96; // units
	
	public const int FLOOR_INCREMENT_STEP = 2; // units
	
	public const int FLOOR_TILE_SIZE = 100; // pixels
	public const int WALL_TILE_SIZE = 200; // pixels
	
	public const float CAMERA_ROTATION_SPEED = 25f;
	
	public enum MouseButton
	{
		Left,
		Right,
		Middle
	};
	
	public enum Axis
	{
		X,
		Y,
		Both
	};
	
	private Axis m_incrementAxis = Axis.Both;
	
	public bool m_showGrid = true;
	
	private float m_currentSize = 0f;
	private float m_targetSize = 0f;
	private bool m_moveCamera = false;
	
	private Color m_gridColor = new Color(0.2f, 0.2f, 0.2f, 1f);
	private Material m_gridMaterial = null;
	private bool m_isLerping = false;
	
	private int m_floorWidth = DEFAULT_FLOOR_WIDTH;
	private int m_floorHeight = DEFAULT_FLOOR_HEIGHT;
	
	private int m_floorWidthTemp = DEFAULT_FLOOR_WIDTH;
	private int m_floorHeightTemp = DEFAULT_FLOOR_HEIGHT;
	
	private GameObject[,] m_tiles = null;
	private GameObject[,] m_exteriorWalls = null;
	
	// Mouse
	private Vector3 m_mouseScreenPosition = Vector3.zero;
	private Vector3 m_mouseWorldPosition = Vector3.zero;
	
	private int m_mouseLogicalX = 0;
	private int m_mouseLogicalY = 0;
	
	// Painter
	private GameObject m_tile = null;
	private SpriteRenderer m_tileRenderer = null;
	float m_tileRendererX = 0f;
	float m_tileRendererY = 0f;
	float m_tileX = 0f;
	float m_tileY = 0f;

	// Selected Object
	public static GameObject s_selectedObject = null;
	public static LevelEditorObject s_selectedObjectScript = null;
	public static SpriteRenderer s_selectedObjectRenderer = null;

	public static void SetSelectedObject(GameObject selectedObject)
	{
		if(selectedObject != null)
		{
			s_selectedObject = selectedObject;
			s_selectedObjectScript = selectedObject.GetComponent<LevelEditorObject>();
			s_selectedObjectRenderer = selectedObject.GetComponent<SpriteRenderer>();
		}
	}
	
	public void Start()
	{
		m_currentSize = Camera.main.orthographicSize;
		m_targetSize = Camera.main.orthographicSize;
		
		InitializeTile();
		InitializeTiles();
	}
	
	public void Update()
	{
		if(s_selectedObject == null)
			UpdateInput();
		else if(s_selectedObjectScript != null)
			s_selectedObjectScript.UpdateInput();

		UpdateGlobalInput();
	}
	
	private void UpdateInput()
	{
		UpdateMiscKeyboardCommands();
		UpdatePainter();
	}

	private void UpdateGlobalInput()
	{
		UpdateMouse();
		UpdateCamera();
		UpdateGrid();
		UpdateSaveManager();
		UpdateLoadManager();
	}
	
	private void UpdateMouse()
	{
		m_mouseScreenPosition.x = Input.mousePosition.x;
		m_mouseScreenPosition.y = Input.mousePosition.y;
		m_mouseScreenPosition.z = -Camera.main.transform.position.z;
		
		m_mouseWorldPosition = Camera.main.ScreenToWorldPoint(m_mouseScreenPosition);
	}
	
	private void UpdateCamera()
	{
		UpdateCameraMovement();
		UpdateCameraZoom();
	}
	
	private void UpdateCameraMovement()
	{
		if(Input.GetMouseButtonDown((int)MouseButton.Middle))
			m_moveCamera = true;
		
		if(Input.GetMouseButtonUp((int)MouseButton.Middle))
			m_moveCamera = false;
		
		if(m_moveCamera)
		{
			Vector3 cameraPosition = Vector3.zero;
			
			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = Input.GetAxis("Mouse Y");
			
			Camera.main.transform.position += (INITIAL_CAMERA_MOVE_SPEED / INITIAL_CAMERA_SIZE) * Camera.main.orthographicSize * new Vector3(-mouseX, -mouseY, 0f);
		}
	}
	
	private void UpdateCameraZoom()
	{
		m_currentSize = Camera.main.orthographicSize;
		
		if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
		{
			m_targetSize = m_currentSize - CAMERA_ZOOM_INCREMENT;
			m_isLerping = true;
			
			if(m_targetSize < CAMERA_ZOOM_MIN)
				m_targetSize = CAMERA_ZOOM_MIN;
		}
		else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
		{
			m_targetSize = m_currentSize + CAMERA_ZOOM_INCREMENT;
			m_isLerping = true;
			
			if(m_targetSize > CAMERA_ZOOM_MAX)
				m_targetSize = CAMERA_ZOOM_MAX;
		}
		
		if(m_isLerping)
		{
			Camera.main.orthographicSize = Mathf.Lerp(m_currentSize, m_targetSize, CAMERA_ZOOM_SPEED * Time.deltaTime);
			
			if(Utilities.IsApproximately(m_currentSize, m_targetSize))
			{
				m_isLerping = false;
				
				Camera.main.orthographicSize = m_targetSize;
			}
		}
	}
	
	private void UpdateGrid()
	{
		if(Input.GetKeyUp(KeyCode.G))
			m_showGrid = !m_showGrid;
	}
	
	private void UpdateSaveManager()
	{
		if(Input.GetKeyUp(KeyCode.S))
			StartCoroutine(Save());
	}
	
	private IEnumerator Save()
	{
		PrintStatusMessage("Encoding image file, please wait...");
		
		yield return new WaitForSeconds(0.1f);
		
		SaveImage();
		
		PrintStatusMessage("Saving data file, please wait...");
		
		yield return new WaitForSeconds(0.1f);
		
		SaveData();
		
		PrintStatusMessage("Export finished.");
	}
	
	private void SaveImage()
	{
		GameObject emptyTile = Resources.Load("Prefabs/Tiles/Floor/Empty") as GameObject;
		
		if(emptyTile == null)
			return;
		
		SpriteRenderer emptyTileRenderer = emptyTile.GetComponent<SpriteRenderer>();
		
		if(emptyTileRenderer == null)
			return;
		
		Texture2D saveTexture = new Texture2D(m_floorWidth * FLOOR_TILE_SIZE + WALL_TILE_SIZE * 2, m_floorHeight * FLOOR_TILE_SIZE + WALL_TILE_SIZE * 2, TextureFormat.ARGB32, false);
		
		SpriteRenderer spriteRendererTemp = null;
		Color[] pixelsTemp = null;
		
		for(int y = m_floorHeight - 1; y >= 0; y--)
		{
			for(int x = 0; x < m_floorWidth; x++)
			{
				if(m_tiles[y, x] == null)
					pixelsTemp = emptyTileRenderer.sprite.texture.GetPixels();
				else
				{
					spriteRendererTemp = m_tiles[y, x].GetComponent<SpriteRenderer>();
					
					if(spriteRendererTemp == null)
						return;
					
					pixelsTemp = spriteRendererTemp.sprite.texture.GetPixels();
				}
				
				saveTexture.SetPixels(FLOOR_TILE_SIZE * x + WALL_TILE_SIZE, FLOOR_TILE_SIZE * y + WALL_TILE_SIZE, FLOOR_TILE_SIZE, FLOOR_TILE_SIZE, pixelsTemp);
				
				if(y == m_floorHeight - 1)
				{
					if(x == 0)
					{
						spriteRendererTemp = m_exteriorWalls[0, 0].GetComponent<SpriteRenderer>();
						
						if(spriteRendererTemp == null)
							return;
						
						saveTexture.SetPixels(0, 0, WALL_TILE_SIZE, WALL_TILE_SIZE, spriteRendererTemp.sprite.texture.GetPixels());
					}
					
					if(x % 2 == 0)
					{
						spriteRendererTemp = m_exteriorWalls[0, x / 2 + 1].GetComponent<SpriteRenderer>();
						
						if(spriteRendererTemp == null)
							return;
						
						saveTexture.SetPixels((x / 2 + 1) * WALL_TILE_SIZE, 0, WALL_TILE_SIZE, WALL_TILE_SIZE, spriteRendererTemp.sprite.texture.GetPixels());
					}
					
					if(x == m_floorWidth - 1)
					{
						spriteRendererTemp = m_exteriorWalls[0, m_floorWidth / 2 + 1].GetComponent<SpriteRenderer>();
						
						if(spriteRendererTemp == null)
							return;
						
						saveTexture.SetPixels((m_floorWidth / 2 + 1) * WALL_TILE_SIZE, 0, WALL_TILE_SIZE, WALL_TILE_SIZE, spriteRendererTemp.sprite.texture.GetPixels());
					}
				}
				
				if(y == 0)
				{
					if(x == 0)
					{
						spriteRendererTemp = m_exteriorWalls[m_floorHeight / 2 + 1, 0].GetComponent<SpriteRenderer>();
						
						if(spriteRendererTemp == null)
							return;
						
						saveTexture.SetPixels(0, m_floorHeight * FLOOR_TILE_SIZE + WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, spriteRendererTemp.sprite.texture.GetPixels());
					}
					
					if(x % 2 == 0)
					{
						spriteRendererTemp = m_exteriorWalls[m_floorHeight / 2 + 1, x / 2 + 1].GetComponent<SpriteRenderer>();
						
						if(spriteRendererTemp == null)
							return;
						
						saveTexture.SetPixels((x / 2 + 1) * WALL_TILE_SIZE, m_floorHeight * FLOOR_TILE_SIZE + WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, spriteRendererTemp.sprite.texture.GetPixels());
					}
					
					if(x == m_floorWidth - 1)
					{
						spriteRendererTemp = m_exteriorWalls[m_floorHeight / 2 + 1, m_floorWidth / 2 + 1].GetComponent<SpriteRenderer>();
						
						if(spriteRendererTemp == null)
							return;
						
						saveTexture.SetPixels((m_floorWidth / 2 + 1) * WALL_TILE_SIZE, m_floorHeight * FLOOR_TILE_SIZE + WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, spriteRendererTemp.sprite.texture.GetPixels());
					}
				}
				
				if(x == 0)
				{
					if(y % 2 == 0)
					{
						spriteRendererTemp = m_exteriorWalls[y / 2 + 1, 0].GetComponent<SpriteRenderer>();
						
						if(spriteRendererTemp == null)
							return;
						
						saveTexture.SetPixels(0, y * FLOOR_TILE_SIZE + WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, spriteRendererTemp.sprite.texture.GetPixels());
					}
				}
				
				if(x == m_floorWidth - 1)
				{
					if(y % 2 == 0)
					{
						spriteRendererTemp = m_exteriorWalls[y / 2 + 1, m_floorWidth / 2 + 1].GetComponent<SpriteRenderer>();
						
						if(spriteRendererTemp == null)
							return;
						
						saveTexture.SetPixels(m_floorWidth * FLOOR_TILE_SIZE + WALL_TILE_SIZE, y * FLOOR_TILE_SIZE + WALL_TILE_SIZE, WALL_TILE_SIZE, WALL_TILE_SIZE, spriteRendererTemp.sprite.texture.GetPixels());
					}
				}
			}
		}
		
		File.WriteAllBytes("floor.png", saveTexture.EncodeToPNG());
		
		Destroy(saveTexture);
	}
	
	private void SaveData()
	{
		using(StreamWriter sw = new StreamWriter("floor.json"))
		{
			sw.WriteLine("{");
			sw.WriteLine("\t" + U.Escape("size") + ":{");
			sw.WriteLine(U.Tab(2) + U.Escape("width") + ":" + m_floorWidth + ",");
			sw.WriteLine(U.Tab(2) + U.Escape("height") + ":" + m_floorHeight);
			sw.WriteLine("\t},");
			sw.WriteLine("\t" + U.Escape("tiles") + ":[");
			
			for(int y = 0; y < m_floorHeight; y++)
			{
				for(int x = 0; x < m_floorWidth; x++)
				{
					if(m_tiles[y, x] == null)
						continue;
					
					sw.WriteLine(U.Tab(2) + "{");
					//sw.WriteLine(U.Tab(3) + U.Escape("name") + ":" + (m_tiles[y, x] == null ? U.Escape("Empty") : U.Escape(m_tiles[y, x].name)) + ",");
					sw.WriteLine(U.Tab(3) + U.Escape("name") + ":" + U.Escape(m_tiles[y, x].name) + ",");
					sw.WriteLine(U.Tab(3) + U.Escape("position") + ":{");
					sw.WriteLine(U.Tab(4) + U.Escape("x") + ":" + x + ",");
					sw.WriteLine(U.Tab(4) + U.Escape("y") + ":" + y);
					sw.WriteLine(U.Tab(3) + "}");
					sw.WriteLine(U.Tab(2) + "}" + ((y == m_floorHeight - 1 && x == m_floorWidth - 1) ? "" : ","));
				}
			}
			
			sw.WriteLine("\t]");
			sw.WriteLine("}");
		}
	}
	
	private void UpdateLoadManager()
	{
		if(Input.GetKeyUp(KeyCode.L))
		{
			if(File.Exists("floor.json"))
			{
				string floorDataJSON = File.ReadAllText("floor.json");
				
				ClearTiles();
				ClearExteriorWalls();
				
				Dictionary<string, object> floorData = Json.Deserialize(floorDataJSON) as Dictionary<string, object>;
				Dictionary<string, object> floorSizeDict = floorData["size"] as Dictionary<string, object>;
				
				m_floorWidth = (int)( (long)floorSizeDict["width"] );
				m_floorHeight = (int)( (long)floorSizeDict["height"] );
				
				InitializeTiles();
				
				List<object> tilesData = floorData["tiles"] as List<object>;
				
				if(tilesData.Count != m_floorWidth * m_floorHeight)
					return;
				
				Dictionary<string, object> tileDict = null;
				string tileName = string.Empty;
				Dictionary<string, object> tilePositionDict = null;
				SpriteRenderer spriteRendererTemp = null;
				
				int tileX = 0;
				int tileY = 0;
				
				for(int i = 0; i < tilesData.Count; i++)
				{
					tileDict = tilesData[i] as Dictionary<string, object>;
					tileName = tileDict["name"] as string;
					
					if(tileName != "Empty")
					{
						tilePositionDict = tileDict["position"] as Dictionary<string, object>;
						
						tileX = (int)( (long)tilePositionDict["x"] );
						tileY = (int)( (long)tilePositionDict["y"] );
						
						m_tiles[tileY, tileX] = Resources.Load("Prefabs/Tiles/Floor/" + tileName) as GameObject;
						m_tiles[tileY, tileX] = Instantiate(m_tiles[tileY, tileX]) as GameObject;
						m_tiles[tileY, tileX].name = tileName;
						
						spriteRendererTemp = m_tiles[tileY, tileX].GetComponent<SpriteRenderer>();
						
						if(spriteRendererTemp != null)
							m_tiles[tileY, tileX].transform.position = new Vector3(spriteRendererTemp.bounds.size.x * tileX + spriteRendererTemp.bounds.size.x / 2f, spriteRendererTemp.bounds.size.y * tileY + spriteRendererTemp.bounds.size.y / 2f, 0f);
					}
				}
			}
		}
	}
	
	public void OnPostRender()
	{
		DrawGrid();
		DrawSelectedObject();
	}

	private void DrawGrid()
	{
		if(!m_showGrid)
			return;
		
		if(m_gridMaterial == null)
		{
			m_gridMaterial = new Material("Shader \"Lines/Colored Blended\" { " +
			                              "SubShader { Pass { " +
			                              "Blend SrcAlpha OneMinusSrcAlpha " +
			                              "ZWrite Off Cull Off Fog { Mode Off } " +
			                              "BindChannels { Bind \"vertex\", vertex Bind \"color\", color } " +
			                              "} } }");
			
			m_gridMaterial.hideFlags = HideFlags.HideAndDontSave;
			m_gridMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
		
		m_gridMaterial.SetPass(0);
		
		GL.Begin(GL.LINES);
		
		if(m_showGrid)
		{
			GL.Color(m_gridColor);
			
			for(int y = 0; y <= m_floorHeight; y++)
			{
				for(int x = 0; x <= m_floorWidth; x++)
				{
					GL.Vertex3(0f, m_tileRendererY * y, 0f);
					GL.Vertex3(m_tileRendererX * x, m_tileRendererY * y, 0f);
				}
			}
			
			for(int y = 0; y <= m_floorHeight; y++)
			{
				for(int x = 0; x <= m_floorWidth; x++)
				{
					GL.Vertex3(m_tileRendererX * x, 0f, 0f);
					GL.Vertex3(m_tileRendererX * x, m_tileRendererY * y, 0f);
				}
			}
		}
		
		GL.End();
	}

	private void DrawSelectedObject()
	{
		if(s_selectedObjectRenderer == null)
			return;
		
		Material selectionMaterial = new Material("Shader \"Lines/Colored Blended\" { " +
		                                          "SubShader { Pass { " +
		                                          "Blend SrcAlpha OneMinusSrcAlpha " +
		                                          "ZWrite Off Cull Off Fog { Mode Off } " +
		                                          "BindChannels { Bind \"vertex\", vertex Bind \"color\", color } " +
		                                          "} } }");
		
		selectionMaterial.hideFlags = HideFlags.HideAndDontSave;
		selectionMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		
		selectionMaterial.SetPass(0);
		
		GL.Begin(GL.LINES);
		GL.Color(new Color(0.11f, 0.7f, 0.85f)); // Baby Blue
		
		// Top Line
		GL.Vertex3(s_selectedObjectRenderer.bounds.min.x, s_selectedObjectRenderer.bounds.max.y, 0f);
		GL.Vertex3(s_selectedObjectRenderer.bounds.max.x, s_selectedObjectRenderer.bounds.max.y, 0f);
		
		// Right Line
		GL.Vertex3(s_selectedObjectRenderer.bounds.max.x, s_selectedObjectRenderer.bounds.max.y, 0f);
		GL.Vertex3(s_selectedObjectRenderer.bounds.max.x, s_selectedObjectRenderer.bounds.min.y, 0f);
		
		// Bottom Line (heh)
		GL.Vertex3(s_selectedObjectRenderer.bounds.max.x, s_selectedObjectRenderer.bounds.min.y, 0f);
		GL.Vertex3(s_selectedObjectRenderer.bounds.min.x, s_selectedObjectRenderer.bounds.min.y, 0f);
		
		// Left Line
		GL.Vertex3(s_selectedObjectRenderer.bounds.min.x, s_selectedObjectRenderer.bounds.min.y, 0f);
		GL.Vertex3(s_selectedObjectRenderer.bounds.min.x, s_selectedObjectRenderer.bounds.max.y, 0f);
		
		GL.End();
	}
	
	private void UpdatePainter()
	{
		if(m_tile.activeInHierarchy)
			m_tile.SetActive(false);
		
		if(m_tileRenderer == null || m_isLerping)
			return;
		
		m_mouseLogicalX = Mathf.FloorToInt(m_mouseWorldPosition.x / m_tileRendererX);
		m_mouseLogicalY = Mathf.FloorToInt(m_mouseWorldPosition.y / m_tileRendererY);
		
		m_tileX = m_mouseLogicalX * m_tileRendererX;
		m_tileY = m_mouseLogicalY * m_tileRendererY;
		
		if((m_mouseLogicalX < 0 || m_mouseLogicalX > m_floorWidth - 1) || (m_mouseLogicalY < 0 || m_mouseLogicalY > m_floorHeight - 1))
			return;
		
		m_tile.transform.position = new Vector3(m_tileX + m_tileRendererX / 2f, m_tileY + m_tileRendererY / 2f, 0f);
		m_tile.SetActive(true);
		
		if(Input.GetMouseButtonUp((int)MouseButton.Left))
		{
			if(m_tiles[m_mouseLogicalY, m_mouseLogicalX] == null)
			{
				GameObject newTile = Resources.Load("Prefabs/Tiles/Floor/FloorLight") as GameObject;
				
				if(newTile != null)
				{
					newTile = Instantiate(newTile) as GameObject;
					
					if(newTile != null)
					{
						newTile.transform.position = m_tile.transform.position;
						newTile.name = "FloorLight";
						
						m_tiles[m_mouseLogicalY, m_mouseLogicalX] = newTile;
					}
				}
			}
			else
				Destroy(m_tiles[m_mouseLogicalY, m_mouseLogicalX]);
		}
	}
	
	private void UpdateMiscKeyboardCommands()
	{
		if(Input.GetKeyUp(KeyCode.C))
		{
			ClearTiles();
			InitializeTiles();
		}
		
		if(Input.GetKeyUp(KeyCode.A))
		{
			if(m_incrementAxis == Axis.X)
				m_incrementAxis = Axis.Y;
			else if(m_incrementAxis == Axis.Y)
				m_incrementAxis = Axis.Both;
			else if(m_incrementAxis == Axis.Both)
				m_incrementAxis = Axis.X;
			
			PrintStatusMessage("Increment axis set to: " + (m_incrementAxis == Axis.X ? "X" : (m_incrementAxis == Axis.Y ? "Y" : "Both")));
		}
		
		if(Input.GetKeyUp(KeyCode.KeypadPlus))
		{
			bool incrementX = (m_incrementAxis == Axis.X || m_incrementAxis == Axis.Both) && m_floorWidth < MAX_FLOOR_WIDTH;
			bool incrementY = (m_incrementAxis == Axis.Y || m_incrementAxis == Axis.Both) && m_floorHeight < MAX_FLOOR_HEIGHT;
			
			if(!(incrementX || incrementY))
				return;
			
			ClearTiles();
			ClearExteriorWalls();
			
			if(incrementX)
				m_floorWidth += FLOOR_INCREMENT_STEP;
			
			if(incrementY)
				m_floorHeight += FLOOR_INCREMENT_STEP;
			
			InitializeTiles();
		}
		
		if(Input.GetKeyUp(KeyCode.KeypadMinus))
		{
			bool decrementX = (m_incrementAxis == Axis.X || m_incrementAxis == Axis.Both) && m_floorWidth > MIN_FLOOR_WIDTH;
			bool decrementY = (m_incrementAxis == Axis.Y || m_incrementAxis == Axis.Both) && m_floorHeight > MIN_FLOOR_HEIGHT;
			
			ClearTiles();
			ClearExteriorWalls();
			
			if(decrementX)
				m_floorWidth -= FLOOR_INCREMENT_STEP;
			
			if(decrementY)
				m_floorHeight -= FLOOR_INCREMENT_STEP;
			
			InitializeTiles();
		}
	}
	
	private void InitializeTiles()
	{
		bool initializeWalls = false;
		
		m_tiles = new GameObject[m_floorHeight, m_floorWidth];
		
		if(m_exteriorWalls == null)
		{
			initializeWalls = true;
			
			m_exteriorWalls = new GameObject[m_floorHeight / 2 + 2, m_floorWidth / 2 + 2];
		}
		
		GameObject defaultTile = Resources.Load("Prefabs/Tiles/Floor/FloorLight") as GameObject;
		
		for(int y = 0; y < m_floorHeight; y++)
		{
			for(int x = 0; x < m_floorWidth; x++)
			{
				m_tiles[y, x] = null;
				
				if(defaultTile != null)
				{
					defaultTile = Instantiate(defaultTile) as GameObject;
					
					if(defaultTile != null)
					{
						defaultTile.transform.position = new Vector3(x * m_tileRendererX + m_tileRendererX / 2f, y * m_tileRendererY + m_tileRendererY / 2f, 0f);
						defaultTile.name = "FloorLight";
						
						m_tiles[y, x] = defaultTile;
					}
				}
				
				if(!initializeWalls)
					continue;
				
				if(x == 0)
				{
					if(y == 0)
					{
						GameObject bottomLeftCorner = Resources.Load("Prefabs/Tiles/ExteriorWalls/200/BottomLeftCorner") as GameObject;
						bottomLeftCorner = Instantiate(bottomLeftCorner) as GameObject;
						
						SpriteRenderer wallRenderer = bottomLeftCorner.GetComponent<SpriteRenderer>();
						Vector3 wallPosition = Vector3.zero;
						
						wallPosition.x = -wallRenderer.bounds.size.x / 2;
						wallPosition.y = -wallRenderer.bounds.size.y / 2;
						
						bottomLeftCorner.transform.position = wallPosition;
						bottomLeftCorner.name = "BottomLeftCorner";
						
						m_exteriorWalls[y, x] = bottomLeftCorner;
					}
					
					if(y % 2 == 0)
					{
						GameObject verticalWall = Resources.Load("Prefabs/Tiles/ExteriorWalls/200/VerticalStraight") as GameObject;
						verticalWall = Instantiate(verticalWall) as GameObject;
						
						SpriteRenderer wallRenderer = verticalWall.GetComponent<SpriteRenderer>();
						Vector3 wallPosition = Vector3.zero;
						
						wallPosition.x = -wallRenderer.bounds.size.x / 2;
						wallPosition.y = wallRenderer.bounds.size.y * (y / 2) + wallRenderer.bounds.size.y / 2;
						
						verticalWall.transform.position = wallPosition;
						verticalWall.name = "VerticalStraight";
						
						m_exteriorWalls[y / 2 + 1, x] = verticalWall;
					}
				}
				
				if(y == 0)
				{
					if(x == m_floorWidth - 1)
					{
						GameObject bottomRightCorner = Resources.Load("Prefabs/Tiles/ExteriorWalls/200/BottomRightCorner") as GameObject;
						bottomRightCorner = Instantiate(bottomRightCorner) as GameObject;
						
						SpriteRenderer wallRenderer = bottomRightCorner.GetComponent<SpriteRenderer>();
						Vector3 wallPosition = Vector3.zero;
						
						wallPosition.x = wallRenderer.bounds.size.x / 2 + wallRenderer.bounds.size.x * ((x + 1) / 2);
						wallPosition.y = -wallRenderer.bounds.size.y / 2;
						
						bottomRightCorner.transform.position = wallPosition;
						bottomRightCorner.name = "BottomRightCorner";
						
						m_exteriorWalls[y, m_floorWidth / 2 + 1] = bottomRightCorner;
					}
					
					if(x % 2 == 0)
					{
						GameObject horizontalWall = Resources.Load("Prefabs/Tiles/ExteriorWalls/200/HorizontalStraight") as GameObject;
						horizontalWall = Instantiate(horizontalWall) as GameObject;
						
						SpriteRenderer wallRenderer = horizontalWall.GetComponent<SpriteRenderer>();
						Vector3 wallPosition = Vector3.zero;
						
						wallPosition.x = wallRenderer.bounds.size.x * (x / 2) + wallRenderer.bounds.size.x / 2;
						wallPosition.y = -wallRenderer.bounds.size.y / 2;
						
						horizontalWall.transform.position = wallPosition;
						horizontalWall.name = "HorizontalStraight";
						
						m_exteriorWalls[y, x / 2 + 1] = horizontalWall;
					}
				}
				
				if(x == m_floorWidth - 1)
				{
					if(y == m_floorHeight - 1)
					{
						GameObject topRightCorner = Resources.Load("Prefabs/Tiles/ExteriorWalls/200/TopRightCorner") as GameObject;
						topRightCorner = Instantiate(topRightCorner) as GameObject;
						
						SpriteRenderer wallRenderer = topRightCorner.GetComponent<SpriteRenderer>();
						Vector3 wallPosition = Vector3.zero;
						
						wallPosition.x = wallRenderer.bounds.size.x / 2 + wallRenderer.bounds.size.x * ((x + 1) / 2);
						wallPosition.y = wallRenderer.bounds.size.y * ((y + 1) / 2) + wallRenderer.bounds.size.y / 2;
						
						topRightCorner.transform.position = wallPosition;
						topRightCorner.name = "TopRightCorner";
						
						m_exteriorWalls[m_floorHeight / 2 + 1, m_floorWidth / 2 + 1] = topRightCorner;
					}
					
					if(y % 2 == 0)
					{
						GameObject verticalWall = Resources.Load("Prefabs/Tiles/ExteriorWalls/200/VerticalStraight") as GameObject;
						verticalWall = Instantiate(verticalWall) as GameObject;
						
						SpriteRenderer wallRenderer = verticalWall.GetComponent<SpriteRenderer>();
						Vector3 wallPosition = Vector3.zero;
						
						wallPosition.x = wallRenderer.bounds.size.x / 2 + wallRenderer.bounds.size.x * ((x + 1) / 2);
						wallPosition.y = wallRenderer.bounds.size.y * (y / 2) + wallRenderer.bounds.size.y / 2;
						
						verticalWall.transform.position = wallPosition;
						verticalWall.name = "VerticalStraight";
						
						m_exteriorWalls[y / 2 + 1, m_floorWidth / 2 + 1] = verticalWall;
					}
				}
				
				if(y == m_floorHeight - 1)
				{
					if(x == 0)
					{
						GameObject topLeftCorner = Resources.Load("Prefabs/Tiles/ExteriorWalls/200/TopLeftCorner") as GameObject;
						topLeftCorner = Instantiate(topLeftCorner) as GameObject;
						
						SpriteRenderer wallRenderer = topLeftCorner.GetComponent<SpriteRenderer>();
						Vector3 wallPosition = Vector3.zero;
						
						wallPosition.x = -wallRenderer.bounds.size.x / 2;
						wallPosition.y = wallRenderer.bounds.size.y * ((y + 1) / 2) + wallRenderer.bounds.size.y / 2;
						
						topLeftCorner.transform.position = wallPosition;
						topLeftCorner.name = "TopLeftCorner";
						
						m_exteriorWalls[m_floorHeight / 2 + 1, x] = topLeftCorner;
					}
					
					if(x % 2 == 0)
					{
						GameObject horizontalWall = Resources.Load("Prefabs/Tiles/ExteriorWalls/200/HorizontalStraight") as GameObject;
						horizontalWall = Instantiate(horizontalWall) as GameObject;
						
						SpriteRenderer wallRenderer = horizontalWall.GetComponent<SpriteRenderer>();
						Vector3 wallPosition = Vector3.zero;
						
						wallPosition.x = wallRenderer.bounds.size.x * (x / 2) + wallRenderer.bounds.size.x / 2;
						wallPosition.y = wallRenderer.bounds.size.y / 2 + wallRenderer.bounds.size.y * ((y + 1) / 2);
						
						horizontalWall.transform.position = wallPosition;
						horizontalWall.name = "HorizontalStraight";
						
						m_exteriorWalls[m_floorHeight / 2 + 1, x / 2 + 1] = horizontalWall;
					}
				}
			}
		}
	}
	
	private void ClearExteriorWalls()
	{
		if(m_exteriorWalls == null)
			return;
		
		for(int y = 0; y < m_floorHeight / 2 + 2; y++)
		{
			for(int x = 0; x < m_floorWidth / 2 + 2; x++)
			{
				Destroy(m_exteriorWalls[y, x]);
				m_exteriorWalls[y, x] = null;
			}
		}
		
		m_exteriorWalls = null;
	}
	
	private void ClearTiles()
	{
		if(m_tiles == null)
			return;
		
		for(int y = 0; y < m_floorHeight; y++)
		{
			for(int x = 0; x < m_floorWidth; x++)
			{
				Destroy(m_tiles[y, x]);
				m_tiles[y, x] = null;
			}
		}
		
		m_tiles = null;
	}
	
	private void InitializeTile()
	{
		m_tile = GameObject.Find("FloorLight");
		
		GameObject tileSize = Resources.Load("Prefabs/Tiles/Floor/FloorLight") as GameObject;
		
		if(tileSize != null)
		{
			m_tileRenderer = tileSize.GetComponent<SpriteRenderer>();
			
			if(m_tileRenderer != null)
			{
				m_tileRendererX = m_tileRenderer.bounds.size.x;
				m_tileRendererY = m_tileRenderer.bounds.size.y;
			}
		}
	}
	
	private void PrintStatusMessage(string message)
	{
		GameObject statusTextGameObject = GameObject.Find("StatusText");
		GUIText statusText = null;
		
		if(statusTextGameObject != null)
		{
			statusText = statusTextGameObject.GetComponent<GUIText>();
			
			if(statusText != null)
				statusText.text = "Status: " + message;
		}
	}
	
	/*
	public void OnGUI()
	{
		int.TryParse(GUI.TextField(new Rect(10, 10, 40, 20), m_floorWidthTemp.ToString(), 25), out m_floorWidthTemp);
		int.TryParse(GUI.TextField(new Rect(10, 35, 40, 20), m_floorHeightTemp.ToString(), 25), out m_floorHeightTemp);

		if(m_floorWidthTemp <= 0)
			m_floorWidthTemp = m_floorWidth;

		if(m_floorHeightTemp <= 0)
			m_floorHeightTemp = m_floorHeight;

		m_floorWidth = m_floorWidthTemp;
		m_floorHeight = m_floorHeightTemp;
	}
	*/
}