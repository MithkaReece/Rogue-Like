using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        keepPlayerInRect();
    }

    //TODO: Could instead use percentage of the screen however would need to
    //TODO: know the current resolution
    [SerializeField] private Vector2 PlayArea = new Vector2(8f, 5f);
    //Keeps player centered within the PlayArea width and height
    //Snaps camera when player crosses boundary
    void keepPlayerInRect()
    {
        Vector2 diff = transform.position - player.transform.position;
        float newX = transform.position.x;
        float newY = transform.position.y;
        if (Mathf.Abs(diff.x) > PlayArea.x * 0.5f)
        {
            newX = player.transform.position.x + (diff.x / Mathf.Abs(diff.x)) * PlayArea.x * 0.5f;
        }
        if (Mathf.Abs(diff.y) > PlayArea.y * 0.5f)
        {
            newY = player.transform.position.y + (diff.y / Mathf.Abs(diff.y)) * PlayArea.y * 0.5f;
        }
        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}
