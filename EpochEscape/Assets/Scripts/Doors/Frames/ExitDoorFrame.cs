using UnityEngine;
using System.Collections;

public class ExitDoorFrame : PowerCoreDoorFrame
{
    #region Interface Variables
    #endregion
    
    #region Instance Variables
    #endregion

    #region Class Constants
    #endregion
    
    protected void Start()
    {
        base.Start();
    }

    protected void Update()
    {
        if (!mHasUnlocked)
            if (powerCores == CORES.NONE || PlayerManager.GetCores() == 3)
                unlockDoor();
    }
    
    #region Interface Methods
    #endregion
    
    #region Instance Methods
    #endregion
}
