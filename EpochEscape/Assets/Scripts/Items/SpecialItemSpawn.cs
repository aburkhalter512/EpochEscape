using UnityEngine;
using System.Collections;
using G = GameManager;

public class SpecialItemSpawn : MonoBehaviour
{
	#region Inspector Variables
	#endregion

	#region Instance Variables
	#endregion

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
        switch (G.getInstance().m_currentCharacter)
        {
            case G.CAVEGIRL:
                Club club = Resources.Load<Club>("Prefabs/Items/SpecialItems/Club");
                club.gameObject.transform.position = transform.position;
                break;
            case G.KNIGHT:
                Shield shield = Resources.Load<Shield>("Prefabs/Items/SpecialItems/Shield");
                shield.gameObject.transform.position = transform.position;
                break;
            case G.MUMMY:
                break;
            case G.NINJA:
                break;
            case G.ASTRONAUT:
                break;
            case G.ROBOT:
                break;
        }

        Destroy(gameObject);
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
	}

	#region Update Methods
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
