using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("???? ????")]
    [SerializeField]
    protected float maxHp = 100f;
    [SerializeField]
    protected float currentHp;
    [SerializeField]
    protected float moveSpeed = 2f;
    [SerializeField]
    protected float attackDamage = 10f;

    [Header("???????")]
    protected Animator anim;
    protected static readonly int AnimSpeed = Animator.StringToHash("Speed");
    protected static readonly int AnimAttack = Animator.StringToHash("Attack");
    protected static readonly int AnimKnockback = Animator.StringToHash("Knockback");
    protected static readonly int AnimDead = Animator.StringToHash("Dead");

    [Header("???? & ?úô")]
    protected Rigidbody2D rb;
    [SerializeField]
    protected float knockbackForce = 5f;

    [Header("???? & ???? ????")]
    [SerializeField]
    protected float detectionRange = 5f;
    [SerializeField]
    protected float attackRange = 1f;
    [SerializeField]
    protected LayerMask playerLayer;

    [Header("???? ????")]
    [SerializeField]
    protected GameObject attackHitbox;
    [SerializeField]
    protected float attackDuration = 0.5f;

    public enum MonsterState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Knockback,
        Dead,
        Special
    }
    protected MonsterState currentState;

    protected bool isFacingRight = false;
    protected bool isInvincible = false;
    protected GameObject targetPlayer;

    protected Coroutine currentActionCoroutine;

    protected virtual void Awake()
    {
        currentHp = maxHp;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if(attackHitbox != null )
        {
            attackHitbox.SetActive(false);
        }
    }

    protected virtual void Start()
    {
        SetState(MonsterState.Idle);
    }

    protected virtual void Update()
    {
        anim.SetFloat(AnimSpeed, Mathf.Abs(rb.linearVelocityX));
    }

    protected virtual void FixedUpdate()
    {

    }

    public void SetState(MonsterState newState)
    {
        if(currentState == MonsterState.Dead) return;

        currentState = newState;
        UpdateAnimationState();
    }

    protected virtual void UpdateAnimationState()
    {
        switch(currentState)
        {
            case MonsterState.Idle:
            case MonsterState.Patrol:
            case MonsterState.Chase:
            case MonsterState.Attack:
            case MonsterState.Knockback:
            case MonsterState.Dead:
                break;
        }
    }

    public virtual void TakeDamage(float damage, Vector2 knockbackDirection, bool canKnockback = true)
    {
        if (currentState == MonsterState.Dead) return;

        currentHp -= damage;
        Debug.Log($"???? ???? ????{damage}, ???? HP{currentHp}");

        if(currentHp <= 0)
        {
            Die();
        }
        else if(canKnockback)
        {
            ApplyKnockback(knockbackDirection);
        }
    }

    protected virtual void ApplyKnockback(Vector2 direction)
    {
        rb.linearVelocityX = 0;
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
        SetState(MonsterState.Knockback);
        anim.SetTrigger(AnimKnockback);
        isInvincible = true;
        StartCoroutine(KnockbackRoutine());
    }

    protected virtual IEnumerator KnockbackRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        rb.linearVelocityX = 0;
        isInvincible = false;
    }

    protected virtual void Die()
    {
        SetState(MonsterState.Dead);
        anim.SetTrigger(AnimDead);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        StartCoroutine(DestroyAfterAnimation(anim.GetCurrentAnimatorStateInfo(0).length));
        Debug.Log("???? ????");
    }

    protected IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    protected virtual IEnumerator AttackRoutine()
    {
        anim.SetTrigger(AnimAttack);

        yield return new WaitForSeconds(attackDuration);
    }

    protected bool IsPlayerInRange(float range, out GameObject playerGameObject)
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, range, playerLayer);
        if(playerCollider != null)
        {
            playerGameObject = playerCollider.gameObject;
            //Debug.Log($"Is Player In Range : ?¡À???? ????! : {targetPlayer.name}");
            return true;
        }
        else
        {
            playerGameObject = null;
            //Debug.Log("Is Player In Range : ?¡À???? ????");
            return false;
        }
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void EnableAttackHitbox()
    {
        if(attackHitbox != null)
        {
            AttackBox attackBox = attackHitbox.GetComponent<AttackBox>();
            attackBox.Setup(attackDamage);
            attackHitbox.SetActive(true);
            Debug.Log("???? ????");
        }
    }

    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
            Debug.Log("???? ????");
        }
    }
}
