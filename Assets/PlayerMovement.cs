using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float scale = 10f;
    private Vector2 direction;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        TakeInput();
        Move();
    }

    void TakeInput()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direction = direction.normalized;
        if (direction.x < 0)
        {
            transform.localScale = new Vector2(-scale, transform.localScale.y);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector2(scale, transform.localScale.y);
        }
    }

    void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (direction.magnitude == 0)
        {
            animator.SetLayerWeight(1, 0);
        }
        else
        {
            animator.SetLayerWeight(1, 1);
            SetAnimatorMovement(direction);
        }


    }

    void SetAnimatorMovement(Vector2 direction)
    {
        animator.SetFloat("xDir", direction.x);
        animator.SetFloat("yDir", direction.y);
    }
}
