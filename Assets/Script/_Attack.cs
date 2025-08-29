using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class _Attack : MonoBehaviour
{
    private _PlayerControls playerctrl;
    private PlayerInputActions controls;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriter;
    private Vector2 moveInput;
    private int AttackCount = 0;
    private bool isAttack = false;
    private float lastAttackTime;
    [SerializeField] private float attackDashSpeed = 5f;
    [SerializeField] private float attackDashTime = 0.2f;
    [SerializeField] private float comboResetTime = 1f;
    public float damage;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        controls = new PlayerInputActions();

        controls.Player.Attack.performed += OnAttack;
    }
    private void Start()
    {
        playerctrl = GetComponent<_PlayerControls>();
    }
    private void FixedUpdate()
    {
        if (playerctrl.canMove == true)
        {
            moveInput = controls.Player.Move.ReadValue<Vector2>();
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack!");
        if (Time.time - lastAttackTime > comboResetTime)
        {
            AttackCount = 0;
            Debug.Log("콤보 초기화");
        }
        if (!isAttack)
        {
            AttackCount++;
            if (AttackCount > 2) AttackCount = 1;

            Debug.Log("AttackCount :" + AttackCount);

            isAttack = true;
            lastAttackTime = Time.time;

            anim.SetInteger("AttackCount", AttackCount);
            anim.SetTrigger("Attack");
        }
        if (AttackCount == 2)
        {
            StartCoroutine(AttackDash());
            AttackCount = 0;
        }
        else
        {
            Debug.Log("공격 무시됨");
        }
    }

    private IEnumerator AttackDash() //moveSpeed , jumpForce = 0바꾸기
    {
        playerctrl.canMove = false;
        rb.gravityScale = 0f;

        float dir = playerctrl.IML ? -1f : 1f;
        Vector2 BackUpVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector2(dir * attackDashSpeed, 0f);
        Debug.Log(rb.linearVelocity);
        yield return new WaitForSeconds(attackDashTime);

        playerctrl.canMove = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 3f;

        
    }
    public void EndAttack()
    {
        isAttack = false;
    }
}
