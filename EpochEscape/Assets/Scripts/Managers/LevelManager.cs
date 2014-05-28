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
        Debug.Log(G.getInstance().m_currentCharacter);

		coresFound = 0;

		// Create the character.
		switch(G.getInstance().m_currentCharacter)
		{
		/*case 1:
			m_player = Resources.Load("Prefabs/Players/Knight") as GameObject;
			break;*/

		case 0:
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
			}

			Instantiate(m_player);
		}

		GameObject hudManager = new GameObject();
		hudManager.AddComponent<HUDManager>();
		hudManager.name = "HUDManager";
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
