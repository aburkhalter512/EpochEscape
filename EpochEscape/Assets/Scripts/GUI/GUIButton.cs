using UnityEngine;
using System.Collections;

public abstract class GUIButton : MonoBehaviour
{
#if UNITY_ANDROID
    public void Start()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    public void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (guiTexture.HitTest(touch.position))
            {
                if (touch.phase == TouchPhase.Ended)
                    OnRelease();
                else
                    OnPress();
            }
        }
    }
#endif

    public abstract void OnPress();
    public abstract void OnRelease();
}
