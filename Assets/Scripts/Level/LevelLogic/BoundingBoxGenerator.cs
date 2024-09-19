using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BoundingBoxGenerator : MonoBehaviour
{
    public LayerMask mask;
    StaticObject[] staticObjects;
    DynamicObjects[] dynamicObjects;
    Vector2 bbCenter;
    Vector2 bbSize;
    public Color mapColor;
    public RectTransform canvasRT;
    public RawImage mapImage;

    public TMP_Text debugtext;

    public float boundingOffsetSize = 1.1f;

    [SerializeField] Vector2 boundingBoxMinVector2;
    [SerializeField] Vector2 boundingBoxMaxVector2;
    [SerializeField] float yOffset = 1;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        GetStaticRef();
        GetDynamicRef();
        GenerateBoundingBox2D();
    }

    void GenerateBoundingBox2D()
    {
        Collider[] colliders = FindObjectsOfType<Collider>();

        if (colliders.Length == 0)
        {
            Debug.LogWarning("No colliders found in the scene.");
            return;
        }

        // Initialize the min and max points
        Vector2 minPoint = new Vector2(colliders[0].bounds.min.x, colliders[0].bounds.min.z);
        Vector2 maxPoint = new Vector2(colliders[0].bounds.max.x, colliders[0].bounds.max.z);

        // Iterate through each collider to find the min and max XZ points
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Ignore bounding box"))
            {
                continue;
            }
            Bounds bounds = col.bounds;
            minPoint = Vector2.Min(minPoint, new Vector2(bounds.min.x, bounds.min.z));
            maxPoint = Vector2.Max(maxPoint, new Vector2(bounds.max.x, bounds.max.z));
        }

        minPoint *= boundingOffsetSize;
        maxPoint *= boundingOffsetSize;

        // Calculate the center and size of the 2D bounding box
        bbCenter = (minPoint + maxPoint) / 2;
        bbSize = maxPoint - minPoint;


        // Debugging: visualize the bounding box
        Debug.Log($"Bounding Box 2D Center: {bbCenter}");
        Debug.Log($"Bounding Box 2D Size: {bbSize}");

        // Optional: Draw the 2D bounding box in the editor
        DrawBoundingBox2D(minPoint, maxPoint);

        StartCoroutine(GenerateCamera());
    }

    void DrawBoundingBox2D(Vector2 minPoint, Vector2 maxPoint)
    {
        Vector3 p1 = new Vector3(minPoint.x, 0, minPoint.y);
        Vector3 p2 = new Vector3(maxPoint.x, 0, minPoint.y);
        Vector3 p3 = new Vector3(maxPoint.x, 0, maxPoint.y);
        Vector3 p4 = new Vector3(minPoint.x, 0, maxPoint.y);

        // Draw the lines for the bounding box in the XZ plane
        Debug.DrawLine(p1, p2, Color.red, 10000f);
        Debug.DrawLine(p2, p3, Color.red, 10000f);
        Debug.DrawLine(p3, p4, Color.red, 10000f);
        Debug.DrawLine(p4, p1, Color.red, 10000f);
    }

    void GetStaticRef()

    {
        staticObjects = GameObject.FindObjectsOfType<StaticObject>();
    }

    void GetDynamicRef()
    {
        dynamicObjects = GameObject.FindObjectsOfType<DynamicObjects>();
    }

    IEnumerator GenerateCamera()
    {
        ToggleDynamicCanvas(false);
        GameObject camGO = new();
        Camera cam = camGO.AddComponent<Camera>();
        cam.cullingMask = mask;
        cam.orthographic = true;
        //cam.aspect = bbSize.x / bbSize.y;
        float size = Mathf.Max(bbSize.x, bbSize.y);

        cam.orthographicSize = size / 2;

        cam.transform.position = new(bbCenter.x, 1, bbCenter.y);
        cam.transform.rotation = Quaternion.Euler(90, -90, 0);

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = mapColor;
        int scalar = 5;


        //try new
        // int camHeight = (int)(cam.orthographicSize * 2) * scalar;
        // int camHeight = (int)(cam.orthographicSize * 2);
        // int camWidth = (int)(camHeight * cam.aspect);

        float floatCamHeight = cam.orthographicSize * 2;
        float floatCamWidth = floatCamHeight * cam.aspect;


        boundingBoxMinVector2 = new Vector2(bbCenter.x - floatCamWidth / 2, bbCenter.y - floatCamHeight / 2);
        boundingBoxMaxVector2 = new Vector2(bbCenter.x + floatCamWidth / 2, bbCenter.y + floatCamHeight / 2);
        // DrawBoundingBox2D(cam.rect.min, cam.rect.max);

        //old method ;()
        // int camWidth = (int)(cam.orthographicSize * cam.aspect) * 2 * scalar;
        // manually add the offset to perfectly scale the map up
        //needs to be fixed
        //RenderTexture rt = new(camWidth, camWidth + 120, 0);
        // RenderTexture rt = new(camWidth, (int)((float)camWidth * 3.5f), 0);
        RenderTexture rt = new((int)floatCamWidth * scalar,
        (int)floatCamHeight * scalar, 0);

        Vector2 snapShotSize = new Vector2(floatCamWidth, floatCamHeight);

        //RenderTexture copy = new(camWidth, camWidth + 120, 0);
        //rt.filterMode = FilterMode.Point;
        cam.targetTexture = rt;
        rt.Create();
        //copy.Create();
        GameData.miniMapSnapShot = rt;
        yield return null;
        cam.targetTexture = null;
        Destroy(camGO);
        DestroyAllStaticCanvas();
        ApplyMapSnapshotToCanvas(snapShotSize);
        ToggleDynamicCanvas(true);
    }

    void DestroyAllStaticCanvas()
    {
        foreach (var item in staticObjects)
        {
            foreach (var c in item.canvas)
            {
                Destroy(c);
            }
        }
    }

    void ToggleDynamicCanvas(bool b)
    {
        foreach (var item in dynamicObjects)
        {
            item.canvas.enabled = b;
        }
    }

    void ApplyMapSnapshotToCanvas(Vector2 SnapShotSize)
    {
        canvasRT.position = new(bbCenter.x, 1, bbCenter.y);
        canvasRT.sizeDelta = new(SnapShotSize.x, SnapShotSize.y);

        //canvasRT.sizeDelta = new(bbSize.x,bbSize.y);


        mapImage.texture = GameData.miniMapSnapShot;
    }

    // private void OnDrawGizmos()
    // {
    //     Vector3 p1 = new Vector3(boundingBoxMinVector2.x, yOffset, boundingBoxMinVector2.y);
    //     Vector3 p2 = new Vector3(boundingBoxMaxVector2.x, yOffset, boundingBoxMinVector2.y);
    //     Vector3 p3 = new Vector3(boundingBoxMaxVector2.x, yOffset, boundingBoxMaxVector2.y);
    //     Vector3 p4 = new Vector3(boundingBoxMinVector2.x, yOffset, boundingBoxMaxVector2.y);

    //     // Draw the lines for the bounding box in the XZ plane
    //     Debug.DrawLine(p1, p2, Color.red);
    //     Debug.DrawLine(p2, p3, Color.red);
    //     Debug.DrawLine(p3, p4, Color.red);
    //     Debug.DrawLine(p4, p1, Color.red);
    // }
}
