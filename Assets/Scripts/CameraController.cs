using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameManager gameManager;

    #region Zooming
    [Header("Zooming")]
    public float maxZoom = 5;
    public float minZoom = 100;
    public float sensitivityZoom = 1;
    public float speed = 30;
    float targetZoom;
    float originalZoom;
    #endregion

    #region Dragging
    [Header("Dragging")]
    public float sensitivityDrag = 1;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        originalZoom = targetZoom = 30;
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main == null) return;
        if (gameManager == null || !gameManager.IsGameStarted()) return;

        // Zooming
        targetZoom -= Input.mouseScrollDelta.y * sensitivityZoom;
        targetZoom = Mathf.Clamp(targetZoom, maxZoom, minZoom);
        float newSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetZoom, speed * Time.deltaTime);
        Camera.main.orthographicSize = newSize;

        // Dragging
        if (Input.GetButton("Fire3"))
        {
            float mouseDeltaX = Input.GetAxisRaw("Mouse X");
            float mouseDeltaY = Input.GetAxisRaw("Mouse Y");

            Camera.main.transform.position += new Vector3(-mouseDeltaX, 0, -mouseDeltaY) * sensitivityDrag;
        }
    }

    public void ResetOriginalCamera()
    {
        Camera.main.orthographicSize = targetZoom = originalZoom;
    }

    private Vector3 GetMousePositionOnZ(float valueZ)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = valueZ;

        return mousePosition;
    }
}
