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
            EventSystem.dialog.AddListener(DialogEvents.ACTIVATE_HEARTRATE, ActivatedHeartRateMonitor);
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
            EventSystem.player.AddListener(PlayerEvents.DEATH_SCREEN_FADED, poop);

            //trigger event
            EventSystem.level.TriggerEvent(LevelEvents.INIT_TUTORIAL);
        }

        public override void OnExit()
        {
            base.OnExit();

            EventSystem.player.RemoveListener(PlayerEvents.DEATH_SCREEN_FADED, poop);
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
            
        }

        void poop()
        {
            StartCoroutine(TutorialDeath());
        }

        IEnumerator TutorialDeath()
        {

            yield return null;
            //set player to death location
            GameData.playerTransform.position = gameOver.deathPoint.position;

            //reset the scene to just before they scan kiosk 4
            //ResetScene();
        }
    }
}