using UnityEngine;
using System.Collections;

public class TeleporterDoorSide : DoorSide
{
    #region Interface Variables
    public GameObject teleportDestination;
    public GameObject teleportSpawn;
    public float teleportMovementDelay = 1.0f;
    #endregion
    
    #region Instance Variables
    TeleporterDoorSide mTeleportDestination;
    Vector3 mSpawnPosition;
    Player mPlayerToTeleport;

    bool mWasBackHit = false;
    bool mCanTeleport = false;
    #endregion 
    
    protected new void Awake()
    {
        base.Awake();

        if (teleportMovementDelay < 0)
            teleportMovementDelay = 1.0f;

        mSpawnPosition = teleportSpawn.transform.position;
        GameObject.Destroy(teleportSpawn);
    }
    
    protected void Start()
    {
        mTeleportDestination = teleportDestination.GetComponent<TeleporterDoorSide>();
    }
    
    #region Interface Methods
    public Vector3 getSpawnPosition()
    {
        return mSpawnPosition;
    }

    public override void triggerFrontEnter()
    {
        if (!mWasBackHit)
            mCanTeleport = true;
    }
    public override void triggerFrontExit()
    {
        mCanTeleport = false;
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
            mSR.sprite = inactiveSprite;

            mIsActive = false;
            mIsFirstUpdate = false;
        }
    }
    #endregion

    #region Instance Methods
    protected void teleport()
    {
        if (mPlayerToTeleport == null || 
            mTeleportDestination == null || 
            !mCanTeleport)
            return;

        mPlayerToTeleport.transform.position = mTeleportDestination.getSpawnPosition();
    }

    protected IEnumerator pauseMovement()
    {
        GameManager.Get().PauseMovement();

        yield return new WaitForSeconds(teleportMovementDelay);

        GameManager.Get().UnpauseMovement();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collidee)
    {
        mPlayerToTeleport = collidee.GetComponent<Player>();

        if (mPlayerToTeleport != null && isActive())
            teleport();
    }
    #endregion
}
