using UnityEngine;
using System.Collections;
using G = GameManager;

public class PlayerCaughtAnimation : MonoBehaviour
{
	public void DoneAnimating()
	{
		Application.LoadLevel(Application.loadedLevelName);

		G.getInstance().UnpauseMovement();
	}
}
