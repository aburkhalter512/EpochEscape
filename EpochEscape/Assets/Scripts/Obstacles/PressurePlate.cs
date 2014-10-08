using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour, ISerializable
{
    #region Inspector Variables
    public GameObject[] actuators;

    public Sprite switchOn;
    public Sprite switchOff;

    public STATE currentState;
    #endregion

    #region Instance Variables
    protected SpriteRenderer mSR;

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

        previousState = STATE.UN_INIT;
        currentState = STATE.ON;
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

    #region Update Methods
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
        if (collidee.tag == "Player")
        {
            audio.Play ();

            //Activate all of the connected actuators
            foreach (GameObject actuator in actuators)
            {
                //Debug.Log("Triggered");

                if (actuator != null)
                    CameraManager.AddTransition(actuator);
            }

            CameraManager.PlayTransitions();

            //currentState = STATE.OFF;

            currentState = (currentState == STATE.ON ? STATE.OFF : STATE.ON);
        }
    }

    /*
     * If the collidee is the player then the pressure plate is turned on.
     */
    virtual protected void OnTriggerExit2D(Collider2D collidee)
    {
        if (collidee.tag == "Player")
        {
            //currentState = STATE.ON;
        }
    }
    #endregion

    public void Serialize(ref Dictionary<string, object> data)
    {
        if (data != null && !data.ContainsKey("actuators"))
            data["actuators"] = actuators;
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

                    actuators = new GameObject[actuatorHashes.Count];

                    // Doors
                    if (doors != null)
                    {
                        for (int i = 0; i < actuatorHashes.Count; i++)
                        {
                            for (int j = 0; j < doors.Count; j++)
                            {
                                hashTemp = LevelEditorUtilities.GenerateObjectHash(doors[j].name, doors[j].transform.position);

                                if (hashTemp == actuatorHashes[i].ToString())
                                    actuators[i] = doors[j];
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
                                    actuators[i] = dynamicWalls[j];
                            }
                        }
                    }
                }
            }
        }
    }
}
