﻿using UnityEngine;
using System.Collections;

/**
 * This class triggers the ExitDoorFrame to begin exiting the level. 
 * If a Player  collides with the gameobject,
 * then trigger is started and exiting the level commences.
 * 
 * Interface Variables:
 *      There are no interface variables
 *      
 * Interface Methods
 *      There are no interface methods
 */
namespace Game
{
    public class EndOfLevelDetector : MonoBehaviour
    {
        #region Instance Methods
        protected void OnTriggerEnter2D(Collider2D collidee)
        {
            Player player = collidee.GetComponent<Player>();

            if (player != null)
                LevelManager.ExitLevel();
        }
        #endregion
    }
}
