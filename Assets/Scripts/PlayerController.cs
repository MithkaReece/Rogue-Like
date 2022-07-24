using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float scale = 4f;
    private Rigidbody2D rb;
    private Rigidbody2D drb;
    private Rigidbody2D krb;

    private Animator bodyAnimator;

    private bool canMove = true;

    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private SpriteRenderer sr;
    private Sprite[] sprites;

    [SerializeField] private float AttackCooldown = 0.5f;
    [SerializeField] private float AttackCooldownCounter = 0f;

    [SerializeField] private float RollCooldown = 0.5f;
    [SerializeField] private float RollCooldownCounter = 0f;

    [SerializeField] private string swordEquiped = "Sword1";
    private float swapCooldown = 2f;
    private float swapCooldownCounter = 0f;

    private State state;
    private enum State
    {
        Normal,
        DodgeRoll,
        Attack,
    }

    // Start is called before the first frame update
    void Start()
    {
        drb = GetComponent<Rigidbody2D>();
        krb = transform.Find("Blocker").GetComponent<Rigidbody2D>();


        rb = GetComponent<Rigidbody2D>();
        bodyAnimator = transform.Find("Body").GetComponent<Animator>();
        currentHealth = maxHealth;

        sr = transform.Find("Body").transform.Find("Sword2").GetComponent<SpriteRenderer>();
        sprites = Resources.LoadAll<Sprite>("Swords/" + swordEquiped);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Normal:
                if (canMove)
                {
                    HandleWalk();
                    LeftRightFlip();
                    HandleSwordSwap();
                    HandleAttack();
                    HandleDodgeRoll();
                }
                break;
            case State.DodgeRoll:
                HandleDodgeRollMotion();
                break;
            case State.Attack:
                break;
        }
    }

    [SerializeField] private float moveSpeed;
    private Vector2 inputDirection;
    void HandleWalk()
    {
        inputDirection = (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))).normalized;
        //drb.MovePosition(drb.position + inputDirection * moveSpeed * Time.deltaTime);
        drb.velocity = inputDirection * moveSpeed;

        if (inputDirection.magnitude == 0)
        { //Stop walk animation as not moving
            bodyAnimator.SetLayerWeight(1, 0);
        }
        else
        {
            bodyAnimator.SetLayerWeight(1, 1);
            SetAnimatorMovement(inputDirection);
        }
    }
    void SetAnimatorMovement(Vector2 direction)
    {
        bodyAnimator.SetFloat("xDir", direction.x);
        bodyAnimator.SetFloat("yDir", direction.y);
    }

    //Allows right animation to be flipped and used as left
    void LeftRightFlip()
    {
        if (inputDirection.x < 0)
        {
            transform.localScale = new Vector2(-scale, scale);
        }
        else if (inputDirection.x > 0)
        {
            transform.localScale = new Vector2(scale, scale);
        }
    }
    private void HandleSwordSwap()
    {
        if (swapCooldownCounter > 0) { swapCooldownCounter -= Time.deltaTime; }
        if (Input.GetButton("Attack2") && swapCooldownCounter <= 0f)
        {
            swapCooldownCounter = swapCooldown;
            if (swordEquiped == "Sword1")
            {
                Debug.Log("To sword2");
                SwapSword("Sword2");
            }
            else
            {
                Debug.Log("To sword1");
                SwapSword("Sword1");
            }
        }
    }
    void SwapSword(string newSword)
    {
        swordEquiped = newSword;
        sprites = Resources.LoadAll<Sprite>("Swords/" + newSword);
    }

    private void HandleAttack()
    {
        if (AttackCooldownCounter > 0) { AttackCooldownCounter -= Time.deltaTime; }
        if (Input.GetButton("Attack1") && AttackCooldownCounter <= 0f)
        {
            bodyAnimator.SetTrigger("Attack2");
            AttackCooldownCounter = AttackCooldown;
        }
    }



    void HandleDodgeRoll()
    {
        if (RollCooldownCounter > 0) { RollCooldownCounter -= Time.deltaTime; }
        if (Input.GetButton("Jump"))
        {
            if (RollCooldownCounter > 0) { return; }
            bodyAnimator.SetTrigger("Roll");
            RollCooldownCounter = RollCooldown;
            canMove = false;
            state = State.DodgeRoll;
        }
    }
    [SerializeField] private float rollSpeed = 3f;
    void HandleDodgeRollMotion()
    {
        rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * rollSpeed, 0);
        /*if (transform.localScale.x < 0)
        {
            transform.position += new Vector3(-1, 0) * rollSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(1, 0) * rollSpeed * Time.deltaTime;
        }*/

    }
    public void EndRoll(int par)
    {
        canMove = true;
        state = State.Normal;
    }




    [SerializeField] private LayerMask enemyLayers;
    //So far the only trigger is the collider around the sword when swinging
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (enemyLayers == (enemyLayers | (1 << collider.gameObject.layer)))
        {
            Debug.Log("Hit");
            //collider.gameObject.GetComponent<EnemyController>().TakeDamage(50);
        }
    }


    public IEnumerator Knockback(float knockbackDuration, float knockbackPower, Vector2 objPos)
    {
        canMove = false;
        rb.isKinematic = false;
        Vector2 direction = (objPos - ((Vector2)transform.position + GetComponent<Collider2D>().offset)).normalized;
        rb.AddForce(-direction * knockbackPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        canMove = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    private Sprite sword1;
    public void DisplaySword1(int par)
    {
        state = State.Attack;
        sr.sprite = sprites[0];
    }
    private Sprite sword2;
    public void DisplaySword2(int par)
    {
        sr.sprite = sprites[1];
    }
    private Sprite sword3;
    public void DisplaySword3(int par)
    {
        sr.sprite = sprites[2];
    }

    public void DisplayNothing(int par)
    {
        state = State.Normal;
        sr.sprite = null;
    }
}
