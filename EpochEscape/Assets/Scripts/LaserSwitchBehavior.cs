﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserSwitchBehavior : MonoBehaviour {
    public List<DoorFrame> door;
    public List<doorSides> doorSideToActivate;
    public List<bool> toActivate; 
    public List<Color> colorMatch;
	public Color toMatch;

    public enum doorSides{
        Front, Back, Both
    }
    
    void Start () {
        //gameObject.GetComponent<SpriteRenderer> ().color = colorMatch;
		foreach (Color c in colorMatch) {
			toMatch = Utilities.addColors (toMatch, c);
		}
    }

    public void resetActivate(){
        //loop the doors that this switch is responsible for
        for(int i = 0; i < door.Count; i++){
            //check if activating or deactivating
            if(toActivate[i]){
                //check which sides to activate for that specific door
                switch (doorSideToActivate[i]) {
                case doorSides.Front:
                    door[i].deactivateSide (DoorFrame.SIDE.FRONT);
                    break;
                case doorSides.Back:
                    door[i].deactivateSide (DoorFrame.SIDE.BACK);
                    break;
                case doorSides.Both:
                    door[i].deactivateSides ();
                    break;
                }
            }
            else{
                switch (doorSideToActivate[i]) {
                case doorSides.Front:
                    door[i].activateSide (DoorFrame.SIDE.FRONT);
                    break;
                case doorSides.Back:
                    door[i].activateSide (DoorFrame.SIDE.BACK);
                    break;
                case doorSides.Both:
                    door[i].activateSides ();
                    break;
                }
            }
        }
    }

    public void Activate(Color color){
        //loop through doors this switch is responsible for
		if (Utilities.areEqualColors (color, toMatch)) {
			for (int i = 0; i < door.Count; i++) {
				//check if activating or deactivating
				if (toActivate [i]) {
						//check which sides to activate or deactivate
						switch (doorSideToActivate [i]) {
						case doorSides.Front:
								door [i].activateSide (DoorFrame.SIDE.FRONT);
								break;
						case doorSides.Back:
								door [i].activateSide (DoorFrame.SIDE.BACK);
								break;
						case doorSides.Both:
								door [i].activateSides ();
								break;
						}

				} else {
						switch (doorSideToActivate [i]) {
						case doorSides.Front:
								door [i].deactivateSide (DoorFrame.SIDE.FRONT);
								break;
						case doorSides.Back:
								door [i].deactivateSide (DoorFrame.SIDE.BACK);
								break;
						case doorSides.Both:
								door [i].deactivateSides ();
								break;
						}
				}
			}
		}
    }
}
	