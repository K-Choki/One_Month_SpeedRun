using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private PlayerStats playerStats;
    private float speed;
    private float damage;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 shootDirection, float shootSpeed, float projectileDamage = 10f)
    {
        direction = shootDirection;
        speed = shootSpeed;
        damage = projectileDamage;
        rb.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x)* Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) return;

        PlayerStats playerHealth = collision.GetComponent<PlayerStats>();
        if(playerHealth != null && collision.gameObject.CompareTag("Player"))
        {
            playerHealth.TakeDamage(damage);
        }
        if(!collision.gameObject.CompareTag("Monster"))
        {
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
