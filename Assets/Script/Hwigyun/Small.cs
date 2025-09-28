using System.Collections;
using UnityEngine;

public class Small : Monster
{
    [Header("Small Settings")]
    [SerializeField]
    private float patrolRange = 5f;
    [SerializeField]
    private float stopDurationAtPatrolEnd = 2f;

    [Header("Animation")]
    protected static readonly int AnimSpecial = Animator.StringToHash("Special");

    private Vector2 patrolStartPosition;
    private int patrolDirection = 1;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        patrolStartPosition = transform.position;
        SetState(MonsterState.Idle);
    }

    protected override void Update()
    {
        base.Update();

        if (currentState == MonsterState.Special ||
            currentState == MonsterState.Dead ||
            currentState == MonsterState.Knockback ||
            currentState == MonsterState.Attack) return;

        bool playerDetected = IsPlayerInRange(detectionRange, out targetPlayer);

        switch (currentState)
        {
            case MonsterState.Idle:
                if (playerDetected)
                {
                    ChangeState(MonsterState.Chase);
                }
                else
                {
                    ChangeState(MonsterState.Patrol);
                }
                break;
            case MonsterState.Patrol:
                if (playerDetected)
                {
                    ChangeState(MonsterState.Chase);
                }
                break;
            case MonsterState.Chase:
                if (!playerDetected)
                {
                    ChangeState(MonsterState.Patrol);
                }
                else if (IsPlayerInRange(attackRange, out GameObject attackTarget))
                {
                    ChangeState(MonsterState.Attack);
                }
                else
                {
                    if (targetPlayer != null)
                    {
                        ChaseLogic();
                    }
                    else
                    {
                        ChangeState(MonsterState.Patrol);
                    }
                }
                break;
        }
    }

    protected override void UpdateAnimationState()
    {
        anim.SetBool(AnimSpecial, false);

        switch (currentState)
        {
            case MonsterState.Idle:
            case MonsterState.Patrol:
            case MonsterState.Chase:
            case MonsterState.Attack:
            case MonsterState.Knockback:
            case MonsterState.Dead:
                break;
            case MonsterState.Special:
                anim.SetBool(AnimSpecial, true);
                break;
        }
    }

    private void ChangeState(MonsterState newState)
    {
        if (currentState == newState) return;
        if (currentState == MonsterState.Dead) return;

        if (currentActionCoroutine != null)
        {
            StopCoroutine(currentActionCoroutine);
            currentActionCoroutine = null;
        }

        SetState(newState);

        switch (newState)
        {
            case MonsterState.Idle:
                rb.linearVelocityX = 0;
                break;
            case MonsterState.Patrol:
                currentActionCoroutine = StartCoroutine(PatrolRoutine());
                break;
            case MonsterState.Chase:
                break;
            case MonsterState.Attack:
                currentActionCoroutine = StartCoroutine(AttackRoutine());
                break;
            case MonsterState.Knockback:
                break;
            case MonsterState.Special:
                currentActionCoroutine = StartCoroutine(SpecialStateRoutine());
                break;
            case MonsterState.Dead:
                break;
        }
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            Vector2 targetPosition = patrolStartPosition + new Vector2(patrolRange * patrolDirection, 0);

            while (Mathf.Abs(transform.position.x - targetPosition.x) > 0.1f)
            {
                if (currentState != MonsterState.Patrol) yield break;

                float moveDirectionX = Mathf.Sign(targetPosition.x - transform.position.x);
                rb.linearVelocityX = moveDirectionX * moveSpeed;

                if ((moveDirectionX > 0 && !isFacingRight) || (moveDirectionX < 0 && isFacingRight))
                {
                    Flip();
                }
                yield return null;
            }

            rb.linearVelocityX = 0;

            yield return new WaitForSeconds(stopDurationAtPatrolEnd);

            if (currentState != MonsterState.Patrol) yield break;

            patrolDirection *= -1;
        }
    }

    private void ChaseLogic()
    {
        if (targetPlayer == null)
        {
            Debug.Log("Not Player");
            return;
        }

        Debug.Log("Follow Start");

        float targetX = targetPlayer.transform.position.x;
        float currentX = transform.position.x;

        float moveDirectionX = 0;
        if (Mathf.Abs(targetX - currentX) > 0.1f)
        {
            moveDirectionX = Mathf.Sign(targetX - currentX);
        }

        rb.linearVelocityX = moveDirectionX * moveSpeed;

        if (moveDirectionX > 0 && !isFacingRight || moveDirectionX < 0 && isFacingRight)
        {
            Flip();
        }
    }

    protected override IEnumerator AttackRoutine()
    {
        anim.SetTrigger(AnimAttack);
        float timer = 0f;
        while (timer < attackDuration)
        {
            if (currentState == MonsterState.Knockback || currentState == MonsterState.Dead || currentState == MonsterState.Special)
            {
                Debug.Log("Small Attack fail");
                if (attackHitbox != null) attackHitbox.SetActive(false);
                ChangeState(MonsterState.Chase);
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Small Attack Okay!");
        ChangeState(MonsterState.Special);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerStats playerHealth = collision.GetComponent<PlayerStats>();
        if(playerHealth != null && collision.gameObject.CompareTag("Player"))
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    protected IEnumerator SpecialStateRoutine()
    {
        rb.linearVelocityX = 0;
        isInvincible = true;

        while (true)
        {
            yield return null;
        }
    }

    public override void TakeDamage(float damage, Vector2 knockbackDirection, bool canKnockback = true)
    {
        bool actualCanKnockback = canKnockback && (currentState != MonsterState.Special && currentState != MonsterState.Dead);
        base.TakeDamage(damage, isInvincible ? Vector2.zero : knockbackDirection, actualCanKnockback);
    }

    protected override IEnumerator KnockbackRoutine()
    {
        yield return base.KnockbackRoutine();
        ChangeState(MonsterState.Patrol);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(patrolStartPosition, new Vector3(patrolRange * 2, 0.5f, 0));
        Gizmos.DrawWireSphere(patrolStartPosition, 0.2f);
        Gizmos.DrawWireSphere(patrolStartPosition + new Vector2(patrolRange * patrolDirection, 0), 0.2f);
    }
    
}
