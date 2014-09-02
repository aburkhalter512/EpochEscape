using UnityEngine;
using System.Collections;

public class BackgroundAnimation : MonoBehaviour {
    public Animator animator = null;
    private bool afterStartup;
    private bool loopAfterStartup;
    Background currentState;
    private enum Background{
        Startup, Loop
    }



    // Use this for initialization
    void Start () {
        if(animator == null)
            animator = GetComponent<Animator>();
        afterStartup = false;
        loopAfterStartup = false;
        currentState = Background.Startup;
    }
    
    // Update is called once per frame
    void Update () {
        if(animator != null){
            animator.SetBool ("afterStartup", afterStartup);
            animator.SetBool("Loop", loopAfterStartup);
        }
        switch(currentState){
        case Background.Startup:
            break;
        case Background.Loop:
            loopAfterStartup = true;
            break;
        }
    }
}
