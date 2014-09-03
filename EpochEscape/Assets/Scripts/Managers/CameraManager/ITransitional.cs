using System.Collections;

public interface ITransitional
{
    void OnFinishTransition();
    float GetWaitTime();
}
