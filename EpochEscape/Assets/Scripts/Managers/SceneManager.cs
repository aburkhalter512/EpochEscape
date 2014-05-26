using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
	#region Inspector Variables
	public Vector2 tileSize = new Vector2(1.0f, 1.0f);
	public LevelManager[] levels = null;
	public GameObject transitionScreen = null;
	#endregion

	#region Instance Variables
	//bool isLoadingLevel = false;

	LevelManager currentLevel = null;
	LevelManager nextLevel = null;
	#endregion

	#region Class Constants
	public const int DEFAULT_LEVEL_WIDTH = 100;
	public const int DEFAULT_LEVEL_HEIGHT = 100;
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
		DontDestroyOnLoad(this.gameObject);
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
		if (nextLevel != null)
		{
			if (!Application.isLoadingLevel) //Means finished loading level
			{
				if (currentLevel != null)
					hideTransitionScreen();

				currentLevel = nextLevel;
				nextLevel = null;
			}
		}//*/
	}

	public static void Load(string name)
	{
		GameObject go = new GameObject("SceneManager");
		SceneManager instance = go.AddComponent<SceneManager>();
		instance.StartCoroutine(instance.InnerLoad(name));

		//Debug.Log (GameManager.getInstance ().m_currentCharacter);
	}
	
	IEnumerator InnerLoad(string name)
	{
		Object.DontDestroyOnLoad(this.gameObject);
		FadeManager.StartAlphaFade (Color.black, false, 2f, 0f, ()=> {Application.LoadLevel("Loading");});
		
		yield return new WaitForSeconds(2.5f);
		
		FadeManager.StartAlphaFade (Color.black, false, 2f, 0f, ()=> {Application.LoadLevel (name);});
		Destroy(this.gameObject);
	}

	#region Update Methods
	public void transitionToLevel(string toLevel)
	{
		if (Application.isLoadingLevel)
			return;

		nextLevel = findLevel(toLevel);
		if (nextLevel == null)
			return;

		if (currentLevel != null)
			showTransitionScreen();
	
		nextLevel.loadLevel();
	}

	public void showTransitionScreen()
	{
		if (transitionScreen == null)
			return;

		Vector3 showPosition = Camera.main.transform.position;
		showPosition.z = 0.0f;
		transitionScreen.transform.position = showPosition;
	}

	public void hideTransitionScreen()
	{
		if (transitionScreen == null)
			return;

		Vector3 hidePosition = Camera.main.transform.position;
		hidePosition.z *= 2;
		transitionScreen.transform.position = hidePosition;
	}

	private LevelManager findLevel(string target)
	{
		LevelManager retVal = null;

		foreach (System.Object level in levels)
		{
			if (((LevelManager)level).levelName == target)
			{
				retVal = (LevelManager)level;
				break;
			}
		}

		return retVal;
	}
	#endregion
	//*/

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
