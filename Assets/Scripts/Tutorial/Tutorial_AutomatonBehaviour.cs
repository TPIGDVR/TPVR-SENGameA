using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Automaton
{
    public class Tutorial_AutomatonBehaviour : MonoBehaviour
    {
        AutomatonStates _state;
        Animator _ani;
        AudioSource _audio;
        NavMeshAgent _agent;
        float _travelCompleteThreshold = 0.001f;
        [SerializeField]
        AudioClip[] _footStepClips;
        [SerializeField]
        Waypoint[] _wayPoints;
        int _wayPointIndex = 0;
        public Vector3 targetDestination;

        public NavMeshAgent Agent { get => _agent; }

        Vector3 originalPosition;

        private void Start()
        {
            _ani = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _audio = GetComponent<AudioSource>();
            StartCoroutine(Behaviour());
            originalPosition = transform.position;  
        }

        IEnumerator Behaviour()
        {
            while (true)
            {
                //print($"{transform.parent.name} is at {_state}");
                switch (_state)
                {
                    case AutomatonStates.IDLE:
                        yield return null;
                        break;
                    case AutomatonStates.ROAM:
                        _wayPointIndex++;
                        if (_wayPointIndex >= _wayPoints.Length)
                            _wayPointIndex = 0;

                        Vector3 pos = _wayPoints[_wayPointIndex].Position;
                        yield return new WaitForSeconds(_wayPoints[_wayPointIndex].Delay);
                        SetDestination(pos);

                        yield return new WaitForSeconds(0.1f);
                        yield return new WaitUntil(() => Agent.remainingDistance <= _travelCompleteThreshold);
                        _ani.SetFloat("Spd", 0f);
                        break;
                    case AutomatonStates.WALK_TO_TARGET:
                        SetDestination(targetDestination);
                        yield return new WaitForSeconds(0.1f);
                        yield return new WaitUntil(() => Agent.remainingDistance <= _travelCompleteThreshold);
                        _ani.SetFloat("Spd", 0f);
                        SwitchToIdle();
                        break;
                    default:
                        Debug.Log("Automaton : Unable to get a state...");
                        break;
                }
            }
        }

        public void SwitchToIdle()
        {
            _state = AutomatonStates.IDLE;
        }

        void SwitchToRoam()
        {
            _state = AutomatonStates.ROAM;
        }

        public void SwitchToTarget()
        {
            _state = AutomatonStates.WALK_TO_TARGET;
        }

        void SetDestination(Vector3 pos)
        {
            Agent.SetDestination(pos);
            _ani.SetFloat("Spd", 0.5f);
        }

        void OnFootstep()
        {
            float vol = Random.Range(0.8f, 1f);
            int index = Random.Range(0, _footStepClips.Length);
            _audio.PlayOneShot(_footStepClips[index], vol);
        }

        public void SetAgentSpeed(float speed)
        {
            Agent.speed = speed;
        }

        public void ResetOriginalPostion()
        {
            _agent.Stop();
            transform.position = originalPosition;
            SwitchToIdle() ;

        }


        enum AutomatonStates
        {
            IDLE,
            ROAM,
            WALK_TO_TARGET,

        }
    }
}