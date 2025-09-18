using UnityEngine;

public class Boss : MonoBehaviour
{
    public float maxHealth = 1000f;
    public float currentHealth;

    void Start()
    {
        maxHealth = currentHealth;
    } 
}
