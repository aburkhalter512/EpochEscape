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
    private Player m_player = null;

    public override void Awake()
    {
        base.Awake();

        m_isInitialized = InitializeCamera() && InitializeTransitionContainer();
    }

    public void Update()
    {
        FindPlayer();
        FollowPlayer();
    }

    private void FindPlayer()
    {
        if (m_player == null)
        {
            GameObject player = GameObject.FindWithTag("Player");

            if (player != null)
                m_player = player.GetComponent<Player>();
        }
    }

    private void FollowPlayer()
    {
        if (!(m_player == null || m_isTransitioning))
            transform.position = Vector3.Lerp(transform.position, new Vector3(m_player.transform.position.x, m_player.transform.position.y, transform.position.z), 2f * Time.smoothDeltaTime);
    }

    private bool InitializeCamera()
    {
        m_camera = GetComponent<Camera>();

        if(m_camera == null)
            return false;

        m_initialCameraSize = m_camera.orthographicSize;

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

    private IEnumerator ProcessAllTransitions()
    {
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
        Vector3 previousPosition = new Vector3(m_player.transform.position.x, m_player.transform.position.y, transform.position.z);
        float targetSize = 0f;

        if (sizeX / sizeY > m_camera.aspect)
            targetSize = (sizeX / m_camera.aspect) / 2;
        else
            targetSize = sizeY / 2;

        GameManager.PauseMovement();

        m_isTransitioning = true;

        yield return StartCoroutine(TransformCamera(targetPosition, targetSize));
        yield return StartCoroutine(TransformObjects());
        yield return StartCoroutine(TransformCamera(previousPosition, m_initialCameraSize));

        m_isTransitioning = false;
        m_waitTime = 0f;

        GameManager.UnpauseMovement();
    }

    private IEnumerator TransformCamera(Vector3 targetPosition, float targetSize)
    {
        if (!m_isInitialized)
            yield break;

        while (Vector3.Distance(transform.position, targetPosition) > TRANSITION_THRESHOLD && Mathf.Abs(targetSize - m_camera.orthographicSize) > TRANSITION_THRESHOLD)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, TRANSITION_SPEED_MULTIPLIER * Time.smoothDeltaTime);
            m_camera.orthographicSize = Mathf.Lerp(m_camera.orthographicSize, targetSize, TRANSITION_SPEED_MULTIPLIER * Time.smoothDeltaTime);

            yield return null;
        }

        m_camera.orthographicSize = targetSize;
    }

    private IEnumerator TransformObjects()
    {
        foreach (Transition transition in m_transitions)
            transition.OnFinishTransition();

        m_transitions.Clear();

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
}
