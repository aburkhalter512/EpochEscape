using UnityEngine;
using System.Collections;

public class LeftButton : GUIButton
{
    public override void OnPress()
    {
        PlayerManager.MoveLeft(true);
    }

    public override void OnRelease()
    {
        PlayerManager.MoveLeft(false);
    }
}
