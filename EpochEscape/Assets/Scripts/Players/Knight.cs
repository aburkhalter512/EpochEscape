using UnityEngine;
using System.Collections;

public class Knight : Player
{
    #region Inspector Variables
    #endregion

    #region Instance Variables
    //REMOVE WHEN FINISHED REFACTORING
    /*public bool m_isShieldActive = false;
    public float m_shieldDuration = 3f; // seconds
    public float m_shieldTime = 0f;*/
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
        base.Update();

        //UpdateShield();
    }

    #region Update Methods
    /*private void UpdateShield()
    {
        if (!m_isShieldActive) return;

        if (Time.time - m_shieldTime > m_shieldDuration)
        {
            m_isShieldActive = false;

            if (inventory.inventory[Inventory.SPECIAL_SLOT] == null)
                m_hasSpecialItem = false;
        }
    }//*/
    #endregion

    #region Static Methods
    #endregion

    #region Utilities
    #endregion
}
