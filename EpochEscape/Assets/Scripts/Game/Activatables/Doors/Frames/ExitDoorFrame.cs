using UnityEngine;
using System.Collections;

public class ExitDoorFrame : PowerCoreDoorFrame
{
    protected new void Update()
    {
        if (!mHasUnlocked)
            if (powerCores == CORES.NONE || Player.Get().getCurrentCores() == Player.MAX_CORES)
                activate();
    }
}
