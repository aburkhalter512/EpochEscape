using UnityEngine;
using System.Collections;

public class CoroutinePractice : MonoBehaviour
{
	public GUIText m_statusText = null;

	public void Start()
	{
		StartCoroutine(Computation());
	}

	private IEnumerator Computation()
	{
		PrintStatusMessage("Please wait...");

		yield return new WaitForSeconds(5);

		PrintStatusMessage("Done");
	}

	private void PrintStatusMessage(string message)
	{
		if(m_statusText != null)
			m_statusText.text = message;
	}
}
