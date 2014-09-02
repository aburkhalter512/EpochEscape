using UnityEngine;
using System.Collections;
using G = GameManager;

public class PlayerCaughtAnimation : MonoBehaviour
{
    public LevelManager levelManager;

    public void DoneAnimating()
    {
        //levelManager.loadCheckpoint();

        Application.LoadLevel(Application.loadedLevelName);

        G.getInstance().UnpauseMovement();
    }
}
