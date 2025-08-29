using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public void TakeDamage(float damage)
    {
        Debug.Log($"플레이어 - 데이미 입음 : {damage}");
    }
}
