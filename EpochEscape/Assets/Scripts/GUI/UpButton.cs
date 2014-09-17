using UnityEngine;
using System.Collections;

public class UpButton : GUIButton
{
    public override void OnPress()
    {
        PlayerManager.MoveUp(true);
    }

    public override void OnRelease()
    {
        PlayerManager.MoveUp(false);
    }
}
