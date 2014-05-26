using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
	#region Inspector Variables
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

	public static void Load(string name)
	{
		GameObject go = new GameObject("SceneManager");
		SceneManager instance = go.AddComponent<SceneManager>();
		instance.StartCoroutine(instance.InnerLoad(name));

		Debug.Log (GameManager.getInstance ().m_currentCharacter);
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
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
