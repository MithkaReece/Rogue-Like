using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float scale = 4f;
    private Vector2 direction;

    private Animator bodyAnimator;
    private Animator swordAnimator;

    private float AttackCooldown = 0.3f;
    private float AttackCooldownCounter = 0f;

    // Start is called before the first frame update
    void Start()
    {
        bodyAnimator = GetComponent<Animator>();
        swordAnimator = transform.Find("Sword").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        TakeInput();

        LeftRightFlip();

        Move();

        if (AttackCooldownCounter > 0)
        {
            AttackCooldownCounter -= Time.deltaTime;
        }

    }

    void TakeInput()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direction = direction.normalized;

        //Attack test with attack cool down
        if (Input.GetButton("Jump"))
        {
            if (AttackCooldownCounter <= 0)
            {
                swordAnimator.SetTrigger("Attack");
                AttackCooldownCounter = AttackCooldown;
            }
        }
    }

    void LeftRightFlip()
    {
        if (direction.x < 0)
        {
            transform.localScale = new Vector2(-scale, scale);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector2(scale, scale);
        }
    }

    void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        
        
        if (direction.magnitude == 0)
        { //Stop walk animation as not moving
            bodyAnimator.SetLayerWeight(1, 0);
        }
        else
        {
            bodyAnimator.SetLayerWeight(1, 1);
            SetAnimatorMovement(direction);
        }


    }

    void SetAnimatorMovement(Vector2 direction)
    {
        bodyAnimator.SetFloat("xDir", direction.x);
        bodyAnimator.SetFloat("yDir", direction.y);
    }
}
