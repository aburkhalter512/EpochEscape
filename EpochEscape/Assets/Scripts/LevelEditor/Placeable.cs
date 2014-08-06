using UnityEngine;
using System.Collections;

public class Placeable : MonoBehaviour
{
	private bool m_isPlacing = false;
	private Vector3 m_mousePosition = Vector3.zero;
	private Vector3 m_position = Vector3.zero;

	public void Start()
	{
		TempLevelEditor.SetSelectedObject(gameObject);
	}

	public void Update()
	{
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		if(!m_isPlacing)
			return;

		m_mousePosition.x = Input.mousePosition.x;
		m_mousePosition.y = Input.mousePosition.y;
		m_mousePosition.z = -Camera.main.transform.position.z;

		m_mousePosition = Camera.main.ScreenToWorldPoint(m_mousePosition);

		m_position.x = m_mousePosition.x;
		m_position.y = m_mousePosition.y;

		transform.position = m_position;
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
		TempLevelEditor.SetSelectedObject(gameObject);
	}
}
