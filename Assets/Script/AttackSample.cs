using UnityEngine;
using UnityEngine.InputSystem;

public class AttackSample : MonoBehaviour
{
    public Monster monster;

    private void OnEnable()
    {
        if (InputManager.Instance != null && InputManager.Instance.PlayerInputActions != null)
        {
            InputManager.Instance.PlayerInputActions.Player.Attack.performed += OnMonsterAttack;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance.PlayerInputActions != null)
        {
            InputManager.Instance.PlayerInputActions.Player.Attack.performed -= OnMonsterAttack;
        }
    }

    private void OnMonsterAttack(InputAction.CallbackContext context)
    {
        monster.TakeDamage(10, new Vector2(Mathf.Sign(monster.transform.position.x - transform.position.x), 0));
    }
}
