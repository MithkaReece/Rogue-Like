using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.Animations;

public class EntityController : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator bodyAnimator;
    protected GameObject healthRing;
    protected Inventory inv;
    protected Equipment equipment;

    public EntityStats entityStats;
    public EntityObserver EntityObserver { get; } = new EntityObserver();
    public string Name; //Identifier for the entity

    private ReposController repos;

    private int _up;//Flips animations between up and down
    protected string _currentState;
    protected const string IDLE = "Idle";
    protected const string WALK = "Run3D";
    //const string WALK = "Walk";
    protected const string ROLL = "RunRoll";

    protected const string ATTACK = "Attack";

    protected void Awake()
    {
        inv = GetComponent<Inventory>();
        equipment = GetComponent<Equipment>();
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject body = transform.GetChild(0).gameObject;
        bodyAnimator = body.GetComponent<Animator>();
        healthRing = transform.GetChild(3).gameObject;
        repos = healthRing.transform.GetChild(0).gameObject.GetComponent<ReposController>();
        ChangeState(IDLE); //START on IDLE state
    }



    public void ReceiveItem(Item item)
    {
        if (item is Weapon weapon)
        {
            //Retrieve animation clip
            string clipPath = "Animation/" + Name + "/" + weapon.AttackAnimationName;
            AnimationClip[] clips0 = Resources.LoadAll<AnimationClip>(clipPath+"_0");
            if (clips0.Length == 0)
            {
                Debug.Log("Clip:" + clipPath + "_0 was not found");
                return;
            }
            AnimationClip[] clips1 = Resources.LoadAll<AnimationClip>(clipPath + "_1");
            if (clips1.Length == 0)
            {
                Debug.Log("Clip:" + clipPath + "_1 was not found");
                return;
            }
            Debug.Log("Successfully loaded animation clip");
            inv.AddItem(item);

        }
    }

    protected State state;
    protected enum State
    {
        Idle,
        DodgeRoll,
        Attack,
        Block,
        Blocking,
        Parry,
        Hit,
        Stunned,
        Die,
    }

    //Used for rigidbody (physics)
    protected virtual void FixedUpdate()
    {
        Debug.Log(GetState());
        switch (GetState())
        {
            case ATTACK:
                //UpdateAttackLunge();
                break;
            case ROLL:
                UpdateDodgeRoll();
                break;
            //case d:
                //HandleStun();
            //    break;
        }
    }

    #region Update Movements

    void UpdateDodgeRoll() {
        if (!isAnimationPlaying(bodyAnimator, ROLL)) {
            ChangeState(IDLE);
            return;
        }
        rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * entityStats.RollSpeed, 0);
    }

    #endregion


    #region Animation Helpers
    void ChangeState(string _newState)
    {
        string newState = _newState + "_" + _up.ToString();
        if (newState == _currentState)
            return;
        bodyAnimator.Play(newState);
        _currentState = newState;
    }


    protected string GetState() {
        if(_currentState == null) {
            return "";
        }
        return _currentState.Substring(0, _currentState.Length - 2);
    }

    bool isAnimationPlaying(Animator animator, string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName + "_" + _up.ToString()) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }

    //It should add clip to controller dynamically
    //TODO: This should be called when you equip a weapon
    //to make sure the player has all the animations required for the weapon
    void AddAnimationClip(AnimationClip animationClip)
    {
        //Don't add it already exists (Only checks layer 0 but we don't use any layers)
        if (bodyAnimator.HasState(0, Animator.StringToHash(animationClip.name)))
            return;

        AnimatorController animatorController = bodyAnimator.runtimeAnimatorController as AnimatorController;

        if (animatorController != null)
        {
            AnimatorControllerLayer[] layers = animatorController.layers;

            if (layers.Length > 0)
            {
                AnimatorStateMachine stateMachine = layers[0].stateMachine;
                AnimatorState state = stateMachine.AddState(animationClip.name);
                state.motion = animationClip;
            }
        }
    }

    bool IsAnimationFinished()
    {
        AnimatorStateInfo stateInfo = bodyAnimator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        float normalizedTime = stateInfo.normalizedTime;

        return normalizedTime+FloatError >= animationLength;
    }

    #endregion

    protected void Move(Vector2 inputDir)
    {
        rb.velocity = inputDir * entityStats.MoveSpeed;
        //Update up/down direction
        if (rb.velocity.y != 0.0f)
            _up = rb.velocity.y > 0.0f ? 1 : 0;
        Debug.Log(inputDir.magnitude);
        if (inputDir.magnitude > FloatError)
        {
            ChangeState(WALK);
            LeftRightFlipping(inputDir);
        }
        else
            ChangeState(IDLE);
    }
    //TODO: Put this in a better place
    private const float FloatError = 0.01f;
    private bool IsFloatZero(float number) {
        return number < FloatError && number > -FloatError;
    }

    //Flip entity (left/right)
    private void LeftRightFlipping(Vector2 inputDir) {
        if (IsFloatZero(inputDir.x))
            return;
        float sign = inputDir.x / Mathf.Abs(inputDir.x);
        transform.localScale = new Vector2(sign * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        healthRing.transform.localScale = new Vector2(sign * Mathf.Abs(healthRing.transform.localScale.x), healthRing.transform.localScale.y);
    }


    protected void DodgeRoll() {
        if (entityStats.CanRoll()) {
            ChangeState(ROLL);
        }
    }





    //TODO: Unrefactored below (some aren't even used anymore)


    void HandleStun()
    {
        float stunSpeed = 1f;
        if (StunMovement)
            rb.velocity = new Vector2((transform.localScale.x / Mathf.Abs(transform.localScale.x)) * -stunSpeed, 0);
        else
            rb.velocity = Vector2.zero;
    }

    public virtual void TakeDamage(DamageReport dr, EntityController dealer)
    {
        if (repos.MaxRepos())
            dr.damage *= 4;
        rb.velocity = Vector2.zero;
        entityStats.TakeDamage(dr, dealer);

        if (entityStats.CurrentHealth <= 0)
        {
            state = State.Die;
            bodyAnimator.SetTrigger("Die");
            dealer.GotKill();
        }
        else if (repos.MaxRepos())
        {
            state = State.Stunned;
            bodyAnimator.SetTrigger("Stun");
        }
        else
        {
            state = State.Hit;
            bodyAnimator.SetTrigger("Hit");
        }

        repos.AddRepos(dr.damage);
        //Invoke delegates for observers
        this.EntityObserver.OnDamageTaken(dr);
        dr.causedBy.EntityObserver.OnDamageDealt(dr);
    }


    public virtual void Block()
    {
        state = State.Block;
        bodyAnimator.SetTrigger("Block");
    }
    public virtual void Parried()
    {
        state = State.Stunned;
        bodyAnimator.SetTrigger("Stun");
    }

    public virtual void GotKill()
    {

    }

    #region Animation Events
    protected bool attackLunging;
    //Called: Stage when attack starts lunging (moving)
    public virtual void StartAttackLunge() { attackLunging = true; }
    //Called: Stage when attack lunge ends (stops moving)
    public virtual void EndAttackLunge() { attackLunging = false; }
    public virtual void EndAttack()
    {
        state = State.Idle;
        entityStats.Combat.AttackCooldownCounter.Reset(1f / entityStats.Combat.AttackSpeed);
    }

    public virtual void EndHit() { state = State.Idle; }
    //TODO: Make faster (player doesn't use, overrides)
    public virtual void EndDie()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        transform.Find("Collision Blocker").GetComponent<CapsuleCollider2D>().enabled = false;
        transform.Find("HitBox").GetComponent<CapsuleCollider2D>().enabled = false;
        transform.Find("HealthRing").GetComponent<SpriteRenderer>().enabled = false;
        transform.Find("HealthRing").Find("ReposRing").GetComponent<SpriteRenderer>().enabled = false;
        this.enabled = false;
    }

    protected bool StunMovement;
    public void StartStun() { StunMovement = true; }

    public void EndStunMovement() { StunMovement = false; }
    public void EndStun()
    {
        state = State.Idle;
        bodyAnimator.SetBool("Stun", false);
    }

    public void StopBlocking() { state = State.Idle; }



    protected bool invulnerable;
    public void StartInv() { invulnerable = true; }
    public void EndInv() { invulnerable = false; }
    //TODO: Do for enemy as well as player
    public virtual void EndRoll() { }

    public void EndParry() { state = State.Idle; }

    #endregion
}

