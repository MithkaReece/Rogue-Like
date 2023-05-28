using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public bool canSeePlayer;
    [SerializeField] public float maxFollowDist;
    [Range(0, 360)]
    public float FOVAngle;
    public Vector2 LookingDirection;

    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<EnemyController>().player;
        LookingDirection = new Vector2(transform.localScale.x, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //IDK if I need this, or FOV will handle it
        //Player radius would have to always be larger than follow dit
        if (canSeePlayer)
        {
            LookingDirection = player.transform.position - transform.position;
            if (LookingDirection.magnitude > Mathf.Pow(maxFollowDist, 2))
            {
                canSeePlayer = false;
            }
        }
    }
}
