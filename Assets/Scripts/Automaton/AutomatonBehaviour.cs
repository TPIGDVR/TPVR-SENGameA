using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutomatonBehaviour : MonoBehaviour
{
    AutomatonStates _state;
    Animator _ani;
    NavMeshAgent _agent;

    // Update is called once per frame
    void Update()
    {
        
    }

    void EvaluateState()
    {
        
    }

    void ExecuteBehaviour()
    {

    }

    void OnFootstep()
    {
        Debug.Log("Step");
    }

    enum AutomatonStates
    {
        IDLE,
        WALK,
        TALK
    }
}
