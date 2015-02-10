using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class PowerCoreDoorFrame : DoorFrame
{
    #region Interface Variables
    public CORES powerCores = CORES.FULL;
    #endregion
    
    #region Instance Variables
    protected bool mHasUnlocked = false;
    #endregion

    #region Class Constants
    public new enum STATE
    {
        LOCKED,
        UNLOCKED
    }

    public enum CORES
    {
        NONE,
        ONE,
        TWO,
        FULL
    }
    #endregion
    
    protected new void Start()
    {
        initialState = DoorFrame.STATE.INACTIVE;

        base.Start();
        mFrontSide.deactivate();
        mBackSide.deactivate();
    }

    protected void Update()
    {
        if (!mHasUnlocked)
        {
            switch (powerCores)
            {
                case CORES.ONE:
                    if (Player.Get().getCurrentCores() == 1)
                    {
                        mFrontSide.activate();
                        mBackSide.activate();
                    }
                    break;
                case CORES.TWO:
                    if (Player.Get().getCurrentCores() == 2)
                    {
                        mFrontSide.activate();
                        mBackSide.activate();
                    }
                    break;
                case CORES.FULL:
                    if (Player.Get().getCurrentCores() == 3)
                    {
                        mFrontSide.activate();
                        mBackSide.activate();
                    }
                    break;
            }
        }
    }
    
    #region Interface Methods
    public override void triggerFrontEnter()
    {
        return;
    }
    public override void triggerFrontExit()
    {
        return;
    }

    public override void triggerBackEnter()
    {
        return;
    }
    public override void triggerBackExit()
    {
        return;
    }

    public override void activate()
    {
        return;
    }
    public override void deactivate()
    {
        return;
    }
    #endregion

    public override void Reset()
    {
        mFrontSide.deactivate();
        mBackSide.deactivate();
    }

    public override XmlElement Serialize(XmlDocument document)
    {
        XmlElement doorTag = base.Serialize(document);
        doorTag.SetAttribute("cores", powerCores.ToString());

        return doorTag;
    }
}
