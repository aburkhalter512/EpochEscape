using System.Collections;

public interface ITransitional
{
    void OnFinishTransition();
    void OnReadyIdle();
    float GetWaitTime();
}
