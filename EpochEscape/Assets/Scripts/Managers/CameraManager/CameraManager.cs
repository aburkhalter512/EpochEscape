using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : Manager<CameraManager>
{
    private Camera m_camera = null;

    private Vector3 m_idlePosition = Vector3.zero;

    protected override void Initialize()
    {
        InitializeCamera();
    }

    protected void Update()
    {
        _SetIdlePosition(Player.Get().transform.position);
        _UpdateIdlePosition();
    }

    #region Interface Methods
    public static void SetPosition(Vector3 position)
    {
        CameraManager.Get()._SetPosition(position);
    }
    #endregion

    #region Instance Methods
    private void _SetIdlePosition(Vector3 position)
    {
        m_idlePosition = new Vector3(position.x, position.y, transform.position.z);
    }

    private void _UpdateIdlePosition()
    {
        transform.position = Vector3.Lerp(transform.position, m_idlePosition, 2f * Time.smoothDeltaTime);
    }

    private bool InitializeCamera()
    {
        m_camera = GetComponent<Camera>();

        if(m_camera == null)
            return false;

        return true;
    }

    private void _SetPosition(Vector3 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
    #endregion
}
