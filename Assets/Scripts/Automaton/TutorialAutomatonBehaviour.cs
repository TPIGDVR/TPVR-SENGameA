using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Automaton
{
    public class TutorialAutomatonBehaviour : BaseAutomatonBehaviour
    {
        protected override void EvaluateState()
        {
            //we want to control it base on event.
        }

        protected override void StartBehaviour()
        {
            //we want the tutorial bot be in idle.
            _state = AutomatonStates.IDLE;
            //subscribe to the event
            SubscibeToEvent();

        }

        void SubscibeToEvent()
        {
            EventSystem.level.AddListener<Transform>(LevelEvents.FIRST_KIOSK, MoveToKiosk);
        }

        void MoveToKiosk(Transform targetPosition)
        {
            //print($"{name} is moving to {targetPosition.position}");
            ChangeToMoveTarget(targetPosition.position);
            EventSystem.level.RemoveListener<Transform>(LevelEvents.FIRST_KIOSK, MoveToKiosk);
        }


    }
}