using UnityEngine;
using System.Collections;

public class MiniMapManager : Manager<MiniMapManager>
{
	#region Interface Variables
	#endregion

	#region Instance Variables
    private bool m_isInitialized = false;
    private Camera m_camera = null;
	#endregion

    protected override void Initialize()
    {
        m_isInitialized = InitializeCamera();

        _Hide();
    }

	#region Interface Methods
    public static void Hide()
    {
        MiniMapManager.GetInstance()._Hide();
    }

    public static void Show()
    {
        MiniMapManager.GetInstance()._Show();
    }

    public static void SetChamber()
    {
        MiniMapManager.GetInstance()._SetChamber();
    }
	#endregion

	#region Instance Methods
    private bool InitializeCamera()
    {
        m_camera = GetComponent<Camera>();

        if (m_camera == null)
            return false;

        return true;
    }

    private void _Hide()
    {
        gameObject.SetActive(false);
    }

    private void _Show()
    {
        gameObject.SetActive(true);
    }

    private void _SetChamber()
    {
        Chamber currentChamber = LevelManager.GetCurrentChamber();

        if (currentChamber != null)
        {
            Vector2 currentChamberPosition = currentChamber.GetPosition();
            Vector2 currentChamberSize = currentChamber.GetSize();
            float offset = 0f;

            if (currentChamberSize.x > currentChamberSize.y)
            {
                m_camera.orthographicSize = (currentChamberSize.x / m_camera.aspect) / 2;

                offset = (m_camera.orthographicSize * 2 - currentChamberSize.y) / 2;

                transform.position = new Vector3(currentChamberPosition.x, currentChamberPosition.y - offset, transform.position.z);
            }
            else
            {
                m_camera.orthographicSize = currentChamberSize.y / 2;

                offset = ((currentChamberSize.y * m_camera.aspect) - currentChamberSize.x) / 2;

                transform.position = new Vector3(currentChamberPosition.x - offset, currentChamberPosition.y, transform.position.z);
            }
        }
    }
	#endregion
}
