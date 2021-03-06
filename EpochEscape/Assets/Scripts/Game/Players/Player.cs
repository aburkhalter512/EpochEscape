using UnityEngine;
using System.Collections;
using G = Game.GameManager;

namespace Game
{
    public class Player : Manager<Player>
    {
        #region Interface Variables
        public AudioClip[] grateFoot;
        public AudioClip[] solidFoot;

        public int m_floorType; // 0 = solid, 1 = gratex
        #endregion

        #region Instance Variables
        Animator m_animator;

        #region Movement Variables
        bool m_isMoving;
        Vector2 mMovementDirection = Vector2.zero;

        float m_rotationSpeed = 15f;
        #endregion

        #region Audio Variables
        int m_footCounter;
        #endregion

        #endregion

        #region Instance Variables
        protected bool mIsPaused = false;

        protected InputManager mIM;

        protected float m_detectionLevel = 0.0f;

        //Detection Variables
        protected bool m_isDetected;
        protected float m_detectionRate = 50f; // rate/second at which character becomes detected by cameras
        protected float m_detectionFade = 1.5f;
        protected float mDetectionMin = 0.0f;

        //Power Core Variables
        protected int mCollectedCores = 0;

        PlayerState m_currentState;
        #endregion

        #region Class Constants
        public static readonly float SPEED = 2f;

        public static readonly float MAX_DETECTION_LEVEL = 100.0f;

        public static readonly int MAX_CORES = 3;

        public static readonly float INTERACTION_DISTANCE = 0.2f;
        public static readonly int INTERACTION_LAYER_MAX = 1 << 9;

        public enum PlayerState
        {
            ALIVE,
            DEAD
        }
        #endregion

        protected override void Awaken()
        {
        }

        protected override void Initialize()
        {
            m_floorType = 0;
            m_isMoving = false;

            m_isDetected = false;
            m_currentState = PlayerState.ALIVE;

            m_animator = GetComponent<Animator>();

            mIM = InputManager.Get();
        }

        protected void Update()
        {
            if (!G.Get().paused)
                UpdateCurrentState();
            else
                m_isMoving = false;

            UpdateAnimator();
        }

        #region Interface Methods
        public void load(Vector3 position, Vector3 rotation)
        {
            return;
        }

        public void hide()
        {
            gameObject.SetActive(false);
        }
        public void show()
        {
            gameObject.SetActive(true);
        }

        public void Resurrect()
        {
            m_currentState = PlayerState.ALIVE;
        }

        public void addCore()
        {
            mCollectedCores++;

            if (mCollectedCores > MAX_CORES)
                mCollectedCores = MAX_CORES;
        }
        public void removeCore()
        {
            mCollectedCores--;

            if (mCollectedCores < 0)
                mCollectedCores = 0;
        }
        public int getCurrentCores()
        {
            return mCollectedCores;
        }
        public void clearCores()
        {
            mCollectedCores = 0;
        }

        // Returns a value between 0 and 1
        public float getCurrentDetection()
        {
            return m_detectionLevel / MAX_DETECTION_LEVEL;
        }
        public void detect()
        {
            m_isDetected = true;
        }

        public void pause()
        {
            mIsPaused = true;
        }
        public void unpause()
        {
            mIsPaused = false;
        }
        #endregion

        #region Instance Methods
        private void UpdateUserControl()
        {
            if (mIsPaused)
                return;

            m_isMoving = false;

            Vector3 playerMovement = mIM.primaryJoystick.getRaw();

            //Vertical Movement
            mMovementDirection = Vector2.zero;

            if (Utilities.Math.IsApproximately(playerMovement.y, 0.0f))
                mMovementDirection.y = 0.0f;
            else if (playerMovement.y > 0.0f)
            {
                m_isMoving = true;
                mMovementDirection.y = 1.0f;
            }
            else if (playerMovement.y < 0.0f)
            {
                m_isMoving = true;
                mMovementDirection.y = -1.0f;
            }

            //Horizontal Movement
            if (Utilities.Math.IsApproximately(playerMovement.x, 0.0f))
                mMovementDirection.x = 0.0f;
            else if (playerMovement.x > 0.0f)
            {
                m_isMoving = true;
                mMovementDirection.x = 1.0f;
            }
            else if (playerMovement.x < 0.0f)
            {
                m_isMoving = true;
                mMovementDirection.x = -1.0f;
            }

            mMovementDirection.Normalize();

            if (mIM.interactButton.getDown())
                interact();
        }
        private void UpdateMovement()
        {
            if (!m_isMoving) return;

            transform.up = Vector3.Lerp(transform.up, mMovementDirection, m_rotationSpeed * Time.smoothDeltaTime);
            transform.position += (Utilities.Math.toVector3(mMovementDirection) * SPEED * Time.smoothDeltaTime);
        }
        private void UpdateDetection()
        {
            if (m_isDetected)
            {
                m_detectionLevel += m_detectionRate * Time.deltaTime;

                if (m_detectionLevel >= MAX_DETECTION_LEVEL)
                {
                    m_currentState = PlayerState.DEAD;
                    m_detectionLevel = 0;
                }
                else if (m_detectionLevel / 1.5f > mDetectionMin)
                    mDetectionMin = m_detectionLevel / 1.5f;
            }
            else if (m_detectionLevel > mDetectionMin)
            {
                m_detectionLevel -= m_detectionRate / 10 * Time.smoothDeltaTime;

                if (m_detectionLevel < mDetectionMin)
                    m_detectionLevel = mDetectionMin;
            }

            m_isDetected = false;
        }
        private void UpdateAnimator()
        {
            m_animator.SetBool("isMoving", m_isMoving);
        }
        private void UpdateCurrentState()
        {
            switch (m_currentState)
            {
                case PlayerState.ALIVE:
                    Alive();
                    break;

                case PlayerState.DEAD:
                    Dead();
                    break;
            }
        }
        private void PlayFootstep()
        {
            switch (m_floorType)
            {
                case 0:
                    if (solidFoot.Length > 0)
                    {
                        GetComponent<AudioSource>().clip = solidFoot[m_footCounter % solidFoot.Length];
                        m_footCounter = (m_footCounter + 1) % solidFoot.Length;
                    }
                    break;
                case 1:
                    if (solidFoot.Length > 0)
                    {
                        GetComponent<AudioSource>().clip = grateFoot[m_footCounter % grateFoot.Length];
                        m_footCounter = (m_footCounter + 1) % grateFoot.Length;
                    }
                    break;
            }
            GetComponent<AudioSource>().Play();
        }

        private void Alive()
        {
            UpdateUserControl();
            UpdateDetection();
            UpdateMovement();
        }
        private void Dead()
        {
            G.Get().PauseMovement();

            HUDManager.Hide();

            GameObject playerCaught = Resources.Load("Prefabs/PlayerCaught") as GameObject;

            if (playerCaught != null)
            {
                playerCaught = Instantiate(playerCaught) as GameObject;
                playerCaught.transform.position = transform.position;
            }
        }

        private void interact()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, INTERACTION_DISTANCE, INTERACTION_LAYER_MAX);

            if (hit.collider != null)
            {
                IInteractable interactor = hit.collider.GetComponent<MonoBehaviour>() as IInteractable;

                if (interactor != null)
                    interactor.Interact();
            }
        }
        #endregion
    }
}
