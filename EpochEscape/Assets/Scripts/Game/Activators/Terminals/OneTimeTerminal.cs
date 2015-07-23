using UnityEngine;
using System.Collections.Generic;

namespace Game
{
    public class OneTimeTerminal : Terminal
    {
        public override void Interact()
        {
            if (!mIsActivated)
                base.Interact();
        }
    }
}
