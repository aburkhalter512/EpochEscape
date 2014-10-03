using UnityEngine;
using System.Collections;

public class Knight : Player
{
    //Put all initialization code here
    //Remember to comment!
    protected void Start()
    {
    	base.Start();
    	
    	inventory.setSpecialCooldown(7.0f);
    }

    //Put all update code here
    //Remember to comment!
    protected void Update()
    {
        base.Update();
    }

	#region Instance Methods
	protected override void activateSpecialItem()
	{
		m_isShieldActive = true;
		m_isAttacking = false;
		m_shieldTime = Time.time;
	}
    #endregion
}
