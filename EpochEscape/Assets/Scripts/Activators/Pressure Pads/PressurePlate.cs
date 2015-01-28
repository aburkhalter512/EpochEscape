using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour, ISerializable
{
    #region Inspector Variables
    public GameObject[] activatables;

    public Sprite switchOn;
    public Sprite switchOff;

    public STATE currentState;
    #endregion

    #region Instance Variables
    protected List<IActivatable> mActivatables;

    protected SpriteRenderer mSR;

    protected BoxCollider2D mCollider;
    protected Vector2 mBaseSize;

    private STATE previousState;
    #endregion

    #region Class Constants
    public enum STATE
    {
        ON,
        OFF,
        UN_INIT
    }
    #endregion

    /*
     * Initializes the Pressure Plate
     */
    protected void Start()
    {
        mSR = gameObject.GetComponent<SpriteRenderer>();
        mCollider = gameObject.GetComponent<BoxCollider2D>();
        mBaseSize = mCollider.size;

        previousState = STATE.UN_INIT;
        currentState = STATE.ON;

        mActivatables = new List<IActivatable>();
        foreach (GameObject activatable in activatables)
        {
            if (activatable == null)
                continue;

            IActivatable actuator = activatable.GetComponent<MonoBehaviour>() as IActivatable;

            if (actuator != null)
                mActivatables.Add(actuator);
        }
    }

    /*
     * Updates the state of the Pressure Plate
     */
    protected void Update()
    {
        if (previousState != currentState)
        {
            switch (currentState)
            {
                case STATE.OFF:
                    Off();
                    break;
                case STATE.ON:
                    On();
                    break;
            }

            previousState = currentState;
        }
    }

    #region Instance Methods
    /*
     * Turns the pressure plate off
     */
    virtual protected void Off()
    {
        mSR.sprite = switchOff;
    }

    /*
     * Turns the pressure plate on
     */
    virtual protected void On()
    {
        mSR.sprite = switchOn;
    }

    /*
     * If the collidee is the player, then all actuators are triggered and the
     * pressure plate is turned off.
     */
    virtual protected void OnTriggerEnter2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
        {
            mCollider.size *= 2;

            audio.Play ();

            GameManager.Get().delayFunction(() =>
                {
                    //Move the player to the center of the pressure plate
                    player.transform.position = transform.position;
                });

            //Activate all of the connected actuators
            foreach (IActivatable activatable in mActivatables)
                activatable.toggle();

            currentState = (currentState == STATE.ON ? STATE.OFF : STATE.ON);
        }
    }

    virtual protected void OnTriggerExit2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
        {
            mCollider.size = mBaseSize;
        }
    }
    #endregion

    public void Serialize(ref Dictionary<string, object> data)
    {
        if (data != null && !data.ContainsKey("actuators"))
            data["actuators"] = activatables;
    }

    public void Unserialize(ref Dictionary<string, object> data)
    {
        if (data != null && data.ContainsKey("actuators"))
        {
            List<object> actuatorHashes = data["actuators"] as List<object>;

            if (actuatorHashes != null)
            {
                int actuatorCount = 0;

                List<GameObject> doors = SceneManager.GetDoors();
                List<GameObject> dynamicWalls = SceneManager.GetDynamicWalls();

                if (doors != null)
                    actuatorCount += doors.Count;

                if (dynamicWalls != null)
                    actuatorCount += dynamicWalls.Count;

                if (actuatorCount > 0)
                {
                    string hashTemp = string.Empty;

                    activatables = new GameObject[actuatorHashes.Count];

                    // Doors
                    if (doors != null)
                    {
                        for (int i = 0; i < actuatorHashes.Count; i++)
                        {
                            for (int j = 0; j < doors.Count; j++)
                            {
                                hashTemp = LevelEditorUtilities.GenerateObjectHash(doors[j].name, doors[j].transform.position);

                                if (hashTemp == actuatorHashes[i].ToString())
                                    activatables[i] = doors[j];
                            }
                        }
                    }

                    // Dynamic Walls
                    if (dynamicWalls != null)
                    {
                        for (int i = 0; i < actuatorHashes.Count; i++)
                        {
                            for (int j = 0; j < dynamicWalls.Count; j++)
                            {
                                hashTemp = LevelEditorUtilities.GenerateObjectHash(dynamicWalls[j].name, dynamicWalls[j].transform.position);

                                if (hashTemp == actuatorHashes[i].ToString())
                                    activatables[i] = dynamicWalls[j];
                            }
                        }
                    }
                }
            }
        }
    }
}
