using UnityEngine;
using System.Collections;
using G = GameManager;

public class SpecialItemSpawn : MonoBehaviour
{
    #region Inspector Variables
    #endregion

    #region Instance Variables
    private GameObject mToPlace = null;
    private GameObject mToInstantiate = null;
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
                mToInstantiate = Resources.Load<GameObject>("Prefabs/Items/SpecialItems/Club");
                break;
            case G.KNIGHT:
                mToInstantiate = Resources.Load<GameObject>("Prefabs/Items/SpecialItems/Shield");
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

        mToPlace = Instantiate(mToInstantiate) as GameObject;

        if (mToPlace != null)
            mToPlace.transform.position = transform.position;

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
