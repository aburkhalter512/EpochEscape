using UnityEngine;
using System.Collections.Generic;

public class OneTimeTerminal : Terminal
{
    public override void Interact()
    {
        if (!mIsActivated)
            base.Interact();
    }
}
