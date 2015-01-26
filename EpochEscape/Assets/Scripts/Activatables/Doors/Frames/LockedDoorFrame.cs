using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class LockedDoorFrame : DoorFrame<StandardDoorSide, StandardDoorSide>
{
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
    #endregion
}
