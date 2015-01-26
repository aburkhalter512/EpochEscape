using UnityEngine;
using System.Collections;

public class TeleporterDoorFrame : DoorFrame<StandardDoorSide, StandardDoorSide>
{
    #region Interface Variables
    public GameObject teleportTarget;

    public GameObject spawn;
    #endregion

    #region Instance Variables
    protected TeleporterDoorFrame mTeleportTarget;

    protected Vector3 mSpawn;
    #endregion

    protected new void Start()
    {
        base.Start();

        mTeleportTarget = teleportTarget.GetComponent<TeleporterDoorFrame>();
        mSpawn = spawn.transform.position;
    }

    #region Interface Methods
    /**
     *  void triggerFrontEnter()
     *      A method that alerts the door that a gameobject has entered the front detection area.
     */
    public override void triggerFrontEnter()
    {
        return;
    }

    /**
     *  void triggerFrontExit()
     *      A method that alerts the door that a gameobject has exited the front detection area.
     */
    public override void triggerFrontExit()
    {
        return;
    }

    /**
     *  void triggerBackEnter()
     *      A method that alerts the door that a gameobject has entered the back detection area.
     */
    public override void triggerBackEnter()
    {
        return;
    }

    /**
     *  void triggerBackExit()
     *      A method that alerts the door that a gameobject has exited the back detection area.
     */
    public override void triggerBackExit()
    {
        return;
    }

    public override void activate()
    {
        if (mState != STATE.ACTIVE)
        {
            mFrontSide.activate();
            mBackSide.deactivate();
            mState = STATE.INACTIVE;
        }
    }
    public override void deactivate()
    {
        if (mState != STATE.INACTIVE)
        {
            mFrontSide.deactivate();
            mBackSide.deactivate();
            mState = STATE.ACTIVE;
        }
    }

    public Vector3 getSpawnPosition()
    {
        return mSpawn;
    }
    #endregion

    #region Instance Methods
    protected void teleport(Player player)
    {
        if (player == null || mTeleportTarget == null)
            return;

        player.transform.position = mTeleportTarget.getSpawnPosition();

    }

    protected virtual void OnTriggerEnter2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null && mState == STATE.ACTIVE)
            teleport(player);
    }
    #endregion
}
