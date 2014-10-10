using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public abstract class Item : MonoBehaviour
{
    #region Interface Variables
    public AudioSource PickUpSound;
	public AudioSource ActivateSound;
    #endregion

    #region Instance Variables
    protected SpriteRenderer mSR;
    #endregion

    protected virtual void Awake()
    {
        mSR = GetComponent<SpriteRenderer>();

        if (mSR == null)
            Debug.Log("Fail");
    }

    #region Interface Methods
    public abstract void PickUp(Player player);	
    #endregion

    #region Instance Methods
    protected virtual void OnTriggerEnter2D(Collider2D other)
	{
        Player player = other.GetComponent<Player>();

		if(player != null)
            PickUp(player);
    }
    #endregion
}
