using UnityEngine;
using System.Collections;

public class ExitDoorFrame : PowerCoreDoorFrame
{
    #region Interface Variables
    #endregion
    
    #region Instance Variables
    LevelManager mLevelManager;
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
            if (powerCores == CORES.NONE || mPlayer.currentCores == mPlayer.MAX_CORES)
                unlockDoor();
    }
    
    #region Interface Methods
    public void attachLevelManager(LevelManager levelManager)
    {
        mLevelManager = levelManager;
    }

    public void exitLevel(Player player)
    {
        if (player == null)
            return;

        mLevelManager.exitLevel();
    }
    #endregion
    
    #region Instance Methods
    #endregion
}
