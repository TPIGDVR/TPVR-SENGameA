using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Mathematics;
public class ControllerCheckForCollider : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;


    [SerializeField]
    Transform parentControllerComponent;

    [SerializeField]
    Vector3 directionAwayFromTheController;
    [SerializeField]
    float maxDistanceFromController = 0.1f;
    [SerializeField]
    float weightForDirectionAwayFromController = 10f;
    [SerializeField]
    float weightForDirectionTowardsCamera = 20f;
    
    Camera mainCamera;
    [SerializeField]
    Vector3 shiftOffset;
    Ray ray;
    [SerializeField]
    LayerMask layerMask;
    void Start()
    {
        mainCamera = Camera.main;
        lineRenderer.positionCount = 2;
        EventSystem.level.AddListener(LevelEvents.FINISH_TUTORIAL, OnEndTutorial);
        lineRenderer.useWorldSpace = false;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 dirAwayFromController = directionAwayFromTheController.normalized;
        dirAwayFromController *= weightForDirectionAwayFromController;

        //then check whether the stateText hits the player.
        Vector3 dirTowardsCamera = (mainCamera.transform.position - transform.position).normalized;
        ray = new Ray(mainCamera.transform.position, - dirTowardsCamera);
        //dirTowardsCamera += shiftOffset;
        dirTowardsCamera.Normalize();

        dirTowardsCamera *= weightForDirectionTowardsCamera;

        bool hasHit = Physics.Raycast(ray, out RaycastHit hitinfo, 5f, layerMask);

        //if (hasHit)
        //    Debug.Log($"Collider hit {hitinfo.collider.name}");
        hasHit = false; //Band-aid patch for now

        Vector3 upwardOffset = hasHit? Vector3.up * 0.2f : Vector3.zero;

        Vector3 TargetPosition = parentControllerComponent.position +
                    dirAwayFromController +
                    dirTowardsCamera +
                    upwardOffset +
                    shiftOffset;

        //transform.position = Vector3.Lerp(transform.position,
        //    TargetPosition,
        //    math.saturate(Time.deltaTime * 5f));

        transform.position = TargetPosition;

    }

    private void LateUpdate()
    {
        UpdateLine();
    }

    void UpdateLine()
    {
        //lineRenderer.SetPosition(0, transform.position);
        //lineRenderer.SetPosition(1, parentControllerComponent.position);


        lineRenderer.SetPositions(new Vector3[] { transform.localPosition, parentControllerComponent.localPosition });
    }

    void OnEndTutorial()
    {
        EventSystem.level.RemoveListener(LevelEvents.FINISH_TUTORIAL, OnEndTutorial);
        gameObject.SetActive(false);
    }    

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray);
    }
}

