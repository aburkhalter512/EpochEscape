using UnityEngine;

public interface IDetectable
{
    void triggerFrontEnter();
    void triggerFrontExit();

    void triggerBackEnter();
    void triggerBackExit();
}