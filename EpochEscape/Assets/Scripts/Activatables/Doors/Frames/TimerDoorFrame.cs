using UnityEngine;
using System.Collections;

public class TimerDoorFrame : LockedDoorFrame
{
    #region Interface Variables
    public int time = 1;

    public GUIStyle timerStyle;
    #endregion

    #region Instance Variables
    private int timeRemaining = 0;
    public TimerState m_timerState = TimerState.Ready;
    #endregion

    #region Class Constants
    public enum TimerState
    {
        Ready,
        Paused,
        Running,
        Expired,
        Beat
    };
    #endregion

    #region Interface Methods
    public override void triggerFrontEnter()
    {
        m_timerState = TimerState.Beat;
    }
    #endregion

    #region Instance Methods
    protected IEnumerator startTimer()
    {
        m_timerState = TimerState.Running;

        yield return StartCoroutine(countDownTimer());

        timerStyle.normal.textColor = Color.white;
    }

    private IEnumerator countDownTimer()
    {
        int panicTime = 5;
        bool panic = false;
        Color panicColor = Color.red;

        for (timeRemaining = time; timeRemaining >= 0; timeRemaining--)
        {
            if(m_timerState == TimerState.Beat)
                yield break;

            if(timeRemaining <= panicTime && !panic)
            {
                panic = true;

                timerStyle.normal.textColor = panicColor;
            }

            yield return new WaitForSeconds(1f);
        }

        m_timerState = TimerState.Expired;
        timeRemaining = 0;

        CameraManager.AddTransition(gameObject);
        CameraManager.PlayTransitions();
    }
    #endregion
    
    protected void OnGUI()
    {
    	if(m_timerState == TimerState.Paused || m_timerState == TimerState.Running) {
			GUI.Label(new Rect(Screen.width / 2 - 50f, 30f, 100f, 100f), timeRemaining.ToString(), timerStyle);
    	}
    }

    public override void OnFinishTransition()
    {
        if(m_timerState != TimerState.Beat && timeRemaining == 0)
        {
            base.OnFinishTransition();

            if (m_timerState == TimerState.Ready)
            {
                timeRemaining = time;

                m_timerState = TimerState.Paused;
            }
            else if(m_timerState == TimerState.Running)
                timeRemaining = time;
        }
    }

    public override void OnReadyIdle()
    {
        if (m_timerState != TimerState.Running && m_timerState != TimerState.Beat)
        {
            base.OnReadyIdle();

            if(m_timerState == TimerState.Ready || m_timerState == TimerState.Paused)
                StartCoroutine(startTimer());
            else
                m_timerState = TimerState.Ready;
        }
    }
}
