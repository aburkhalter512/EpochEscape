using UnityEngine;
using System.Collections;
using G = GameManager;

public class Player : MonoBehaviour
{
    #region Inspector Variables
    public LevelManager levelManager;

    public float m_interactionDistance = .2f;

    public bool m_isMoving;
    public bool m_isMovingForward;
    public bool m_isMovingDown;
    public bool m_isMovingLeft;
    public bool m_isMovingRight;
    private bool m_isThrowing;
    public bool m_isAttacking;
    public bool m_hasSpecialItem;
    private bool m_isDrinking;
    private float m_directionAngle;

    public AudioClip[] grateFoot;
    public AudioClip[] solidFoot;
    public int m_floorType; // 0 = solid, 1 = grate
    private int m_footCounter;

    public bool m_isDetected;
    public bool m_isHiding;
    public bool m_isWithinEarshot;

    public float m_rotationSpeed = 15f;
    public float m_detectionLevel = 0f;
    public float m_detectionRate = 50f; // rate/second at which character becomes detected by cameras
    public float m_detectionFade = 1.5f;
    public float m_detectionMax = 0f;
    public float m_detectionThres = 15f;
    public float Boost = 1f;
    public float BoostTime = 0f;

    public PlayerState m_currentState;
    //public Vector3 m_spawnLocation;

    public Inventory inventory;
    public int m_selectedSlot;
    public int m_specItems = 0;

    public int MAX_CORES = 3;

    //private Vector3 m_previousMouseScreenPos;

    Transform m_collectAnimation = null;
    public bool m_playSpecialItemAnim = false;
    public float m_specialItemAnimDuration; // seconds
    public float m_specialItemCollectTime = 0f;

    public bool m_isHoldingBox = false;
    public GameObject k_boxPrefab;
    #endregion

    #region Instance Variables
    private Animator m_animator;

    public int currentCores = 0;

    private InputManager mIM = null;
    #endregion

    #region Class Constants
    public const float SPEED = 2f;
    public const float MAX_DETECTION_LEVEL = 100.0f;
    public const int UNIQUE_ITEM_SLOTS = 1;

    // Cave Girl variables
    private bool m_isAtHitFrame = false;
    private bool m_isClubBroken = false;

    public bool m_isShieldActive = false;
    public float m_shieldDuration = 3f; // seconds
    public float m_shieldTime = 0f;

    public enum PlayerState
    {
        ALIVE,
        DEAD
    }
    #endregion

#if UNITY_ANDROID
    public bool m_up = false;
    public bool m_down = false;
    public bool m_left = false;
    public bool m_right = false;
#endif

    #region Class Variables
    public bool Special = false;
    #endregion

    #region Initialization Methods
    protected void Start()
    {
        //FadeManager.StartAlphaFade (Color.black, true, 1f, 0f);

        m_animator = GetComponent<Animator>();

        m_floorType = 0;
        m_isMoving = false;
        m_isMovingForward = false;
        m_isMovingDown = false;
        m_isMovingLeft = false;
        m_isMovingRight = false;
        m_isThrowing = false;
        m_hasSpecialItem = false;
        m_isDrinking = false;

        m_isDetected = false;
        m_isHiding = false;
        m_isWithinEarshot = false;
        m_currentState = PlayerState.ALIVE;

        inventory = new Inventory();
        m_selectedSlot = 0;
        //transform.position = m_spawnLocation;

        //m_previousMouseScreenPos = Vector3.zero;

        m_collectAnimation = transform.FindChild("CollectAnimation");

        mIM = InputManager.getInstance();
    }
    #endregion

    public void Interact() { //REQUIREMENT: DISABLE RAYCAST HITTING TRIGGERS IN EDIT->PROJECT SETTINGS->PHYSICS2D
        if (mIM.interactButton.getDown()) {
            if (!m_isHoldingBox) {
                collider2D.enabled = false;
                RaycastHit2D hit = Physics2D.Raycast (transform.position, transform.up, m_interactionDistance);
                collider2D.enabled = true;
                if (hit.collider != null) {
                    if (hit.collider.gameObject.tag == "InteractiveObject") {
                        hit.collider.gameObject.SendMessage("Interact");
                    }
                }
            } else {
                collider2D.enabled = false;
                RaycastHit2D hit = Physics2D.Raycast (transform.position, transform.up, m_interactionDistance);
                collider2D.enabled = true;
                if (hit.collider != null) {
                    if (hit.collider.gameObject.GetComponent<PitFloor>() != null) {
                        hit.collider.gameObject.SendMessage("Interact");
                    }
                } else {
                    HoldableBox box = GetComponentInChildren<HoldableBox>();
                    box.Place();
                }
            }
        }
    }

