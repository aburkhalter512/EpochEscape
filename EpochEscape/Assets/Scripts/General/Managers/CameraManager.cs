using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : Manager<CameraManager>
{
    #region Instance Variables
    private bool _isControlled = false;
    private Vector3 m_idlePosition = Vector3.zero;

    private Vector3 _basePos;
    private CAM_TYPE _type;

    private MapEditor.InputManager IM = null;

    Camera m_camera;
    #endregion

    #region Class Constants
    public const float CAMERA_MOVE_SPEED = 0.25f;
    public const float CAMERA_ZOOM_MIN_SIZE = 1.5f;
    public const float CAMERA_ZOOM_MAX_SIZE = 10f;
    public const float CAMERA_ZOOM_SCALE_FACTOR = 8f;
    #endregion

    public enum CAM_TYPE
    {
        GAME,
        EDITOR,
        MAIN_MENU,
    }

    protected override void Awaken()
    {
        _type = CAM_TYPE.MAIN_MENU;
        m_camera = this.GetComponent<Camera>();
    }

    protected override void Initialize()
    {
        IM = MapEditor.InputManager.Get();
    }

    protected void Update()
    {
        switch (_type)
        {
            case CAM_TYPE.GAME:
                idlePosition(Game.Player.Get().transform.position);
                updatePosition();
                break;
            case CAM_TYPE.EDITOR:
                updateControlledMovement();
                updateZoom();
                break;
           	case CAM_TYPE.MAIN_MENU:
           		break;

        }
    }

    #region Interface Methods
    public void setPosition(Vector3 v)
    {
        transform.position = new Vector3(v.x, v.y, transform.position.z);
    }

    public void camType(CAM_TYPE type)
    {
        _type = type;
    }
    #endregion

    #region Instance Methods
    private void idlePosition(Vector3 position)
    {
        m_idlePosition = new Vector3(position.x, position.y, transform.position.z);
    }

    private void updatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, m_idlePosition, 2f * Time.smoothDeltaTime);
    }

    private void updateControlledMovement()
    {
        if (IM.cameraJoystick.getRaw().sqrMagnitude > 0.25f)
        {
            if (!_isControlled)
            {
                _isControlled = true;
                m_idlePosition = IM.mouse.inWorld();
            }
            else
            {
                Vector3 v = (IM.mouse.inWorld() - m_idlePosition);
                v.z = 0;

                transform.position -= v;
            }
        }
        else
            m_idlePosition = Vector3.zero;

    }

    private void updateZoom()
    {
        float intensity = IM.cameraZoomNormal.get();

        if (!Utilities.Math.IsApproximately(intensity, 0f))
        {
            float zoomDir = Mathf.Sign(intensity);

            /*if (IM.cameraZoomLeftModifier.get() || IM.cameraZoomRightModifier.get())
            {
                if ((intensity > 0 && m_camera.orthographicSize > CAMERA_ZOOM_MIN_SIZE) || 
                    (intensity < 0 && m_camera.orthographicSize < CAMERA_ZOOM_MAX_SIZE))
                {
                    float scaleFactor = m_camera.orthographicSize * CAMERA_ZOOM_SCALE_FACTOR;
                    Vector3 mouseDelta = (IM.mouse.inWorld() - transform.position) / scaleFactor;
						
                    Vector3 pos = IM.mouse.inWorld() - transform.position +
                        zoomDir * mouseDelta;
                    pos.z = -10f;

                    transform.position = pos;
                }
            }*/

            m_camera.orthographicSize = m_camera.orthographicSize + Mathf.Sign(-intensity) * CAMERA_ZOOM_SCALE_FACTOR * Time.smoothDeltaTime;
            m_camera.orthographicSize = Mathf.Clamp(m_camera.orthographicSize, CAMERA_ZOOM_MIN_SIZE, CAMERA_ZOOM_MAX_SIZE);
        }
    }
    #endregion
}
