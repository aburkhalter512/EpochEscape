using UnityEngine;
using System.Collections.Generic;

public class Terminal : InteractiveObject
{
    #region Interface Variables
    public Sprite activatedSprite;
    public Sprite deactivatedSprited;

    public GameObject[] actuators;
    #endregion

    #region InstanceVariables
    List<DynamicWall> mWallActuators;
    List<LockedDoorFrame> mDoorActuators;
    List<DoorSide> mDoorSideActuators;
    List<SecurityCamera> mCameraActuators;

    bool mIsActivated = false;
    bool mCanInteract = false;

    SpriteRenderer mSR;
    #endregion

    // Use this for initialization
    protected void Awake()
    {
        mSR = GetComponent<SpriteRenderer>();
    }

    protected void Start ()
    {
        MonoBehaviour script = null;

        mWallActuators = new List<DynamicWall>();
        mDoorActuators = new List<LockedDoorFrame>();
        mDoorSideActuators = new List<DoorSide>();
        mCameraActuators = new List<SecurityCamera>();

        foreach (GameObject actuator in actuators)
        {
            #region Retrieving Dynamic Wall Actuators
            script = actuator.GetComponent<DynamicWall>();

            if (script != null)
                mWallActuators.Add(script as DynamicWall);
            #endregion

            #region Retrieving Locked Door Actuators
            script = actuator.GetComponent<LockedDoorFrame>();
            if (script != null)
                mDoorActuators.Add(script as LockedDoorFrame);
            #endregion

            #region Retrieving Door Side Actuators
            script = actuator.GetComponent<DoorSide>();
            if (script != null)
                mDoorSideActuators.Add(script as DoorSide);
            #endregion

            #region Retrieving Camera Actuators
            script = actuator.GetComponent<SecurityCamera>();
            if (script != null)
                mCameraActuators.Add(script as SecurityCamera);
            #endregion
        }
    }

    protected void Update()
    {
        if (mCanInteract && InputManager.getInstance().interactButton.getDown())
            Interact();
    }
    
    public override void Interact()
    {
        foreach (DynamicWall actuator in mWallActuators)
            actuator.currentState = DynamicWall.STATES.CHANGE;

        foreach (LockedDoorFrame actuator in mDoorActuators)
            actuator.toggleLock();

        foreach (DoorSide actuator in mDoorSideActuators)
            actuator.toggle();

        foreach (SecurityCamera actuator in mCameraActuators)
            actuator.toggle();

        if (mIsActivated)
        {
            mSR.sprite = deactivatedSprited;
            mIsActivated = false;
        }
        else
        {
            mSR.sprite = activatedSprite;
            mIsActivated = true;
        }
    }

    public void OnTriggerEnter2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
            mCanInteract = true;
    }

    public void OnTriggerExit2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
            mCanInteract = false;
    }
}
