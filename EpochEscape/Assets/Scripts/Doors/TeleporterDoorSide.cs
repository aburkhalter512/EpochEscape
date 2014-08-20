using UnityEngine;
using System.Collections;

public class TeleporterDoorSide : DoorSide
{
    #region Interface Variables
    public Sprite openSprite;
    public Sprite closeSprite;

    public GameObject teleportDestination;
    #endregion

    #region Instance Variables
    protected BoxCollider2D mCollider;

    protected TeleporterDoorSide mTeleportDestination;
    protected GameObject mTeleportSpawn;

    bool mIsOpened;
    bool mCanTeleport;
	#endregion

    #region Class Constants
    public static string OPEN_SPRITE = "Textures/Doors/DoorSides/Teleporter";
    public static string CLOSE_SPRITE = "Textures/Doors/DoorSides/Locked";
    #endregion

    //Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
        base.Start();

        if (openSprite == null)
            openSprite = Resources.Load<Sprite>(OPEN_SPRITE);

        if (closeSprite == null)
            closeSprite = Resources.Load<Sprite>(CLOSE_SPRITE);

        mCollider = GetComponent<BoxCollider2D>();
        mCollider.isTrigger = true;

        if (teleportDestination != null)
            mTeleportDestination = teleportDestination.GetComponent<TeleporterDoorSide>();
        mTeleportSpawn = new GameObject();
        mTeleportSpawn.transform.parent = transform;
        mTeleportSpawn.transform.localPosition = new Vector3(0.0f, -.5f, 0.0f);

        mSR.sprite = openSprite;
        mIsOpened = true;
        mCanTeleport = true;
	}

    #region Update Methods
    protected void teleport(Player player)
    {
        if (player == null || mTeleportDestination == null)
            return;

        Vector3 v = mTeleportDestination.mTeleportSpawn.transform.position;
        player.transform.position = v;
        //mTeleportDestination.mCanTeleport = false;
    }
    #endregion

    #region Interface Methods
    public override void init()
    {
        return;
    }

    public override void open()
    {
        if (!mIsOpened)
        {
            mSR.sprite = openSprite;
            mIsOpened = true;

            mCollider.isTrigger = true;
        }
    }

    public override void close()
    {
        if (mIsOpened)
        {
            mSR.sprite = closeSprite;
            mIsOpened = false;

            mCollider.isTrigger = false;
        }
    }

    public override void toggle()
    {
        if (!mIsOpened)
            open();
        else
            close();
    }

    public override void triggerFrontEnter()
    {
        if (mWasBackHit)
            mCanTeleport = false;

        mWasFrontHit = true;
    }

    public override void triggerFrontExit()
    {
        if (!mWasBackHit)
            mCanTeleport = true;

        mWasFrontHit = false;
    }

    public override void triggerBackEnter()
    {
        if (!mWasFrontHit)
            mCanTeleport = false;

        mWasBackHit = true;
    }

    public override void triggerBackExit()
    {
        if (mWasFrontHit)
            mCanTeleport = true;

        mWasBackHit = false;
    }

    public void OnTriggerEnter2D(Collider2D collidee)
    {
        if (collidee.tag == "Player" && mCanTeleport)
            teleport(collidee.GetComponent<Player>());
    }
    
    public void Activate() {
    	renderer.enabled = !renderer.enabled;
    	collider2D.enabled = !collider2D.enabled;
    }
	#endregion
}
