using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Animator animator;
    string currentState;
    const string PLAYER_IDLE = "Idle_Up";
    const string PLAYER_WALK = "^_Walk";




    void ChangeState(string newState)
    {
        if (newState == currentState)
            return;
        animator.Play(newState);
        currentState = newState;
    }


    bool isAnimationPlaying(Animator animator, string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }
}
