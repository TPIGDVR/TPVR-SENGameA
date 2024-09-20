using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Create a bounding box and generate an image based on that. Helps
/// boost performance by having the map render under 1 canvas instead of
/// multiple canvas.
/// </summary>
public class BoundingBoxGenerator : MonoBehaviour
{
    //Which layer that will be capture for the map to be displayed.
    //KEEP THIS UNDER THE MAP LAYER.
    public LayerMask mask;
    //reference of all the static images (Like the wall's canvas)
    StaticObject[] staticObjects;
    //reference of all the dyanmic images (Like the player and automaton canvas)
    DynamicObjects[] dynamicObjects;

    //bb => boundingbox. 
    /// <summary>
    /// Where the center of all the level
    /// </summary>
    Vector2 bbCenter;
    Vector2 bbSize;
    public Color mapColor;
    public RectTransform canvasRT;
    public RawImage mapImage;

    public float boundingOffsetSize = 1.1f;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        GetStaticRef();
        GetDynamicRef();
        GenerateBoundingBox2D();
    }
    #region get reference method
    //Simple methods that gets all the scripts that contains the 
    //staticObject or dynamicobject reference in the scene itself
    void GetStaticRef()
    {
        staticObjects = GameObject.FindObjectsOfType<StaticObject>();
    }

    void GetDynamicRef()
    {
        dynamicObjects = GameObject.FindObjectsOfType<DynamicObjects>();
    }
    #endregion

    /// <summary>
    /// Find the bounding box of the scene.
    /// Used to generate a camera to snap shot the bounding box.
    /// </summary>
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
        //the scale of the map
        bbSize = maxPoint - minPoint;


        // Debugging: visualize the bounding box
        Debug.Log($"Bounding Box 2D Center: {bbCenter}");
        Debug.Log($"Bounding Box 2D Size: {bbSize}");

        // Optional: Draw the 2D bounding box in the editor
        // DrawBoundingBox2D(minPoint, maxPoint);

        //starts a coroutine to generate the camera and take the snap
        StartCoroutine(GenerateCamera());
    }

    #region  generating Camera
    IEnumerator GenerateCamera()
    {
        //make sure to hide all the dynamic object
        //As we only want to show the static object for the map.
        ToggleDynamicCanvas(false);

        //create a camera to create the rendertexture.
        GameObject camGO = new();
        Camera cam = camGO.AddComponent<Camera>();
        cam.cullingMask = mask;
        cam.orthographic = true;

        float size = Mathf.Max(bbSize.x, bbSize.y);
        //Divide the size by half due to half size.
        cam.orthographicSize = size / 2;

        //make sure the camera is position in the middle of the level to take the snapshot
        cam.transform.position = new(bbCenter.x, 1, bbCenter.y);
        //have the camera have the proper rotation.
        cam.transform.rotation = Quaternion.Euler(90, -90, 0);

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = mapColor;
        int scalar = 5;

        //calculate the size of the rendertexture that is needed to 
        //capture the entire map.
        float floatCamHeight = cam.orthographicSize * 2;
        float floatCamWidth = floatCamHeight * cam.aspect;

        RenderTexture rt = new((int)floatCamWidth * scalar,
        (int)floatCamHeight * scalar, 0);

        Vector2 snapShotSize = new Vector2(floatCamWidth, floatCamHeight);

        //afterwards, reference the render texture on the camera
        cam.targetTexture = rt;
        rt.Create();
        GameData.miniMapSnapShot = rt;
        yield return null;
        //once a texture has been created by the camera, dont reference it anymore
        //as we already have the snapshot of the map.
        cam.targetTexture = null;
        Destroy(camGO); //remove the camera as we dont need.
        DestroyAllStaticCanvas(); //remove all the static canvas as we dont need.
        ApplyMapSnapshotToCanvas(snapShotSize); //afterwards apply the texture to the canvas to display the image.
        ToggleDynamicCanvas(true); //finally enable the dynamic images for the map camera to render.
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
        //makes a dynamic canvas
        canvasRT.position = new(bbCenter.x, 1, bbCenter.y);
        //make the canvas width and height similar to the map.
        canvasRT.sizeDelta = new(SnapShotSize.x, SnapShotSize.y);
        //finally, render the snapshot of the Map.
        mapImage.texture = GameData.miniMapSnapShot;
    }
    #endregion

    #region Debugging Related
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
    #endregion

}
