using UnityEngine;
using System.Collections;

public class LevelTransitioner : MonoBehaviour
{
	#region Inspector Variables
    public string Level = "";

    public bool isWinning = false;
	#endregion

	#region Instance Variables
	#endregion

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
	}

	#region Update Methods
    protected void OnTriggerEnter2D(Collider2D collidee)
    {
        if (collidee.tag == "Player"){
			if(Level == "Level1"){
				GameManager.getInstance ().currentLevel = 1;
				//GameManager.getInstance ().tutorial = false;
			}
			else
				GameManager.getInstance().currentLevel++;
            if (isWinning)
                SceneManager.Win(Level);
            else
                SceneManager.Load(Level);
			SaveManager.Save ();

		}
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
