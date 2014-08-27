using UnityEngine;
using System.Collections;

public class TeleporterDoorSide : DoorSide
{
	#region Interface Variables
    public GameObject teleportDestination;
    public GameObject teleportSpawn;
	#endregion
	
	#region Instance Variables
    TeleporterDoorSide mTeleportDestination;

    bool mWasFrontHit = false;
    bool mWasBackHit = false;
    bool mCanTeleport = false;
	#endregion 
	
	protected void Awake()
	{
		base.Awake();
	}
	
	protected void Start()
	{
		base.Start();

        mTeleportDestination = teleportDestination.GetComponent<TeleporterDoorSide>();
	}
	
	protected void Update()
	{
		base.Update();
	}
	
	#region Interface Methods
    public override void triggerFrontEnter()
    {
        if (!mWasBackHit)
            mCanTeleport = true;

        mWasFrontHit = true;
    }
    public override void triggerFrontExit()
    {
        mCanTeleport = false;
        mWasFrontHit = false;
    }

    public override void triggerBackEnter()
    {
        mWasBackHit = true;
    }
    public override void triggerBackExit()
    {
        mWasBackHit = false;
    }

    public override void activate()
    {
        if (!isActive() || mIsFirstUpdate)
        {
            mSR.sprite = activeSprite;

            mIsActive = true;
            mIsFirstUpdate = false;
        }
    }

    public override void deactivate()
    {
        if (isActive() || mIsFirstUpdate)
        {
            mSR.sprite = deactiveSprite;

            mIsActive = false;
            mIsFirstUpdate = false;
        }
    }

    public override void toggle()
    {
        if (mIsActive)
            deactivate();
        else
            activate();
    }
	#endregion

    #region Instance Methods
    protected void teleport(Player player)
    {
        if (player == null || 
            mTeleportDestination == null || 
            mTeleportDestination.teleportSpawn == null)
            return;

        player.transform.position = mTeleportDestination.teleportSpawn.transform.position;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
        {
            Debug.Log("Teleporting player");

            teleport(player);
        }
    }
    #endregion
}
