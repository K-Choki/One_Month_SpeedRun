 using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    [SerializeField] private Image HPBar;
    public void SetHealth(float currentHealth, float maxHealth)
    {
        HPBar.fillAmount = currentHealth / maxHealth;
    }
}