    #region Update Methods
    protected void Update()
    {
        if(!G.getInstance ().paused)
            UpdateCurrentState();
        else
        {
            m_isMoving = false;
            m_isMovingForward = false;
            m_isMovingDown = false;
            m_isMovingLeft = false;
            m_isMovingRight = false;
        }

        UpdateAnimator();
    }

    #region State Directions
    private void UpdateCurrentState()
    {
        switch(m_currentState)
        {
        case PlayerState.ALIVE:
            Alive();
            break;

        case PlayerState.DEAD:
            Dead();
            break;
        }
    }

    private void Alive()
    {
        UpdateUserControl();
        UpdateDetection();
        UpdateDirection();
        UpdateMovement();

        UpdateItemCollectionAnim();
        UpdateShield();
    }

    private void Dead()
    {
        //m_currentState = PlayerState.ALIVE;
        
        //Application.LoadLevel(Application.loadedLevelName);
        G.getInstance().PauseMovement();
        
        /*
        transform.position = new Vector3(-5f, 0f, 0f);
        transform.up = -Vector3.up;
        */

        HUDManager.Hide();
        
        //CameraBehavior cam = Camera.main.GetComponent<CameraBehavior>();
        
        GameObject playerCaught = Resources.Load("Prefabs/PlayerCaught") as GameObject;
        
        if(playerCaught != null)
        {
            playerCaught = Instantiate(playerCaught) as GameObject;
            playerCaught.transform.position = transform.position;
            //playerCaught.GetComponent<PlayerCaughtAnimation>().levelManager = levelManager;
        }
        
        //cam.m_currentState = CameraBehavior.State.PLAYER_CAUGHT;
    }
    #endregion

    #region Update UserControl
    private void UpdateUserControl()
    {
        UpdateMouse();
        UpdateKeyboard();
    }
    
    private void UpdateMouse()
    {
        if(!m_isMoving)
        {
            Vector3 mouseScreenPosition = mIM.mouse.inScreen();
            Vector3 mouseWorldPosition = mIM.mouse.inWorld();
            Vector3 toMousePosition = mouseWorldPosition - transform.renderer.bounds.center;
            toMousePosition.z = 0f;

            //m_previousMouseScreenPos = mouseScreenPosition;

            CircleCollider2D playerCollider = GetComponent<CircleCollider2D>();

            if(playerCollider != null && toMousePosition.magnitude > playerCollider.radius)
            {
                toMousePosition.Normalize();

                float angle = Utilities.RotateTowards(gameObject, toMousePosition, m_rotationSpeed);

                if(Utilities.IsApproximately(angle, 0f))
                {
                    transform.up = toMousePosition;
                    transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
                }
            }
        }
    }

