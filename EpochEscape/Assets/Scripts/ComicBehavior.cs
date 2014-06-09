using UnityEngine;
using System.Collections;

public class ComicBehavior : MonoBehaviour {
	public int MAX_PAGES = 8;
	public GUITexture currentComic;
	public GUISkin EpochSkin;
	private int page = 0;
	public string comicName = "Epoch comic";
	public Texture2D[] comics;

	// Use this for initialization
	void Start () {
		comics = new Texture2D[MAX_PAGES];
		if(comics[0] == null)
			for(int i = 0; i < MAX_PAGES; i++)
				comics[i] = Resources.Load ("Textures/Comic/" + comicName + (i + 1)) as Texture2D;
	}
	
	// Update is called once per frame
	void Update () {
		GetKeyPresses ();
		currentComic.texture = comics[page];
	}

	void OnGUI(){
		ShowNext ();
		ShowPrevious ();
	}

	void GetKeyPresses(){
		if (Input.anyKeyDown && !Input.GetKeyDown (KeyCode.Escape) && !Input.GetMouseButtonDown(0)) {
			if(page == (MAX_PAGES - 1))
				FadeManager.StartAlphaFade(Color.black, false, 2f, 0f, () => { Application.LoadLevel("Level1"); });
			else
				page++;
		}
		else if(Input.GetKeyDown (KeyCode.Escape))
			FadeManager.StartAlphaFade(Color.black, false, 2f, 0f, () => { Application.LoadLevel("Level1"); });
	}

	void ShowNext(){
		GUILayout.BeginArea (new Rect (Screen.width - 175, Screen.height - 75, 110, 50));
		if (GUILayout.Button ("Skip", EpochSkin.GetStyle("Small Button"))) {
			FadeManager.StartAlphaFade(Color.black, false, 2f, 0f, () => { Application.LoadLevel("Level1"); });
		}
		GUILayout.EndArea ();
		GUILayout.BeginArea (new Rect (Screen.width - 50, Screen.height - 75, 110, 50));
		if (GUILayout.Button ("", EpochSkin.GetStyle("Arrow Right"))) {
			if(page == (MAX_PAGES - 1))
				FadeManager.StartAlphaFade(Color.black, false, 2f, 0f, () => { Application.LoadLevel("Level1"); });
			else
				page++;
		}
		GUILayout.EndArea ();
	}
	void ShowPrevious(){
		GUILayout.BeginArea (new Rect (0, Screen.height - 75, 110, 50));
		if (GUILayout.Button ("", EpochSkin.GetStyle("Arrow Left"))) {
			if(page != 0){
				page--;
			}
		}
		GUILayout.EndArea ();
	}
}
