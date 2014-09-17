using UnityEngine;
using System.Collections;

public class RightButton : GUIButton
{
    public override void OnPress()
    {
        PlayerManager.MoveRight(true);
    }

    public override void OnRelease()
    {
        PlayerManager.MoveRight(false);
    }
}
