using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Base for which every UI component should have to make it easy to use when making UI component.
    /// As the UI component is in world space. Take it as a gameobject in the world
    /// </summary>
    public class UIBase : MonoBehaviour
    {
        [Header("Attach to player")]
        [SerializeField] private Transform localParent;
        [SerializeField] private Vector3 translateOffsetFromParent;
        [SerializeField] private Vector3 rotationOffsetFromParent;
        [SerializeField] private bool ignoreRaycasting = true;
        [SerializeField] private float offsetFromObject = 0.5f;
        protected virtual void Awake()
        {

            if (localParent == null) Debug.LogError("No local parent. " +
                $"Need to specify for {transform.name}");
            transform.SetParent(localParent);
            transform.localPosition = translateOffsetFromParent;
            transform.localRotation = Quaternion.Euler(rotationOffsetFromParent);
        }

        protected virtual void Update()
        {
            transform.localPosition = translateOffsetFromParent;
            transform.localRotation = Quaternion.Euler(rotationOffsetFromParent);
            //do ray casting
            if (!ignoreRaycasting)
            {
                Vector3 direction = localParent.position - transform.position;
                Ray ray = new Ray(transform.position, direction);
                if (Physics.Raycast(ray, out var hit, 30f, LayerManager.UILayer))
                {
                    transform.position = hit.point - direction.normalized * offsetFromObject;
                    
                }
            }
        }

        public virtual void ActivateUI()
        {
            gameObject.SetActive(true);
        }

        public virtual void DeactivateUI()
        {
            gameObject.SetActive(false);
        }

        public virtual void ToggleUI()
        {
            if (gameObject.active)
            {
                DeactivateUI();
            }
            else
            {
                ActivateUI();
            }
        }

    }
}