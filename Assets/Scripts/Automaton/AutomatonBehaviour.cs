using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AutomatonBehaviour : MonoBehaviour
{
    AutomatonStates _state;
    Animator _ani;
    AudioSource _audio;
    NavMeshAgent _agent;
    float _travelCompleteThreshold = 0.001f;
    float _minWaitTime = 1;
    float _maxWaitTime = 3;
    [SerializeField]
    AudioClip[] _footStepClips;
    [SerializeField]
    Transform[] _wayPoints;
    int _wayPointIndex = 0;

    [SerializeField]
    Transform[] _dataInterfaceT;

    [SerializeField]
    bool _isStationary;

    private void Start()
    {
        _ani = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _audio = GetComponent<AudioSource>();
        StartCoroutine(Behaviour());
    }

    IEnumerator Behaviour()
    {
        while (true) 
        {
            EvaluateState();
            float waitTime = Random.Range(_minWaitTime, _maxWaitTime);

            switch (_state)
            {
                case AutomatonStates.IDLE:
                    Debug.Log("idling...");
                    yield return new WaitForSeconds(waitTime);
                    break;
                case AutomatonStates.WALK:
                    Debug.Log("walking...");

                    _wayPointIndex++;
                    if(_wayPointIndex >= _wayPoints.Length)
                        _wayPointIndex = 0;

                    Vector3 pos = _wayPoints[_wayPointIndex].position;
                    SetDestination(pos);

                    yield return new WaitForSeconds(0.1f);
                    yield return new WaitUntil(() => _agent.remainingDistance <= _travelCompleteThreshold);
                    _ani.SetFloat("Spd", 0f);
                    break;
                case AutomatonStates.SCAN:
                    Debug.Log("I talk");

                    int index = Random.Range(0, _dataInterfaceT.Length);
                    if (_dataInterfaceT[index] != null)      
                        SetDestination(_dataInterfaceT[index].position);

                    yield return new WaitForSeconds(0.1f);
                    yield return new WaitUntil(() => _agent.remainingDistance <= _travelCompleteThreshold);
                    _ani.SetFloat("Spd", 0f);
                    break;
                default:
                    Debug.Log("Automaton : Unable to get a state...");
                    break;
            }
        }
    }

    void SetDestination(Vector3 pos)
    {
        _agent.SetDestination(pos);
        _ani.SetFloat("Spd", 0.5f);
    }

    void EvaluateState()
    {
        //AAAAA.AAAAAA.O.MMM.MMMMMMMM.AAAAAAAAA?
        if (_isStationary)
        {
            _state = AutomatonStates.IDLE;
            return;
        }

        int nxtState = (int)_state + 1;
        if(nxtState == 3)
        {
            nxtState = 0;
        }
        _state = (AutomatonStates)nxtState;
    }

    void OnFootstep()
    {
        float vol = Random.Range(0.8f, 1f);
        int index = Random.Range(0,_footStepClips.Length);
        _audio.PlayOneShot(_footStepClips[index], vol);
    }

    enum AutomatonStates
    {
        IDLE,
        WALK,
        SCAN
    }
}
