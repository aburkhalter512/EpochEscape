using UnityEngine;
using System.Collections;

/*
 * A specialized Pressure Plate that can only be activated once.
 */
namespace Game
{
    public class PressureSwitch : PressurePlate
    {
        /*
         * If the collidee is the player, and the Pressure Switch has not been
         * activated previously, then it is activated.
         */
        override protected void OnTriggerEnter2D(Collider2D collidee)
        {
            if (collidee.tag == "Player" && currentState != STATE.OFF)
                base.OnTriggerEnter2D(collidee);
        }
    }
}
