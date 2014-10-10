using UnityEngine;
using System.Collections;
using G = GameManager;

public class RedPotion : ActiveItem
{
    #region Class Constants
    public const float HEAL_AMOUNT = 25f;
    #endregion

    protected virtual void Start()
	{
		gameObject.tag = "Red Potion";
	}

    #region Interface Methods
    public override void PickUp(Player player)
	{
		PickUpSound.Play ();

        if (player.inventory.add(this))
        {
            mIsInventory = true;

            mSR.enabled = false;
        }

		return;
	}

	public override void Activate ()
	{
		ActivateSound.Play ();
		Player player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
		player.m_detectionLevel -= HEAL_AMOUNT;
		if (player.m_detectionLevel < 0)
			player.m_detectionLevel = 0;
		player.m_detectionMax -= HEAL_AMOUNT;
		if (player.m_detectionMax < 0)
			player.m_detectionMax = 0;
	}
    #endregion

    #region Instance Methods
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
            if (player.inventory.canAdd(this))
                base.OnTriggerEnter2D(other);
    }
    #endregion
}