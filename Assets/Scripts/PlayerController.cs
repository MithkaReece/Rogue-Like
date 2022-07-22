using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float scale = 4f;
    private Rigidbody2D rb;
    private Animator bodyAnimator;

    private bool canMove = true;

    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private SpriteRenderer sr;
    private Sprite[] sprites;

    [SerializeField] private float AttackCooldown = 0.5f;
    [SerializeField] private float AttackCooldownCounter = 0f;

    [SerializeField] private string swordEquiped = "Sword1";
    private float swapCooldown = 2f;
    private float swapCooldownCounter = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyAnimator = transform.Find("Body").GetComponent<Animator>();
        //swordAnimator = transform.Find("Body").transform.Find("OldSword").GetComponent<Animator>();
        currentHealth = maxHealth;

        sr = transform.Find("Body").transform.Find("Sword2").GetComponent<SpriteRenderer>();
        sprites = Resources.LoadAll<Sprite>("Swords/" + swordEquiped);
    }

    // Update is called once per frame
    void Update()
    {
        TakeInput();
        LeftRightFlip();
        if (canMove) { Move(); }

        //sr.sprite = null;

        if (AttackCooldownCounter > 0)
        {
            AttackCooldownCounter -= Time.deltaTime;
        }

        if(Input.GetButton("Jump") && swapCooldownCounter <= 0f){
            swapCooldownCounter = swapCooldown;
            if(swordEquiped == "Sword1"){
                Debug.Log("To sword2");
                swapSword("Sword2");}
            else{
                Debug.Log("To sword1");
                swapSword("Sword1");}
        }
        if(swapCooldownCounter>0){swapCooldownCounter -= Time.deltaTime;} 

    }


    void swapSword(string newSword){
        swordEquiped = newSword;
        sprites = Resources.LoadAll<Sprite>("Swords/" + newSword);
    }

    private Vector2 inputDirection;
    void TakeInput()
    {
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDirection = inputDirection.normalized;

        if (Input.GetButton("Attack1"))
        {
            //Attack1();
        }

        if (Input.GetButton("Attack2"))
        {
            Attack2();
        }
    }

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

    [SerializeField] private float moveSpeed;
    void Move()
    {
        rb.MovePosition(rb.position + inputDirection * moveSpeed * Time.deltaTime);

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

    //Old attack
    /*private Animator swordAnimator;

    void Attack1()
    {
        if (AttackCooldownCounter > 0) { return; }
        //swordAnimator.SetTrigger("Attack");
        //Reset cooldown
        AttackCooldownCounter = AttackCooldown;
    }*/

    void Attack2()
    {
        if (AttackCooldownCounter > 0) { return; }
        bodyAnimator.SetTrigger("Attack2");
        AttackCooldownCounter = AttackCooldown;
    }

    [SerializeField] private LayerMask enemyLayers;
    //So far the only trigger is the collider around the sword when swinging
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (enemyLayers == (enemyLayers | (1 << collider.gameObject.layer)))
        {
            collider.gameObject.GetComponent<EnemyController>().TakeDamage(50);
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
    public void DisplaySword1(int par){
        sr.sprite = sprites[0];
    }
    private Sprite sword2;
    public void DisplaySword2(int par){
        sr.sprite = sprites[1];
    }
    private Sprite sword3;
    public void DisplaySword3(int par){
        sr.sprite = sprites[2];
    }

    public void DisplayNothing(int par){
        sr.sprite = null;
    }
}
