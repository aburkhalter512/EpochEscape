using UnityEngine;
using System.Collections;
using G = GameManager;

public class LevelManager : MonoBehaviour
{
	#region Inspector Variables
	public enum Character
	{
		CAVE_GIRL,
		KNIGHT
	}

    public string levelName = null;
    public int levelWidth = SceneManager.DEFAULT_LEVEL_WIDTH;
    public int levelHeight = SceneManager.DEFAULT_LEVEL_HEIGHT;
	public int coresToFind = 3;
	public int coresFound;

	private GameObject m_player;
	private GameObject m_playerSpecialItem;
	#endregion

	#region Instance Variables
    ArrayList tiles = null;
	#endregion

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
        //Debug.Log(G.getInstance().m_currentCharacter);

		coresFound = 0;

		if(G.getInstance().currentLevel == 1)
			InstantiateAllPlayers();
		else
			InstantiateCurrentPlayer();

		InstantiateSpecialItems();

		GameObject hudManager = new GameObject();
		hudManager.AddComponent<HUDManager>();
		hudManager.name = "HUDManager";
	}

	private void InstantiateAllPlayers()
	{
		// Lasers
		GameObject brokenLaser = Resources.Load("Prefabs/Obstacles/JailBarBrokenLaser") as GameObject;
		GameObject staticLaser = Resources.Load("Prefabs/Obstacles/JailBarLaser") as GameObject;

		Player playerScript = null;

		// Cave Girl
		GameObject caveGirl = Resources.Load("Prefabs/Players/CaveGirl") as GameObject;
		GameObject caveGirlSpawn = GameObject.FindWithTag("CaveGirlSpawn");
		GameObject caveGirlLaserSpawn = GameObject.FindWithTag("CaveGirlLaserSpawn");

		if(caveGirl != null && caveGirlSpawn != null)
		{
			caveGirl = Instantiate(caveGirl) as GameObject;

			playerScript = caveGirl.GetComponent<Player>();
			
			if(G.getInstance().m_currentCharacter == G.CAVEGIRL)
			{
				// Set the player spawn point
				caveGirl.tag = "Player";
				
				if(playerScript != null)
					playerScript.m_spawnLocation = caveGirlSpawn.transform.position;

				// Create the broken laser
				if(caveGirlLaserSpawn != null)
				{
					brokenLaser = Instantiate(brokenLaser) as GameObject;
					brokenLaser.transform.position = caveGirlLaserSpawn.transform.position;
				}
			}
			else
			{
				// Set the player up as a dummy
				caveGirl.tag = "Dummy";

				if(playerScript != null)
					playerScript.enabled = false;

				// Create the static laser
				if(caveGirlLaserSpawn != null)
				{
					staticLaser = Instantiate(staticLaser) as GameObject;
					staticLaser.transform.position = caveGirlLaserSpawn.transform.position;
				}
			}

			caveGirl.transform.position = caveGirlSpawn.transform.position;
		}

		// Knight
		GameObject knight = Resources.Load("Prefabs/Players/Knight") as GameObject;
		GameObject knightSpawn = GameObject.FindWithTag("KnightSpawn");
		GameObject knightLaserSpawn = GameObject.FindWithTag("KnightLaserSpawn");
		
		if(knight != null && knightSpawn != null)
		{
			knight = Instantiate(knight) as GameObject;
			
			playerScript = knight.GetComponent<Player>();
			
			if(G.getInstance().m_currentCharacter == G.KNIGHT)
			{
				// Set the player spawn point
				knight.tag = "Player";
				
				if(playerScript != null)
					playerScript.m_spawnLocation = knightSpawn.transform.position;
				
				// Create the broken laser
				if(knightLaserSpawn != null)
				{
					brokenLaser = Instantiate(brokenLaser) as GameObject;
					brokenLaser.transform.position = knightLaserSpawn.transform.position;
				}
			}
			else
			{
				// Set the player up as a dummy
				knight.tag = "Dummy";
				
				if(playerScript != null)
					playerScript.enabled = false;
				
				// Create the static laser
				if(knightLaserSpawn != null)
				{
					staticLaser = Instantiate(staticLaser) as GameObject;
					staticLaser.transform.position = knightLaserSpawn.transform.position;
				}
			}
			
			knight.transform.position = knightSpawn.transform.position;
		}
	}

	private void InstantiateCurrentPlayer()
	{
		// Create the character.
		switch(G.getInstance().m_currentCharacter)
		{
		case G.KNIGHT:
			m_player = Resources.Load("Prefabs/Players/Knight") as GameObject;
			break;
			
		case G.CAVEGIRL:
		default:
			m_player = Resources.Load("Prefabs/Players/CaveGirl") as GameObject;
			break;
		}
		
		if(m_player != null)
		{
			Player playerScript = m_player.GetComponent<Player>();
			GameObject spawnLocation = GameObject.FindGameObjectWithTag("SpawnLocation");
			
			if(playerScript != null && spawnLocation != null)
			{
				playerScript.m_spawnLocation = spawnLocation.transform.position;
				m_player.transform.position = spawnLocation.transform.position;
				
				Instantiate(m_player);
			}
		}
	}

	private void InstantiateSpecialItems()
	{
		switch(G.getInstance().m_currentCharacter)
		{
		case G.KNIGHT:
			m_playerSpecialItem = Resources.Load("Prefabs/Items/SpecialItems/Shield") as GameObject;
			break;
			
		case G.CAVEGIRL:
		default:
			m_playerSpecialItem = Resources.Load("Prefabs/Items/SpecialItems/Club") as GameObject;
			break;
		}

		if(m_playerSpecialItem != null)
		{
			GameObject[] specialItemSpawnLocations = GameObject.FindGameObjectsWithTag("SpecialItemSpawn");
			
			for(int i = 0; i < specialItemSpawnLocations.Length; i++)
			{
				GameObject newSpecialItem = Instantiate(m_playerSpecialItem) as GameObject;
				
				newSpecialItem.transform.position = specialItemSpawnLocations[i].transform.position;
			}
		}
	}

    #region Initialization Methods
    #endregion

    //Put all update code here
    //Remember to comment!
    protected void Update()
	{
	}

	#region Update Methods
    public void loadLevel()
    {
        if (levelName != null)
            Application.LoadLevel(levelName);
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
