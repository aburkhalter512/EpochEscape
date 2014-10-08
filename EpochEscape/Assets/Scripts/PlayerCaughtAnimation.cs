using UnityEngine;
using System.Collections;
using G = GameManager;

public class PlayerCaughtAnimation : MonoBehaviour
{
    public void Start()
    {
        transform.position = PlayerManager.GetPosition();
    }

    public void DoneAnimating()
    {
        LevelManager.LoadCheckpoint();

        Destroy(gameObject);
    }
}
