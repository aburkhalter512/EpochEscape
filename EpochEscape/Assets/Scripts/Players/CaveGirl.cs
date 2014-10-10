using UnityEngine;
using System.Collections;

public class CaveGirl : Player
{
    #region Interface Variables
    #endregion

    #region Instance Variables
    #endregion

    #region Class Constants
    #endregion

    //Put all initialization code here
    //Remember to comment!
    protected void Start()
    {
        base.Start();
    }

    //Put all update code here
    //Remember to comment!
    protected void Update()
    {
    	base.Update();
    }
    
	#region Inteface Methods
	#endregion

    #region Instance Methods
    protected override void activateSpecialItem()
    {
    	m_isAttacking = true;
    }
    #endregion
}
