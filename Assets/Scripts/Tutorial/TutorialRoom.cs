using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    public class TutorialRoom : Room
    {
        protected override void InitRoom()
        {
            GameData.player.SetCurrentRoom(this);
            //GameData.ChangeTutorialStatus(true);
            EventSystem.tutorial.TriggerEvent(TutorialEvents.INIT_TUTORIAL);
            DisplayRoomObjective();
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
                EventSystem.tutorial.TriggerEvent<Transform>(TutorialEvents.FIRST_KIOSK, kiosk.TargetDestination);
            }
        }
    }
}