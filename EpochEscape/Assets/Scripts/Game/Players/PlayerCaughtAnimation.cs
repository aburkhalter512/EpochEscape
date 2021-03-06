﻿using UnityEngine;
using System.Collections;

namespace Game
{
    public class PlayerCaughtAnimation : MonoBehaviour
    {
        public void Start()
        {
            transform.position = Player.Get().transform.position;
        }

        public void DoneAnimating()
        {
            LevelManager.LoadCheckpoint();

            Destroy(gameObject);
        }
    }
}
