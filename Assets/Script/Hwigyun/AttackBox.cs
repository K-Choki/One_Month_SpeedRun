using UnityEngine;

public class AttackBox : MonoBehaviour
{
    private float damage;

    public void Setup(float projectileDamage = 10f)
    {
        damage = projectileDamage;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth != null && collision.gameObject.CompareTag("Player"))
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
