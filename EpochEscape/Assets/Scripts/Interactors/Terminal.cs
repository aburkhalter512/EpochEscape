using UnityEngine;
using System.Collections.Generic;

public class Terminal : InteractiveObject, IResettable, ISerializable
{
    #region Interface Variables
    public Sprite activatedSprite;
    public Sprite deactivatedSprited;

    public GameObject[] actuators;
    #endregion

    #region InstanceVariables
    protected bool mIsActivated = false;
    protected bool mCanInteract = false;

    SpriteRenderer mSR;
    #endregion

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();

        mSR = GetComponent<SpriteRenderer>();
    }
    
    public override void Interact()
    {
        if (mCanInteract)
        {
            foreach (GameObject actuator in actuators)
                CameraManager.AddTransition(actuator);

            CameraManager.PlayTransitions();

            mSR.sprite = (mIsActivated ? deactivatedSprited : activatedSprite);
            mIsActivated = !mIsActivated;
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
            mCanInteract = true;
    }

    public virtual void OnTriggerExit2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
            mCanInteract = false;
    }

    public void Reset()
    {
        mCanInteract = false;
        mIsActivated = false;
        
        mSR.sprite = deactivatedSprited;
    }

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
