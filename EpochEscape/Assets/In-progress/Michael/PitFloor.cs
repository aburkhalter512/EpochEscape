using UnityEngine;
using System.Collections;

public class PitFloor : InteractiveObject {
	public Sprite fill;

    #region Instance Variables
    Player mPlayer = null;

    bool mCanInteract = false;
    #endregion

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (mCanInteract && InputManager.getInstance().interactButton.getDown())
            Interact();
            
        if (mPlayer == null)
        	FindPlayer();
	}
	/*
    public void OnTriggerEnter2D(Collider2D collidee)
    {
        mPlayer = collidee.GetComponent<Player>();

        if (mPlayer != null)
            mCanInteract = true;
    }

    public void OnTriggerExit2D(Collider2D collidee)
    {
        if (mPlayer == collidee.GetComponent<Player>())
        {
            mCanInteract = false;
            mPlayer = null;
        }
    } */
	
	public override void Interact()
    {
		if (mPlayer.m_isHoldingBox)
        {
            HoldableBox box = mPlayer.GetComponentInChildren<HoldableBox>();
			SpriteRenderer s = gameObject.GetComponent<SpriteRenderer>();
			s.sprite = fill;
			box.Die ();
            mPlayer.m_isHoldingBox = false;
			gameObject.GetComponent<Collider2D>().enabled = false;
		}
	}
	
	private void FindPlayer()
	{
		if(mPlayer == null)
		{
			GameObject player = GameObject.FindWithTag("Player");
			
			if(player != null)
				mPlayer = player.GetComponent<Player>();
		}
	}
}