    private void UpdateKeyboard()
    {
        m_isMoving = false;
        m_isMovingForward = false;
        m_isMovingDown = false;
        m_isMovingLeft = false;
        m_isMovingRight = false;

        if(m_isThrowing || m_isDrinking || m_isAttacking) return;

        if(Input.GetKey(KeyCode.W)) m_isMovingForward = true;
        if(Input.GetKey(KeyCode.S)) m_isMovingDown = true;
        if(Input.GetKey(KeyCode.A)) m_isMovingLeft = true;
        if(Input.GetKey(KeyCode.D)) m_isMovingRight = true;

#if UNITY_ANDROID
        m_isMovingForward = m_up;
        m_isMovingDown = m_down;
        m_isMovingLeft = m_left;
        m_isMovingRight = m_right;
#endif

        if(m_isMovingForward && m_isMovingDown)
            m_isMovingForward = m_isMovingDown = false;

        if(m_isMovingLeft && m_isMovingRight)
            m_isMovingLeft = m_isMovingRight = false;

        if(m_isMovingForward || m_isMovingDown || m_isMovingLeft || m_isMovingRight)
            m_isMoving = true;

        SelectSlot();
        Interact ();

        // 0th subscript indicates empty flask.
        if(inventory.inventory[m_selectedSlot] != null && !m_isShieldActive)
        {
            if(mIM.actionButton.getDown())
            {
                if(m_selectedSlot == 0)
                {
                    m_isDrinking = true;

                    inventory.activateItem(m_selectedSlot);
                }
                else if(m_selectedSlot == 1)
                    m_isThrowing = true;
            }
        }
        
        if (mIM.specialActionButton.getDown() && inventory.inventory[Inventory.SPECIAL_SLOT] != null)
        {
            if(!m_isShieldActive)
            {
                m_isAttacking = true;

                inventory.activateItem(Inventory.SPECIAL_SLOT);
            }

            if(inventory.inventoryCount[Inventory.SPECIAL_SLOT] <= 0)
                m_isClubBroken = true; // Cave Girl
        }

        if(mIM.itemSwitcher.get() > 0)
            ScrollSlotIncrement ();
        if (mIM.itemSwitcher.get() < 0)
            ScrollSlotDecrement();
    }

    public void ScrollSlotIncrement(){
        m_selectedSlot++;
        if(m_selectedSlot > UNIQUE_ITEM_SLOTS)
            m_selectedSlot = 0;
    }

    public void ScrollSlotDecrement(){
        m_selectedSlot--;
        if(m_selectedSlot < 0)
            m_selectedSlot = UNIQUE_ITEM_SLOTS;
    }

    private void SelectSlot() {
        if(mIM.itemButtons[0].getDown()){
            m_selectedSlot = 0;
        }
        else if (mIM.itemButtons[1].getDown())
        {
            m_selectedSlot = 1;
        }
    }

    public void FinishedThrowing()
    {
        m_isThrowing = false;

        inventory.activateItem(m_selectedSlot);
    }

    public void FinishedDrinking()
    {
        m_isDrinking = false;

        //inventory.activateItem(m_selectedSlot);
    }

    public void FinishedAttacking()
    {
        m_isAttacking = false;

        if(inventory.inventory[Inventory.SPECIAL_SLOT] == null)
        {
            m_isClubBroken = false;
            m_hasSpecialItem = false;
        }
    }

    private void UpdateMovement()
    {
        if (m_isMoving)
        {
            float xDirection = Mathf.Cos(m_directionAngle * Mathf.Deg2Rad);
            float yDirection = Mathf.Sin(m_directionAngle * Mathf.Deg2Rad);
            Vector3 direction = new Vector3(xDirection, yDirection, 0f);

            transform.position += (direction * SPEED * Time.smoothDeltaTime);
        }
    }

    private void UpdateDirection()
    {
        if(!m_isMoving) return;
        
        if(m_isMovingForward)
        {
            if(m_isMovingLeft)
                m_directionAngle = 135f;
            else if(m_isMovingRight)
                m_directionAngle = 45f;
            else
                m_directionAngle = 90f;
        }
        else if(m_isMovingDown)
        {
            if(m_isMovingLeft)
                m_directionAngle = 225f;
            else if(m_isMovingRight)
                m_directionAngle = 315f;
            else
                m_directionAngle = 270f;
        }
        else if(m_isMovingLeft)
            m_directionAngle = 180f;
        else if(m_isMovingRight)
            m_directionAngle = 0f;
        
        // Calculate the target vector and normalize it.
        Vector3 target = new Vector3(Mathf.Cos(m_directionAngle * Mathf.Deg2Rad), Mathf.Sin(m_directionAngle * Mathf.Deg2Rad), 0f);
        target.Normalize();
        
        float currentAngle = Utilities.RotateTowards(gameObject, target, m_rotationSpeed);
        
        if(Utilities.IsApproximately(currentAngle, 0f))
        {
            // Align the up transformation perfectly with the target vector.
            transform.up = target;
            
            // Reset the y Euler angle in case it changed randomly.
            transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
        }
    }
    #endregion

