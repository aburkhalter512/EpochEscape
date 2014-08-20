using UnityEngine;
using System.Collections;

public class SceneManager : UnitySingleton<SceneManager>
{
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

		//Debug.Log (GameManager.getInstance ().m_currentCharacter);
	}

    public static void Win(string name)
    {
        GameObject go = new GameObject("SceneManager");
        SceneManager instance = go.AddComponent<SceneManager>();
        instance.StartCoroutine(instance.InnerWin(name));

        //Debug.Log (GameManager.getInstance ().m_currentCharacter);
    }
	
	IEnumerator InnerWin(string name)
	{
		Object.DontDestroyOnLoad(this.gameObject);
		FadeManager.StartAlphaFade (Color.black, false, 2f, 0f, ()=> {Application.LoadLevel("Winning");});
		
		yield return new WaitForSeconds(15.0f);
		
		FadeManager.StartAlphaFade (Color.black, false, 2f, 0f, ()=> {Application.LoadLevel (name);});
		Destroy(this.gameObject);
	}

    IEnumerator InnerLoad(string name)
    {
        Object.DontDestroyOnLoad(this.gameObject);
        FadeManager.StartAlphaFade(Color.black, false, 2f, 0f, () => { Application.LoadLevel("Loading"); });

        yield return new WaitForSeconds(2.5f);

        FadeManager.StartAlphaFade(Color.black, false, 2f, 0f, () => { Application.LoadLevel(name); });
        Destroy(this.gameObject);
    }
}
