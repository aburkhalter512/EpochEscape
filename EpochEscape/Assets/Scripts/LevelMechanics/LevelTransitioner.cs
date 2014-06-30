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
    /*
     * A 2D collision why the player will trigger the transition between the
     * current level and the next level.
     */
    protected void OnTriggerEnter2D(Collider2D collidee)
    {
        if (collidee.tag == "Player")
        {
            //Set the correct current level
			if(Level == "Level1")
				GameManager.getInstance().currentLevel = 1;
			else
				GameManager.getInstance().currentLevel++;

            //Load the next level
            if (isWinning)
                SceneManager.Win(Level);
            else
                SceneManager.Load(Level);

            //Save all progress
			SaveManager.Save ();
		}
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
