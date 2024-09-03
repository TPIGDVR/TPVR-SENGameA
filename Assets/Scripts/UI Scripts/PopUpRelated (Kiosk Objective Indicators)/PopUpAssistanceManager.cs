using Assets.Scripts.pattern;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PopUpAssistance
{
    public class PopUpAssistanceManager : MonoBehaviour
    {
        [SerializeField]GameObject popUpPrefabs;
        [SerializeField] Transform _PlayerPosition;
        PopUp[] existingPopUp;
        [SerializeField] float maxDistance;
        [SerializeField] float minDistance;
        PoolingPatternBasic poolPopDisplays;

        float minSqrDistance;
        float maxSqrDistance;
        private void Start()
        {
            existingPopUp = GameObject.FindObjectsOfType<PopUp>();
            poolPopDisplays = new PoolingPatternBasic(popUpPrefabs);
            poolPopDisplays.InitWithParent(10, transform);

            minSqrDistance = minDistance * minDistance;
            maxSqrDistance = maxDistance * maxDistance;
        }

        private void Update()
        {
            foreach(var popUp in existingPopUp)
            {
                if (!popUp.CanPopUp)
                {
                    if (popUp.hasDisplay)
                    {
                        RetrieveDisplay(popUp);
                    }
                    continue;
                }

                float distanceFromPopupToPlayer = Vector3.SqrMagnitude(popUp.transform.position - _PlayerPosition.transform.position);
                if ( distanceFromPopupToPlayer < maxSqrDistance && 
                    distanceFromPopupToPlayer > minSqrDistance)
                {
                    if(!popUp.hasDisplay)
                    {
                        var display = poolPopDisplays.Get();
                        //teleport the display to the popup transform
                        display.transform.position = popUp.transform.position;
                        popUp.Display = display;
                    }
                }
                else
                {
                    if (popUp.hasDisplay)
                    {
                        RetrieveDisplay(popUp);
                    }
                    //else ignore already
                }
            }

            void RetrieveDisplay(PopUp popUp)
            {
                var display = popUp.Display;
                poolPopDisplays.Retrieve(display);
                popUp.Display = null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_PlayerPosition.transform.position, maxDistance);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(_PlayerPosition.transform.position, minDistance);
        }

    }
}