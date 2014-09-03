using UnityEngine;
using System.Collections;

public class TeleporterDoorSide : DoorSide, ITransitional
{
    #region Interface Variables
    public GameObject teleportDestination;
    public GameObject teleportSpawn;
    public float teleportMovementDelay = 1.0f;
    #endregion
    
    #region Instance Variables
    TeleporterDoorSide mTeleportDestination;
    Player mPlayerToTeleport;

    bool mWasFrontHit = false;
    bool mWasBackHit = false;
    bool mCanTeleport = false;
    #endregion 
    
    protected void Awake()
    {
        base.Awake();

        if (teleportMovementDelay < 0)
            teleportMovementDelay = 1.0f;
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

    public void OnFinishTransition()
    {
        return;
    }

    public float GetWaitTime()
    {
        return 0.01f;
    }
    #endregion

    #region Instance Methods
    protected void teleport()
    {
        if (mPlayerToTeleport == null || 
            mTeleportDestination == null || 
            mTeleportDestination.teleportSpawn == null ||
            !mCanTeleport)
            return;

        Debug.Log("Teleporting");

        mPlayerToTeleport.transform.position = mTeleportDestination.teleportSpawn.transform.position;

        CameraManager.AddTransition(mTeleportDestination.gameObject);
        CameraManager.PlayTransitions();
    }

    protected IEnumerator pauseMovement()
    {
        GameManager.getInstance().PauseMovement();

        yield return new WaitForSeconds(teleportMovementDelay);

        GameManager.getInstance().UnpauseMovement();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collidee)
    {
        mPlayerToTeleport = collidee.GetComponent<Player>();

        if (mPlayerToTeleport != null && isActive())
        {
            teleport();
        }
            //teleport(player);
    }
    #endregion
}
