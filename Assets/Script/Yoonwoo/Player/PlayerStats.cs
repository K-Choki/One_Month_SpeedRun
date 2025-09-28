using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHealth = 100;
    private float currentHealth;
    [SerializeField] private HPBarUI hpbar;

    private void Start()
    {
        currentHealth = maxHealth;
        hpbar.SetHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage) //시방 휘건이가 damage float로 처리해놔서 체력을 float 로 해야하네
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        hpbar.SetHealth(currentHealth, maxHealth);

        Debug.Log("TakeDamage : " + damage);
        Debug.Log("currentHealth" + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("Dead");
    }
}
