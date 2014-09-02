using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : Manager<CameraManager>
{
    public const float TRANSITION_THRESHOLD = 0.05f;
    public const float DEFAULT_TRANSITION_DURATION = 0.33f; // Seconds

    private Camera camera = null;
    private bool isInitialized = false;

    private Queue<Transition> transitions = null;

    public override void Awake()
    {
        base.Awake();

        isInitialized = InitializeCamera() && InitializeTransitionContainer();
    }

    private IEnumerator ProcessAllTransitions()
    {
        if(isInitialized)
        {
            while(transitions.Count > 0)
            {
                yield return StartCoroutine(ProcessSingleTransition());
                yield return null;
            }
        }
    }

    private IEnumerator ProcessSingleTransition()
    {
        if(isInitialized)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = new Vector3(transitions.Peek().GetPosition().x, transitions.Peek().GetPosition().y, transform.position.z);

            float timeStarted = Time.time;
            float timeSinceStarted = 0f;
            float percentageComplete = 0f;

            while(Vector3.Distance(transform.position, targetPosition) > TRANSITION_THRESHOLD)
            {
                timeSinceStarted = Time.time - timeStarted;
                percentageComplete = timeSinceStarted / transitions.Peek().GetDuration();

                transform.position = Vector3.Lerp(startPosition, targetPosition, percentageComplete);

                yield return null;
            }

            transform.position = targetPosition;

            yield return StartCoroutine(transitions.Peek().OnFinishTransition());

            transitions.Dequeue();
        }
    }

    private bool InitializeCamera()
    {
        camera = GetComponent<Camera>();

        if(camera == null)
            return false;

        return true;
    }

    private bool InitializeTransitionContainer()
    {
        if(transitions == null)
        {
            transitions = new Queue<Transition>();

            if(transitions == null)
                return false;
        }
        else
            transitions.Clear();

        return true;
    }

    private void _AddTransition(GameObject objectToTransition, float duration = DEFAULT_TRANSITION_DURATION)
    {
        if(isInitialized && objectToTransition != null)
        {
            ITransitional transitional = objectToTransition.GetComponent(typeof(ITransitional)) as ITransitional;

            if(transitional != null)
            {
                Transition transition = new Transition(objectToTransition.transform.position, transitional, duration);

                if(transition != null)
                    transitions.Enqueue(transition);
            }
        }
    }

    private void _PlayTransitions()
    {
        if(isInitialized)
            StartCoroutine(ProcessAllTransitions());
    }

    public static void AddTransition(GameObject objectToTransition, float duration = DEFAULT_TRANSITION_DURATION)
    {
        CameraManager.GetInstance()._AddTransition(objectToTransition, duration);
    }

    public static void PlayTransitions()
    {
        CameraManager.GetInstance()._PlayTransitions();
    }
}