    #region Update Detection
    private void UpdateDetection() 
    {
        if (m_isDetected) {
            m_detectionLevel += m_detectionRate * Time.deltaTime;
            if (m_detectionLevel >= MAX_DETECTION_LEVEL)
            {
                m_currentState = PlayerState.DEAD;
                m_detectionLevel = 0;
            }
            if (m_detectionLevel > m_detectionMax) {
                m_detectionMax = m_detectionLevel;
            }
        } else {
            if (m_detectionLevel > (m_detectionMax - m_detectionThres) && m_detectionLevel > 0)
                m_detectionLevel -= m_detectionFade * Time.deltaTime;
        }
    }
    #endregion

    #region Update Animation
    private void UpdateAnimator()
    {
        if(m_animator != null)
        {
            m_animator.SetBool("isMoving", m_isMoving);
            m_animator.SetBool("isMovingForward", m_isMovingForward);
            m_animator.SetBool("isMovingBackward", m_isMovingDown);
            m_animator.SetBool("isMovingLeft", m_isMovingLeft);
            m_animator.SetBool("isMovingRight", m_isMovingRight);
            m_animator.SetBool("isAttacking", m_isAttacking);
            m_animator.SetBool("isThrowing", m_isThrowing);
            m_animator.SetBool("hasSpecialItem", m_hasSpecialItem);
            m_animator.SetBool("isDrinking", m_isDrinking);
            m_animator.SetBool("isHiding", m_isHiding);
            m_animator.SetBool("isClubBroken", m_isClubBroken); // CaveGirl
            m_animator.SetBool("isShieldActive", m_isShieldActive); // Knight
            m_animator.SetBool("isHoldingBox", m_isHoldingBox);
        }

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if(renderer != null)
            renderer.color = new Color(1f, 1f, 1f, (m_isHiding ? 0.6f : 1f));
    }

    private void UpdateItemCollectionAnim()
    {
        if(!m_playSpecialItemAnim) return;
        
        if(m_collectAnimation != null)
        {
            m_collectAnimation.up = Vector3.up;
            
            if(Time.time - m_specialItemCollectTime > m_specialItemAnimDuration)
            {
                m_specialItemCollectTime = 0f;
                m_playSpecialItemAnim = false;
            }
            
            m_collectAnimation.gameObject.SetActive(m_playSpecialItemAnim);
        }
    }
    #endregion

    #endregion

    #region Power Core Interaction
    public void addPowerCore()
    {
        if (currentCores < MAX_CORES)
            currentCores++;
    }

    public int CurrentCores
    {
        get { return currentCores; }
        set
        {
            if (value < 0)
                value = 0;

            currentCores = value;
        }
    }

    public bool isPowerCoreComplete()
    {
        return currentCores == MAX_CORES;
    }
    #endregion

    #region Sound Control
    public void PlayFootstep() {
        switch (m_floorType) {
            case 0:
                audio.clip = solidFoot[m_footCounter];
                break;
            case 1:
                audio.clip = grateFoot[m_footCounter];
                break;
        }
        audio.Play ();
        UpdateFootCounter ();
    }

    private void UpdateFootCounter(){
        m_footCounter++;
        if (m_footCounter >= 6)
            m_footCounter = 0;
    }

    public void HitFrameOn()
    {
        m_isAtHitFrame = true;
    }

    public void HitFrameOff()
    {
        m_isAtHitFrame = false;
    }

    // Cave Girl Method
    public bool IsAtHitFrame
    {
        get { return m_isAtHitFrame; }
    }
    #endregion

    #region Utilities
    public bool IsActive()
    {
        return m_isThrowing || m_isDrinking || m_isMoving || m_isAttacking;
    }

    private void ResetVariables(){
        inventory.clear();
        currentCores = 0;
        m_detectionLevel = 0;
    }
    #endregion

    #region Miscellaneous
    private void UpdateShield()
    {
        if(!m_isShieldActive) return;
        
        if(Time.time - m_shieldTime > m_shieldDuration)
        {
            m_isShieldActive = false;
            
            if(inventory.inventory[Inventory.SPECIAL_SLOT] == null)
                m_hasSpecialItem = false;
        }
    }
    #endregion

    public void Resurrect()
    {
        m_currentState = PlayerState.ALIVE;
    }
}
