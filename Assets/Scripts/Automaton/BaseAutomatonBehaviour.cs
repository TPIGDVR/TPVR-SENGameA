using System.Collections;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

namespace Automaton
{
    public class BaseAutomatonBehaviour : MonoBehaviour, IScriptLoadQueuer
    {
        [SerializeField] protected AutomatonStates _state;
        protected Animator _ani;
        protected AudioSource _audio;
        private NavMeshAgent _agent;
        private float _travelCompleteThreshold = 0.001f;
        private float _minWaitTime = 1;
        private float _maxWaitTime = 3;
        [SerializeField]
        AudioClip[] _footStepClips;
        [SerializeField]
        Waypoint[] _wayPoints;
        int _wayPointIndex = 0;

        [SerializeField]
        bool _isStationary;

        [SerializeField] protected Transform[] _dataInterfaceT;
        private Vector3 targetDestination;
        [Header("Movement")]
        [SerializeField] private float oringinalMovement = 5f;
        [SerializeField] private float acceptableDegree = 15f;
        [SerializeField] private float rotationSpeed = 3f;

        Coroutine currentBehaviourCoroutine;
        public NavMeshAgent Agent { get => _agent; set => _agent = value; }

        public void SetHide()
        {
            StopAllCoroutines();
            //stop them from moving
            _ani.SetFloat("Spd", 0);
            //then stop all the coroutine
            currentBehaviourCoroutine = null;
        }

        public void SetShow()
        {
            enabled = true;
            if(currentBehaviourCoroutine != null)
            {
                StopCoroutine(currentBehaviourCoroutine);
            }
            currentBehaviourCoroutine = StartCoroutine(Behaviour());
        }

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
            InitBehaviour();
            currentBehaviourCoroutine = StartCoroutine(Behaviour());

            _agent.updatePosition = false;
            _agent.speed = oringinalMovement;
        }
        #endregion

        //placeholder coroutine for basic behaviours
        protected virtual IEnumerator Behaviour()
        {
            while (true)
            {
                //print($"Coroutine Running is {transform.parent.name}");
                EvaluateState();
                if (_isStationary) yield return null;
                switch (_state)
                {
                    case AutomatonStates.IDLE:
                        float waitTime = Random.Range(_minWaitTime, _maxWaitTime);
                        yield return new WaitForSeconds(waitTime);
                        break;
                    case AutomatonStates.WALK:
                        yield return WalkToWayPointCoroutine();
                        break;
                    case AutomatonStates.SCAN:
                        yield return WalkToTPointCoroutine();
                        break;
                    case AutomatonStates.WALK_TO_TARGET:
                        yield return MovementCoroutine(targetDestination);
                        break;
                    //add other behaviour
                    default:
                        Debug.Log("Automaton : Unable to get a state...");
                        break;
                }
            }
        }

        /// <summary>
        /// Basic walk coroutine to move the automaton to a selected waypoint
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator WalkToTPointCoroutine()
        {
            Vector3 destination = Vector3.zero;
            if (_dataInterfaceT.Length <= 0) yield break;
            int index = Random.Range(0, _dataInterfaceT.Length);
            if (_dataInterfaceT[index] != null)
                destination = _dataInterfaceT[index].position;
            yield return MovementCoroutine(destination);
        }
        protected virtual IEnumerator WalkToWayPointCoroutine()
        {
            _wayPointIndex++;
            if (_wayPointIndex >= _wayPoints.Length)
                _wayPointIndex = 0;

            Vector3 pos = _wayPoints[_wayPointIndex].Position;
            yield return new WaitForSeconds(_wayPoints[_wayPointIndex].Delay);
            yield return MovementCoroutine(pos);
        }
        
        #region walking

        protected IEnumerator MovementCoroutine(Vector3 destination)
        {
            _agent.SetDestination(destination);

            yield return new WaitForSeconds(0.1f);

            Vector3 previousPosition = transform.position;
            while (_agent.remainingDistance >= _travelCompleteThreshold)
            {
                Vector3 nxtDirection = _agent.nextPosition - previousPosition;
                nxtDirection.y = 0f;
                float degreeOfRotation = Vector3.SignedAngle(transform.forward, nxtDirection, transform.up);
                if (!(degreeOfRotation < acceptableDegree &&
                    degreeOfRotation > -acceptableDegree))
                //if not within acceptable degree.
                {
                    yield return RotateKyle(degreeOfRotation, nxtDirection, transform.forward);
                }
                else
                {
                    _ani.SetFloat("Spd", 0.5f);
                    _agent.speed = oringinalMovement;
                    previousPosition = transform.position;
                    transform.position = _agent.nextPosition;
                }

                yield return null;
            }
            //print("Finish way point");
            _ani.SetFloat("Spd", 0f);
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

            targetDestination.y = 0;

            Quaternion finalRotation = Quaternion.LookRotation(targetDirection);
            Quaternion initialRotation = transform.rotation;

            yield return new WaitUntil(() => _ani.GetCurrentAnimatorStateInfo(0).IsName("Rotation blend tree"));

            while (_ani.GetCurrentAnimatorStateInfo(0).IsName("Rotation blend tree"))
            {
                float normalizeTime = _ani.GetCurrentAnimatorStateInfo(0).normalizedTime;
                transform.rotation = Quaternion.Slerp(initialRotation , finalRotation , normalizeTime); 
                yield return null;
            }

            transform.rotation = finalRotation;
        }
        #endregion
        /// <summary>
        /// Change the evaluate state for other automaton states
        /// </summary>
        protected virtual void EvaluateState()
        {
            if (_isStationary)
            {
                _state = AutomatonStates.IDLE;
                return;
            }

            int nxtState = (int)_state + 1;
            if (nxtState == 3)
            {
                nxtState = 0;
            }
            _state = (AutomatonStates)nxtState;
        }

        protected virtual void InitBehaviour()
        {
            //NOOP
        }
        public void ChangeState(AutomatonStates stateToChange)
        {
            _state = stateToChange;
            StopAllCoroutines();
            if(currentBehaviourCoroutine != null)
            {
                StopCoroutine(currentBehaviourCoroutine);
            }
            currentBehaviourCoroutine = StartCoroutine(Behaviour());
        }

        public enum AutomatonStates
        {
            IDLE,
            WALK,
            SCAN,
            WALK_TO_TARGET,
        }

        [ContextMenu("Change to walk")]
        public void ChangeToWalkToWayPoint()
        {
            ChangeState(AutomatonStates.WALK);
        }

        [ContextMenu("Change to Idle")]
        public void ChangeToIdle()
        {
            ChangeState(AutomatonStates.IDLE);
        }

        public void ChangeToMoveTarget(Vector3 targetDestination)
        {
            this.targetDestination = targetDestination;
            ChangeState(AutomatonStates.WALK_TO_TARGET);
        }

        #region misc functions
        //called in the animator
        void OnFootstep()
        {
            float vol = Random.Range(0.8f, 1f);
            int index = Random.Range(0, _footStepClips.Length);
            _audio.PlayOneShot(_footStepClips[index], vol);
        }
        #endregion
    }
}