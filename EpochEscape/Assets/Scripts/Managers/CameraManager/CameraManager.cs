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
    private float m_initialCameraSize = 0f;
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

        m_initialCameraSize = m_camera.orthographicSize;

        //transform.position = new Vector3(PlayerManager.GetPosition().x, PlayerManager.GetPosition().y, transform.position.z);

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

        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;

        int i = 0;

        float xTemp = 0f;
        float yTemp = 0f;

        float transitionX = 0f;
        float transitionY = 0f;

        float paddingLeft = 0f;
        float paddingRight = 0f;
        float paddingTop = 0f;
        float paddingBottom = 0f;

        foreach (Transition transition in m_transitions)
        {
            if (i == 0)
            {
                min = transition.GetPosition();
                max = transition.GetPosition();

                transitionX = transition.GetSize().x;
                transitionY = transition.GetSize().y;

                paddingLeft = transitionX > transitionY ? transitionX : transitionY;
                paddingRight = transitionX > transitionY ? transitionX : transitionY;
                paddingTop = transitionX > transitionY ? transitionX : transitionY;
                paddingBottom = transitionX > transitionY ? transitionX : transitionY;
            }
            else
            {
                xTemp = transition.GetPosition().x;
                yTemp = transition.GetPosition().y;

                if(xTemp < min.x)
                {
                    min.x = xTemp;

                    paddingLeft = transitionX > transitionY ? transitionX : transitionY;
                }

                if(xTemp > max.x)
                {
                    max.x = xTemp;

                    paddingRight = transitionX > transitionY ? transitionX : transitionY;
                }

                if(yTemp < min.y)
                {
                    min.y = yTemp;

                    paddingBottom = transitionX > transitionY ? transitionX : transitionY;
                }

                if(yTemp > max.y)
                {
                    max.y = yTemp;

                    paddingTop = transitionX > transitionY ? transitionX : transitionY;
                }
            }

            i++;
        }

        float centerX = (min.x + max.x) / 2;
        float centerY = (min.y + max.y) / 2;

        float sizeX = Mathf.Abs(max.x - min.x) + (paddingLeft + paddingRight) * PADDING_MULTIPLIER;
        float sizeY = Mathf.Abs(max.y - min.y) + (paddingTop + paddingBottom) * PADDING_MULTIPLIER;

        Vector3 targetPosition = new Vector3(centerX, centerY, transform.position.z);
        Vector3 previousPosition = new Vector3(PlayerManager.GetPosition().x, PlayerManager.GetPosition().y, transform.position.z);
        float targetSize = m_initialCameraSize;

        if(m_transitions.Count > 1)
        {
            if(sizeX / sizeY > m_camera.aspect)
                targetSize = (sizeX / m_camera.aspect) / 2;
            else
                targetSize = sizeY / 2;
        }

        GameManager.getInstance().PauseMovement();

        m_isTransitioning = true;

        yield return StartCoroutine(TransformCamera(targetPosition, targetSize));
        yield return StartCoroutine(TransformObjects());
        yield return StartCoroutine(TransformCamera(previousPosition, m_initialCameraSize));

        // Crap code
        foreach(Transition transition in m_transitions)
            transition.OnReadyIdle();

        m_transitions.Clear();

        m_isTransitioning = false;
        m_waitTime = 0f;

        GameManager.getInstance().UnpauseMovement();
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
        CameraManager.GetInstance()._AddTransition(objectToTransition);
    }

    public static void PlayTransitions()
    {
        CameraManager.GetInstance()._PlayTransitions();
    }

    public static void Enable()
    {
        CameraManager.GetInstance()._Enable();
    }

    public static void Disable()
    {
        CameraManager.GetInstance()._Disable();
    }

    public static void SetPosition(Vector3 position)
    {
        CameraManager.GetInstance()._SetPosition(position);
    }
}
