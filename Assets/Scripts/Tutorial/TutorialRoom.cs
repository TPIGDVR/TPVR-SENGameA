using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    public class TutorialRoom : Room
    {
        [SerializeField]
        TutorialGameOver gameOver;

        protected override void InitRoom()
        {
            GameData.ChangeTutorialStatus(true);
            DisplayRoomObjective();
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