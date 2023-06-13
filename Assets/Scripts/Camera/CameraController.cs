using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController player;

    private Camera cameraComponent;
    // Start is called before the first frame update
    void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        keepPlayerInRect();

        HandleZoom();
    }

    void HandleZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
            float zoomFactor = 1.0f;
            cameraComponent.orthographicSize -= scrollAmount * zoomFactor;
            //Set min & max zoom
            cameraComponent.orthographicSize = Mathf.Clamp(cameraComponent.orthographicSize, 1.0f, 100f);
        }
    }

    //TODO: Could instead use percentage of the screen however would need to
    //TODO: know the current resolution
    [SerializeField] private Vector2 PlayArea = new Vector2(0.15f, 0.15f);
    //Keeps player centered within the PlayArea width and height
    //Snaps camera when player crosses boundary
    void keepPlayerInRect()
    {
        Vector3 playerViewPos = cameraComponent.WorldToViewportPoint(player.transform.position);

        float offsetX = (1f - PlayArea.x) * 0.5f;
        float offsetY = (1f - PlayArea.y) * 0.5f;
        float minX = offsetX;
        float maxX = 1f - offsetX;
        float minY = offsetY;
        float maxY = 1f - offsetY;

        float clampedX = Mathf.Clamp(playerViewPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(playerViewPos.y, minY, maxY);

        Vector3 clampedWorldPos = cameraComponent.ViewportToWorldPoint(new Vector3(clampedX, clampedY, playerViewPos.z));
        Vector3 diff = player.transform.position - clampedWorldPos;
        transform.position += diff;
    }
}
