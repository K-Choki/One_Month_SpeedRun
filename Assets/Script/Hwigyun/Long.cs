using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;

public class Long : Monster
{
    [Header("Long Settings")]
    [SerializeField]
    private GameObject projectilePrefab;
    public Transform Player;
    [SerializeField]
    private float attackCooldown = 5f;
    [SerializeField]
    private float projectileSpeed = 1f;
    [SerializeField]
    private Vector3 projectilePos = new Vector3(1.5f, 0f, 0f);

    protected override void Start()
    {
        base.Start();
        SetState(MonsterState.Idle);
        currentActionCoroutine = StartCoroutine(AttackRoutine());
    }

    protected override void Update()
    {
        if (Player == null)
        {
            return;
        }
        if (Player.position.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    protected override IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);

            anim.SetTrigger(AnimAttack);
            SetState(MonsterState.Attack);
        }
    }

    public void SpawnProjectile()
    {
        if (currentState == MonsterState.Knockback && currentState == MonsterState.Dead)
        {
            SetState(MonsterState.Idle);
            return;
        }

        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (playerCollider != null)
        {
            Vector3 spawnPosition = transform.position + (projectilePos * transform.localScale.x * -1);
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            Debug.Log("Attack");
            if (projectileScript != null)
            {
                Vector2 direction = (playerCollider.bounds.center - transform.position).normalized;
                projectileScript.Setup(direction, projectileSpeed, attackDamage);
            }
        }

        SetState(MonsterState.Idle);
    }

    public override void TakeDamage(float damage, Vector2 knockbackDirection, bool canKnockback = true)
    {
        base.TakeDamage(damage, Vector2.zero, false);
    }
}
