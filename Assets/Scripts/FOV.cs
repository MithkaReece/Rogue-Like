using System.Collections;
using UnityEngine;

public class FOV : MonoBehaviour
{
    public float radius;

    public LayerMask targetMask;
    public LayerMask obstructionMask;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FOVRoutine());
    }


    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new(0.2f);
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        //Get enemies close to player
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);
        foreach (Collider2D TargetObj in rangeChecks) 
        {
;           Transform target = TargetObj.transform;
            BaseEnemyController enemy = target.GetComponent<BaseEnemyController>();
            if (enemy == null)
                break;

            Vector2 targetToPlayer = transform.position - target.position;
            float dist = Mathf.Sqrt(targetToPlayer.magnitude);
            if (dist > enemy.maxFollowDist)
            {
                enemy.canSeePlayer = false;
                break;
            }

            //Check if in fov angle
            if (Vector2.Angle(enemy.LookingDirection, targetToPlayer) < enemy.FOVAngle / 2)
            {
                if (!Physics2D.Raycast(target.position, targetToPlayer, dist, obstructionMask))
                    enemy.canSeePlayer = true;
                
                else
                    enemy.canSeePlayer = false;
            }
        }
    }



}
