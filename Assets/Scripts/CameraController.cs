using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameManager gameManager;
    public Camera minimapCamera;

    #region Zooming
    [Header("Zooming")]
    public float maxZoom = 5;
    public float minZoom = 100;
    public float sensitivityZoom = 1;
    public float speed = 30;
    public float targetZoom;
    float originalZoom;
    #endregion

    #region Dragging
    [Header("Dragging")]
    public float sensitivityDrag = 1;
    Vector3 originalPosition;
    #endregion

    int[] mainCameraRingToSize = new int[] { 18, 22, 26, 30, 34, 38, 43, 48, 52 };
    int[] minimapCameraRingToSize = new int[] { 18, 24, 28, 34, 39, 44, 49, 54, 60 };

    // Start is called before the first frame update
    void Start()
    {
        originalZoom = targetZoom = 26;
        originalPosition = Camera.main.transform.position;

        if (minimapCamera == null)
        {
            minimapCamera = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main == null) return;
        // if (gameManager == null || !gameManager.IsGameStarted()) return;
        if (gameManager == null || !gameManager.IsObjectsGenerated()) return;

        // Zooming
        targetZoom -= Input.mouseScrollDelta.y * sensitivityZoom;
        targetZoom = Mathf.Clamp(targetZoom, maxZoom, minZoom);

        // float usedDeltaTime = Time.timeScale <= Mathf.Epsilon ? Time.unscaledDeltaTime : Time.deltaTime;
        // float newSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetZoom, speed * usedDeltaTime);
        float newSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetZoom, speed * Time.unscaledDeltaTime);

        Camera.main.orthographicSize = newSize;

        // Dragging
        if (Input.GetButton("Fire3"))
        {
            float mouseDeltaX = Input.GetAxisRaw("Mouse X");
            float mouseDeltaY = Input.GetAxisRaw("Mouse Y");

            Camera.main.transform.position += new Vector3(-mouseDeltaX, 0, -mouseDeltaY) * sensitivityDrag;
        }
    }

    public void AdjustMainCamera(int ringSize)
    {
        float cameraSize = 26;
        if (ringSize - 3 > mainCameraRingToSize.Length || ringSize < 3)
        {
            cameraSize = (ringSize - 11) * 4 + mainCameraRingToSize[8];
        }
        else
        {
            cameraSize = mainCameraRingToSize[ringSize - 3];
        }

        originalZoom = targetZoom = Camera.main.orthographicSize = cameraSize;
    }

    public void AdjustMiniMapcamera(int ringSize)
    {
        Camera camera = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
        if (camera == null) return;

        float cameraSize = 20;
        if (ringSize - 3 > minimapCameraRingToSize.Length || ringSize < 3)
        {
            cameraSize = (ringSize - 11) * 4 + minimapCameraRingToSize[8];
        }
        else
        {
            cameraSize = minimapCameraRingToSize[ringSize - 3];
        }

        camera.orthographicSize = cameraSize;
    }

    public void ResetOriginalCamera()
    {
        Camera.main.orthographicSize = targetZoom = originalZoom;
        Camera.main.transform.position = originalPosition;
    }

    private Vector3 GetMousePositionOnZ(float valueZ)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = valueZ;

        return mousePosition;
    }
}

public enum MainCameraRingToSize
{
    Ring3Size = 18,
    Ring4Size = 22,
    Ring5Size = 26,
    Ring6Size = 30,
    Ring7Size = 34,
    Ring8Size = 38,
    Ring9Size = 43,
    Ring10Size = 48,
    Ring11Size = 52
}

public enum MiniMapCameraRingToSize
{
    Ring3Size = 18,
    Ring4Size = 24,
    Ring5Size = 28,
    Ring6Size = 34,
    Ring7Size = 39,
    Ring8Size = 44,
    Ring9Size = 49,
    Ring10Size = 54,
    Ring11Size = 60
}
