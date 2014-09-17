using UnityEngine;
using System.Collections;

public class DownButton : GUIButton
{
    public override void OnPress()
    {
        PlayerManager.MoveDown(true);
    }

    public override void OnRelease()
    {
        PlayerManager.MoveDown(false);
    }
}
