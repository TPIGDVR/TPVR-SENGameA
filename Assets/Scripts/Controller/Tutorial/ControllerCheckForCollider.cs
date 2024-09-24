using UnityEngine;
//make sure to unparent the text from the controller.
public class ControllerCheckForCollider : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;
    [SerializeField]
    Transform parentControllerComponent;
    [SerializeField]
    Transform targetControl;
    [SerializeField]
    Vector3 globalOffset;
    // [SerializeField]
    // LayerMask layerMask;

    // [SerializeField]
    // Vector3 directionAwayFromTheController;
    // [SerializeField]
    // float maxDistanceFromController = 0.1f;
    // [SerializeField]
    // float weightForDirectionAwayFromController = 10f;
    // [SerializeField]
    // float weightForDirectionTowardsCamera = 20f;

    void Start()
    {
        lineRenderer.positionCount = 2;
        EventSystem.level.AddListener(LevelEvents.FINISH_TUTORIAL, OnEndTutorial);
        lineRenderer.useWorldSpace = true;
    }
    // Update is called once per frame
    // void Update()
    // {
    //     #region  legacy code
    //     // Vector3 dirAwayFromController = directionAwayFromTheController.normalized;
    //     // dirAwayFromController *= weightForDirectionAwayFromController;

    //     // //then check whether the stateText hits the player.
    //     // Vector3 dirTowardsCamera = (mainCamera.transform.position - transform.position).normalized;
    //     // ray = new Ray(mainCamera.transform.position, -dirTowardsCamera);
    //     // //dirTowardsCamera += shiftOffset;
    //     // dirTowardsCamera.Normalize();

    //     // dirTowardsCamera *= weightForDirectionTowardsCamera;

    //     // bool hasHit = Physics.Raycast(ray, out RaycastHit hitinfo, 5f, layerMask);

    //     // //if (hasHit)
    //     // //    Debug.Log($"Collider hit {hitinfo.collider.name}");
    //     // hasHit = false; //Band-aid patch for now

    //     // Vector3 upwardOffset = hasHit ? Vector3.up * 0.2f : Vector3.zero;

    //     // Vector3 TargetPosition = parentControllerComponent.position +
    //     //             dirAwayFromController +
    //     //             dirTowardsCamera +
    //     //             upwardOffset +
    //     //             shiftOffset;

    //     //transform.position = Vector3.Lerp(transform.position,
    //     //    TargetPosition,
    //     //    math.saturate(Time.deltaTime * 5f));

    //     // transform.position = TargetPosition;
    //     #endregion
    // }

    #region legacy
    private void LateUpdate()
    {
        UpdateLine();
    }

    void UpdateLine()
    {
        transform.position = parentControllerComponent.transform.position + globalOffset;
        lineRenderer.SetPositions(new Vector3[] { transform.position, targetControl.position });
    }

    void OnEndTutorial()
    {
        EventSystem.level.RemoveListener(LevelEvents.FINISH_TUTORIAL, OnEndTutorial);
        gameObject.SetActive(false);
        lineRenderer.enabled = false;
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawRay(ray);
    // }

    #endregion
}

