using UnityEngine;
using System;
using System.IO;
using System.Xml;

/**
 * This class a door frame for Epoch Escape. It contains two door sides along with player
 * detectors and should provide core interactability with the player. There needs to be two
 * gameobject children that acts as the detectors. Each detector needs to have either a 
 * FrontDoorDetector or a BackDoorDetector script attached.
 * 
 * This DoorFrame and derived classes represent the primary location for interacting with
 * doors via code. It provides an interface for activating (opening) and deactivating
 * (closing) sides and door sides SHOULD BE INTERACTED WITH THROUGH THE DOOR FRAME.
 * While it is possible to find the door side gameobject through GameObject.Find and directly
 * modify the door side THIS IS HIGHLY DISCOURAGED as this might break internal state 
 * variables and can unexpected behaviour.
 * 
 * Interface Variables
 *      frontSide: GameObject
 *          This variable is used for 'attaching' a side to a door frame, specifically the
 *          front side. 'frontSide' should contain a DoorSide component, otherwise null
 *          reference exceptions will occur. This variable must be set on gameobject creation.
 *      backSide: GameObject
 *          This variable is used for 'attaching' a side to a door frame, specifically the
 *          back side. 'backSide' should contain a DoorSide component, otherwise null
 *          reference exceptions will occur. This variable must be set on gameobject creation.
 *          
 * Interface Methods
 *      void triggerFrontEnter()
 *          A method that alerts the door that a gameobject has entered the front detection area.
 *      void triggerBackExit()
 *          A method that alerts the door that a gameobject has exited the back detection area.
 *      void triggerBackEnter()
 *          A method that alerts the door that a gameobject has entered the back detection area.
 *      void triggerBackExit()
 *          A method that alerts the door that a gameobject has exited the back detection area.
 *      void activateSide(SIDE side)
 *          A method that activates a specific side of a door.
 *          Parameters:
 *              side: The side of the door to activate.
 *      void deactivateSide(SIDE side)
 *          A method that deactivates a specific side of a door.
 *          Parameters:
 *              side: The side of the door to deactivate.
 *      void activateSides()
 *          A method that activates both sides of the door.
 *      void deactivateSides
 *          A method that deactivates both sides of the door.
 */
public abstract class DoorFrame: 
    MonoBehaviour, 
    IActivatable, IDetectable, IResettable, ISerializable, IIdentifiable 
{
    #region Interface Variables
    public GameObject frontSide;
    public GameObject backSide;

    public STATE initialState = STATE.IDLE;
    #endregion
    
    #region Instance Variables
    protected DoorSide mFrontSide;
    protected DoorSide mBackSide;

    protected STATE mState;

    private string mID = "";
    #endregion 

    #region Class Constants
    public enum STATE
    {
        IDLE = 0,
        ACTIVE,
        INACTIVE
    }
    #endregion

    protected void Start()
    {
        getID();

        mFrontSide = frontSide.GetComponent<DoorSide>();
        mBackSide = backSide.GetComponent<DoorSide>();

        mState = initialState;
        setState(mState);
    }
    
    #region Interface Methods
    /**
     *  void triggerFrontEnter()
     *      A method that alerts the door that a gameobject has entered the front detection area.
     */
    public virtual void triggerFrontEnter()
    {
        mFrontSide.triggerFrontEnter();
        mBackSide.triggerBackEnter();

    }

    /**
     *  void triggerFrontExit()
     *      A method that alerts the door that a gameobject has exited the front detection area.
     */
    public virtual void triggerFrontExit()
    {
        mFrontSide.triggerFrontExit();
        mBackSide.triggerBackExit();
    }

    /**
     *  void triggerBackEnter()
     *      A method that alerts the door that a gameobject has entered the back detection area.
     */
    public virtual void triggerBackEnter()
    {
        mFrontSide.triggerBackEnter();
        mBackSide.triggerFrontEnter();
    }

    /**
     *  void triggerBackExit()
     *      A method that alerts the door that a gameobject has exited the back detection area.
     */
    public virtual void triggerBackExit()
    {
        mFrontSide.triggerBackExit();
        mBackSide.triggerFrontExit();
    }

    /**
     * void activateSides()
     *      A method that activates both sides of the door.
     */
    public virtual void activate()
    {
        mState = STATE.ACTIVE;

        mFrontSide.activate();
        mBackSide.activate();
    }

    /**
     * void deactivateSides()
     *      A method that deactivates both sides of the door.
     */
    public virtual void deactivate()
    {
        mState = STATE.INACTIVE;

        mFrontSide.deactivate();
        mBackSide.deactivate();
    }

    public virtual void toggle()
    {
        switch (mState)
        {
            case STATE.ACTIVE:
                deactivate();
                break;
            case STATE.IDLE:
            case STATE.INACTIVE:
                activate();
                break;
        }
    }

    public void setState(STATE state)
    {
        switch (mState)
        {
            case STATE.ACTIVE:
                activate();
                break;
            case STATE.IDLE:
            case STATE.INACTIVE:
                deactivate();
                break;
        }
    }

    public virtual void Reset()
    {
        mState = initialState;

        toggle();
    }

    public virtual XmlElement Serialize(XmlDocument document)
    {
        XmlElement doorTag = document.CreateElement("door");
        doorTag.SetAttribute("id", getID());
        doorTag.SetAttribute("type", GetType().ToString());
        doorTag.SetAttribute("initialState", initialState.ToString());

        doorTag.AppendChild(ComponentSerializer.toXML(transform, document));

        return doorTag;
    }

    public virtual string getID()
    {
        if (mID == "")
            mID = Utilities.generateUUID(this);

        return mID;
    }

    public virtual void setID(string id)
    {
        mID = id;
    }
    #endregion
}
