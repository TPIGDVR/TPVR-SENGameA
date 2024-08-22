using System.Collections;
using UnityEngine;

public class BoundingBoxGenerator : MonoBehaviour
{
    public LayerMask mask;
    StaticObject[] staticObjects;

    Vector2 bbCenter;
    Vector2 bbSize;
    public Color mapColor;
    public RenderTexture mapSnapshot;
    public RectTransform canvasRT;

    void Start()
    {
        GetStaticRef();
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
        Debug.DrawLine(p1, p2, Color.green, 100000f);
        Debug.DrawLine(p2, p3, Color.green, 100000f);
        Debug.DrawLine(p3, p4, Color.green, 100000f);
        Debug.DrawLine(p4, p1, Color.green, 100000f);
    }

    void GetStaticRef()

    {
        staticObjects = GameObject.FindObjectsOfType<StaticObject>();
    }

    IEnumerator GenerateCamera()
    {
        GameObject camGO = new();
        Camera cam  = camGO.AddComponent<Camera>();
        cam.cullingMask = mask;
        cam.orthographic = true;

        float size = 0;
        if(bbSize.x > bbSize.y)
        {
            size = bbSize.x;
        }
        else
        {
            size = bbSize.y;
        }
        cam.orthographicSize = size / 2;

        cam.transform.position = new(bbCenter.x,1,bbCenter.y);
        cam.transform.rotation = Quaternion.Euler(90, -90, 0);

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = mapColor;

        cam.targetTexture = mapSnapshot;
        yield return null;
        cam.targetTexture = null;
        Destroy(camGO);
        DestroyAllCanvas();
        ApplyMapSnapshotToCanvas();
    }

    void DestroyAllCanvas()
    {
        foreach (var item in staticObjects)
        {
            foreach(var c in item.canvas)
            {
                Destroy(c);
            }
        }
    }

    void ApplyMapSnapshotToCanvas()
    {
        canvasRT.position = new(bbCenter.x,0 , bbCenter.y);
        canvasRT.sizeDelta = new(bbSize.y,bbSize.x);
    }
}
