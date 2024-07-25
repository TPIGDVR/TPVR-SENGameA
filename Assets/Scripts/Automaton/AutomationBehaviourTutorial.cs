using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Automaton
{
    public class AutomationBehaviourTutorial : MonoBehaviour
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
                        yield return new WaitUntil(() => _agent.remainingDistance <= _travelCompleteThreshold);
                        _ani.SetFloat("Spd", 0f);
                        break;
                    case AutomatonStates.WALK_TO_TARGET:
                        SetDestination(targetDestination);
                        yield return new WaitForSeconds(0.1f);
                        yield return new WaitUntil(() => _agent.remainingDistance <= _travelCompleteThreshold);
                        _ani.SetFloat("Spd", 0f);
                        SwitchToIdle();
                        break;
                    default:
                        Debug.Log("Automaton : Unable to get a state...");
                        break;
                }
            }
        }

        void SwitchToIdle()
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
            _agent.SetDestination(pos);
            _ani.SetFloat("Spd", 0.5f);
        }

        void OnFootstep()
        {
            float vol = Random.Range(0.8f, 1f);
            int index = Random.Range(0, _footStepClips.Length);
            _audio.PlayOneShot(_footStepClips[index], vol);
        }

        enum AutomatonStates
        {
            IDLE,
            ROAM,
            WALK_TO_TARGET,

        }
    }
}