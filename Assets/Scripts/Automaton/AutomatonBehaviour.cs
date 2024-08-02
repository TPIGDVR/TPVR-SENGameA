using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.AI;

public class AutomatonBehaviour : MonoBehaviour,IScriptLoadQueuer
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
    Waypoint[] _wayPoints;
    int _wayPointIndex = 0;

    [SerializeField]
    Transform[] _dataInterfaceT;

    [SerializeField]
    bool _isStationary;

    [Header("Movement")]
    [SerializeField]float oringinalMovement = 5f;
    [SerializeField] float acceptableDegree = 15f;
    [SerializeField] float rotationSpeed = 3f;

    #region INITIALIZATION
    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.AUTOMATONS);
    }

    public void Initialize()
    {
        _ani = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _audio = GetComponent<AudioSource>();
        StartCoroutine(Behaviour());
    }
    #endregion

    IEnumerator Behaviour()
    {
        while (true) 
        {
            EvaluateState();
            float waitTime = Random.Range(_minWaitTime, _maxWaitTime);

            switch (_state)
            {
                case AutomatonStates.IDLE:
                    yield return new WaitForSeconds(waitTime);
                    break;
                case AutomatonStates.WALK:
                    _wayPointIndex++;
                    if(_wayPointIndex >= _wayPoints.Length)
                        _wayPointIndex = 0;

                    Vector3 pos = _wayPoints[_wayPointIndex].Position;
                    yield return new WaitForSeconds(_wayPoints[_wayPointIndex].Delay);
                    SetDestination(pos);

                    yield return new WaitForSeconds(0.1f);
                    yield return MovementCoroutine();
                    _ani.SetFloat("Spd", 0f);
                    break;
                case AutomatonStates.SCAN:
                    if (_dataInterfaceT.Length <= 0)
                        break;
                    int index = Random.Range(0, _dataInterfaceT.Length);
                    if (_dataInterfaceT[index] != null)      
                        SetDestination(_dataInterfaceT[index].position);

                    yield return new WaitForSeconds(0.1f);
                    yield return new WaitUntil(() => _agent.remainingDistance <= _travelCompleteThreshold);
                    _ani.SetFloat("Spd", 0f);
                    break;
                case AutomatonStates.WALK_TO_TARGET:

                default:
                    Debug.Log("Automaton : Unable to get a state...");
                    break;
            }
        }
    }

    IEnumerator MovementCoroutine()
    {
        Vector3 previousPosition = transform.position;
        while(_agent.remainingDistance >= _travelCompleteThreshold)
        {
            //check 
            Vector3 nxtDirection = _agent.nextPosition - previousPosition;
            
            float degreeOfRotation = Vector3.SignedAngle(transform.forward, nxtDirection , transform.up);
            if(!(degreeOfRotation < acceptableDegree && 
                degreeOfRotation > -acceptableDegree))
                //if not within acceptable degree.
            {
                yield return RotateKyle(degreeOfRotation, nxtDirection, transform.forward);
            }
            else
            {
                _ani.SetFloat("Spd", 0.5f);
                _agent.speed = oringinalMovement;
                
            }
            //print($"rotation;{degreeOfRotation}, forward {transform.forward}, nxtDirection {nxtDirection} \n" +
            //    $"_agent nxt Position {_agent.nextPosition}, transform position {transform.position}");
            previousPosition = transform.position;
            yield return null;
        }
    }

    IEnumerator RotateKyle(float degree, Vector3 targetDirection, Vector3 currentDirection)
    {
        _agent.speed = 0;
        float strength = degree / 180;
        
        _ani.SetFloat("RotationStrength", strength);
        _ani.SetFloat("Spd", 0f);

        _ani.SetTrigger("Rotate");
        targetDirection.Normalize();
        currentDirection.Normalize();
        yield return new WaitUntil(() => _ani.GetCurrentAnimatorStateInfo(0).IsName("Rotation blend tree"));

        while (_ani.GetCurrentAnimatorStateInfo(0).IsName("Rotation blend tree"))
        {
            Debug.DrawRay(transform.position, targetDirection * 50, Color.blue);
            float normalizeTime = _ani.GetCurrentAnimatorStateInfo(0).normalizedTime;
            transform.forward = Vector3.Slerp(currentDirection, targetDirection, normalizeTime);
            yield return null;
        }
    }

    public void SetDestination(Vector3 pos)
    {
        _agent.SetDestination(pos);
        _agent.speed = 0;

        //_ani.SetFloat("Spd", 0.5f);
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
        SCAN,
        WALK_TO_TARGET,
    }
}
