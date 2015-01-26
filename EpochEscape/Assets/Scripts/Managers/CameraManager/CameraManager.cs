using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : Manager<CameraManager>
{
    public const float TRANSITION_THRESHOLD = 0.01f;
    public const float TRANSITION_SPEED_MULTIPLIER = 5f;
    public const float PADDING_MULTIPLIER = 1.5f;

    private Camera m_camera = null;
    private bool m_isInitialized = false;

    private Queue<Transition> m_transitions = null;
    private float m_waitTime = 0f;
    private bool m_isTransitioning = false;
    private Vector3 m_idlePosition = Vector3.zero;

    protected override void Initialize()
    {
        m_isInitialized = InitializeCamera() && InitializeTransitionContainer();
    }

    public void Update()
    {
        _SetIdlePosition(PlayerManager.GetPosition());
        _UpdateIdlePosition();
    }

    private void _SetIdlePosition(Vector3 position)
    {
        m_idlePosition = new Vector3(position.x, position.y, transform.position.z);
    }

    private void _UpdateIdlePosition()
    {
        if(!m_isTransitioning)
            transform.position = Vector3.Lerp(transform.position, m_idlePosition, 2f * Time.smoothDeltaTime);
    }

    private bool InitializeCamera()
    {
        m_camera = GetComponent<Camera>();

        if(m_camera == null)
            return false;

        return true;
    }

    private bool InitializeTransitionContainer()
    {
        if(m_transitions == null)
        {
            m_transitions = new Queue<Transition>();

            if(m_transitions == null)
                return false;
        }
        else
            m_transitions.Clear();

        return true;
    }

    private void _AddTransition(GameObject objectToTransition)
    {
        if(m_isInitialized && objectToTransition != null)
        {
            Transition transition = new Transition(objectToTransition);

                if(transition != null && transition.IsInitialized())
                {
                    m_transitions.Enqueue(transition);

                    if (transition.GetWaitTime() > m_waitTime)
                        m_waitTime = transition.GetWaitTime();
                }
        }
    }

    private void _PlayTransitions()
    {
        if (m_isInitialized && m_transitions.Count > 0 && !m_isTransitioning)
            StartCoroutine(ProcessAllTransitions());
    }

    private void _SetPosition(Vector3 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    private IEnumerator ProcessAllTransitions()
    {
        if (!m_isInitialized)
            yield break;

        m_isTransitioning = true;

        foreach (Transition transition in m_transitions)
            transition.OnFinishTransition();

        m_transitions.Clear();
        m_isTransitioning = false;
    }

    private IEnumerator TransformCamera(Vector3 targetPosition, float targetSize)
    {
        if (!m_isInitialized)
            yield break;

        while (Vector3.Distance(transform.position, targetPosition) > TRANSITION_THRESHOLD || Mathf.Abs(targetSize - m_camera.orthographicSize) > TRANSITION_THRESHOLD)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, TRANSITION_SPEED_MULTIPLIER * Time.smoothDeltaTime);
            m_camera.orthographicSize = Mathf.Lerp(m_camera.orthographicSize, targetSize, TRANSITION_SPEED_MULTIPLIER * Time.smoothDeltaTime);

            yield return null;
        }

        m_camera.orthographicSize = targetSize;
    }

    private void _Enable()
    {
        gameObject.SetActive(true);
    }

    private void _Disable()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator TransformObjects()
    {
        foreach (Transition transition in m_transitions)
            transition.OnFinishTransition();

        //m_transitions.Clear();

        yield return new WaitForSeconds(m_waitTime);
    }

    public static void AddTransition(GameObject objectToTransition)
    {
        CameraManager.Get()._AddTransition(objectToTransition);
    }

    public static void PlayTransitions()
    {
        CameraManager.Get()._PlayTransitions();
    }

    public static void Enable()
    {
        CameraManager.Get()._Enable();
    }

    public static void Disable()
    {
        CameraManager.Get()._Disable();
    }

    public static void SetPosition(Vector3 position)
    {
        CameraManager.Get()._SetPosition(position);
    }
}
