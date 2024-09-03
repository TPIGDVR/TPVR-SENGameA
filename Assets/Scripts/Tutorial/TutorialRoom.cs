using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    public class TutorialRoom : Room
    {
        [SerializeField]
        TutorialGameOver gameOver;
        bool hasActivateHeartRateMonitor = false;
        bool hasActivateNoiseIndicatorDetection = false;
        
        protected override void InitRoom()
        {
            GameData.ChangeTutorialStatus(true);
            DisplayRoomObjective();
            EventSystem.dialog.AddListener(DialogEvents.ACTIVATE_HEARTRATE, ActivatedHeartRateMonitor , DetermineMechanicActivation);
            EventSystem.dialog.AddListener(DialogEvents.ACTIVATE_NOISE_INDICATOR, ActivateNoiseIndicatorDetection);
            EventSystem.dialog.AddListener(DialogEvents.COMPLETED_TUTORIAL_DIALOG, DetermineMechanicActivation);
            
        }

        void ActivatedHeartRateMonitor()
        {
            hasActivateHeartRateMonitor = true;
            //remove the event
            EventSystem.dialog.RemoveListener(DialogEvents.ACTIVATE_HEARTRATE, ActivatedHeartRateMonitor);

            //player anxiety will increase
            GameData.ChangeTutorialStatus(false);
        }

        void ActivateNoiseIndicatorDetection()
        {
            hasActivateNoiseIndicatorDetection = true;
            //remove the event
            EventSystem.dialog.RemoveListener(DialogEvents.ACTIVATE_NOISE_INDICATOR, ActivateNoiseIndicatorDetection);
        }

        void DetermineMechanicActivation()
        {
            foreach (var obj in roomObj_rt)
            {
                if (obj.Name == ObjectiveName.KIOSK)
                {
                    ActivateMissMechanic(obj);
                }
            }

            void ActivateMissMechanic(Objective obj)
            {
                print("running missing mechanic");
                switch (obj.Completed)
                {
                    //if its the third kiosk and the player has not activate it, then activate heart beat monitor
                    case 3:
                        print($"hello running this here {hasActivateNoiseIndicatorDetection}");
                        if (!hasActivateNoiseIndicatorDetection)
                        {
                            EventSystem.dialog.TriggerEvent(DialogEvents.ACTIVATE_NOISE_INDICATOR);
                        }
                        break;
                    //if its the fourth kiosk, then activate the mechanic that is not activated.
                    case 4:
                        print("Checking last kiosk");
                        if (!hasActivateHeartRateMonitor)
                        {
                            EventSystem.dialog.TriggerEvent(DialogEvents.ACTIVATE_HEARTRATE);
                        }
                        if (!hasActivateNoiseIndicatorDetection)
                        {
                            EventSystem.dialog.TriggerEvent(DialogEvents.ACTIVATE_NOISE_INDICATOR);
                        }
                        EventSystem.dialog.RemoveListener(DialogEvents.COMPLETED_TUTORIAL_DIALOG, DetermineMechanicActivation);
                        break;
                }
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            //adding listeners

            //trigger event
            EventSystem.level.TriggerEvent(LevelEvents.INIT_TUTORIAL);
        }

        protected override void EvaluateObjective()
        {
            foreach (var obj in roomObj_rt) 
            { 
                if(obj.Name == ObjectiveName.KIOSK)
                {
                    DetermineKioskEvent(obj);
                }
            }
        }

        void DetermineKioskEvent(Objective objective)
        {
            if(objective.Completed == 1)
            {
                var kiosk = kiosks.FirstOrDefault(kiosk => kiosk.ScanCompleted);
                EventSystem.level.TriggerEvent<Transform>(LevelEvents.FIRST_KIOSK, kiosk.AutomatonTargetDestination);
            }
            else if(objective.Completed == 4)
            {
                EventSystem.level.TriggerEvent(LevelEvents.FINISH_TUTORIAL);
            }
        }
    }
}