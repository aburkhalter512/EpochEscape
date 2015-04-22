using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : Manager<CameraManager>
{
	#region Instance Variables
    private Vector3 m_idlePosition = Vector3.zero;
	private CAM_TYPE _type;

    private InputManager IM = null;

    bool mIsCameraMoving = false;

    Camera m_camera;
	#endregion
	
    #region Class Constants
    public const float CAMERA_MOVE_SPEED = 0.25f;
    public const float CAMERA_ZOOM_MIN_SIZE = 1.5f;
    public const float CAMERA_ZOOM_MAX_SIZE = 10f;
    public const float CAMERA_ZOOM_SCALE_FACTOR = 4f;
    #endregion
	
	public enum CAM_TYPE
	{
		GAME,
		EDITOR
	}

	protected override void Awaken()
	{
		_type = CAM_TYPE.GAME;
        m_camera = this.camera;
	}
	
    protected override void Initialize()
    {
        IM = InputManager.Get();
    }

    protected void Update()
    {
		switch (_type)
		{
			case CAM_TYPE.GAME:
				idlePosition(Player.Get().transform.position);
				updatePosition();
				break;
			case CAM_TYPE.EDITOR:
				updateControlledMovement();
				updateZoom();
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
        if (IM.cameraMoveButton.get())
        {
            mIsCameraMoving = true;
			
			Vector3 v = IM.mouse.inWorld();
			v.z = 0;

			transform.position = v;
		}
        else
        {
            mIsCameraMoving = false;
        }
        
    }

    private void updateZoom()
    {
        float intensity = IM.cameraZoom.get();

        if (!Utilities.IsApproximately(intensity, 0f))
        {
			float zoomDir = Mathf.Sign(intensity);
			
            if (IM.cameraZoomLeftModifier.get() || IM.cameraZoomRightModifier.get())
            {
                if ((intensity > 0 && m_camera.orthographicSize > CAMERA_ZOOM_MIN_SIZE) || 
					(intensity < 0 && m_camera.orthographicSize < CAMERA_ZOOM_MAX_SIZE))
                {
					
					float scaleFactor = m_camera.orthographicSize * CAMERA_ZOOM_SCALE_FACTOR;
					Vector3 mouseDelta = (IM.mouse.inWorld() - transform.position) / scaleFactor;
						
					Vector3 pos = transform.position + 
						zoomDir * mouseDelta * Time.smoothDeltaTime;
					pos.z = 0f;

                    transform.position = pos;
                }
            }

            m_camera.orthographicSize = m_camera.orthographicSize + (-zoomDir) * (1 / CAMERA_ZOOM_SCALE_FACTOR);
            m_camera.orthographicSize = Mathf.Clamp(m_camera.orthographicSize, CAMERA_ZOOM_MIN_SIZE, CAMERA_ZOOM_MAX_SIZE);
        }
    }
    #endregion
}
