using UnityEngine;
using System.Collections;
using G = GameManager;

public class PlayerCaughtAnimation : MonoBehaviour
{
    public void DoneAnimating()
    {
        LevelManager.LoadCheckpoint();

        Destroy(gameObject);
    }
}
