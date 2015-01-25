using UnityEngine;
using System.Collections;

public class Placeable : MonoBehaviour
{
    private bool m_isPlacing = false;

    public void Start()
    {
        LevelEditor.SetSelectedObject(gameObject);
    }

    public void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if(!m_isPlacing)
            return;

        LevelEditor.SnapToGrid(gameObject);
    }

    public void OnMouseDrag()
    {
        m_isPlacing = true;
    }

    public void OnMouseUp()
    {
        m_isPlacing = false;
    }

    public void OnMouseDown()
    {
        LevelEditor.SetSelectedObject(gameObject);
    }
}
