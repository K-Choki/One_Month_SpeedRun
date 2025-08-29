using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class _PlayerControls : MonoBehaviour
{

    private PlayerInputActions controls;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator anim;
    private SpriteRenderer spriter;

    [Header("캐릭터 움직임 관련")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;


    [Header("지면 확인을 위한 변수")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckHalfCircle = 0.2f;
    private bool isGround;
    public bool IML; // is my face left
    public bool canMove = true;
    private bool isDash = false;
    private bool isDashCooldown = false;
    

    [Header("대시 관련")]
    public float DashCooldown = 0.5f;
    public float DashSpeed = 500f;
    public float DashTime = 0.15f;
    public GameObject DashAfterImage;
    public float afterImageIntervel = 0.05f;
    public float afterImageIntervelTimer;

    //현재 지면확인 하는 변수 선언 및 점프 이제 시작해야함

    private void Awake()
    {
        //혹시나 해서 설명 달아놓음 까먹을까봐 , GetComponent는 씬에 붙어있는 컴포넌트를 가져옴
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();

        //new는 C# 클래스를 새로 생성했을때. ** 헷갈리지 말것 **
        controls = new PlayerInputActions();

        controls.Player.Jump.performed += OnJump;
        controls.Player.Dash.performed += OnDash;
        controls.Player.Skill1.performed += OnSkill1;
        controls.Player.Skill2.performed += OnSkill2;

        controls.Player.FallThrough.performed += OnFallThrough;

        //이건 계속 캐릭터 파일이 업데이트되기 때문에 혹시나 rb 파일 없을때를 대비해서 알람용 ** 마지막에 삭제 **
        if (rb == null)
        {
            Debug.LogError("RigidBody 파일 까먹음");
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

    void Update()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckHalfCircle, groundLayer);
    }

    void FixedUpdate()
    {

        if (canMove == true)
        {
            moveInput = controls.Player.Move.ReadValue<Vector2>();
            transform.position += (Vector3)moveInput * moveSpeed * Time.deltaTime;
        }

        anim.SetBool("IsJump", isGround == false);
        anim.SetFloat("JumpForce", rb.linearVelocityY);

    }
    void LateUpdate()
    {
        anim.SetFloat("Speed", moveInput.magnitude);
        if (moveInput.x != 0)
        {
            IML = spriter.flipX = moveInput.x < 0;
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGround == true)
        {
            rb.linearVelocityY = 0; // 지면에 닿으면 y 속도 초기화

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    private void OnDash(InputAction.CallbackContext context)
    {
        if (isDash || isDashCooldown) return;

        StartCoroutine(DashRoutine());
    }
    private IEnumerator DashRoutine()
    {
        isDash = true;
        isDashCooldown = true;
        rb.gravityScale = 0;


        anim.SetBool("Dash",true);

        //왼쪽이면 -1 , 오른쪽이면 +1
        float dashDir = IML ? -1f : 1f;
        Vector2 BackUpVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector2(dashDir * DashSpeed, 0f);
        yield return new WaitForSeconds(DashTime);

        float DashTimer = 0f; //잔상이 생긴후 경과시간
        afterImageIntervelTimer = 0f;
        while (DashTimer <= DashTime)
        {
            DashTimer += Time.deltaTime;
            afterImageIntervelTimer -= Time.deltaTime;

            if (afterImageIntervelTimer <= 0f)
            {
                float alpha = Mathf.Lerp(0.3f, 1f , (DashTime - DashTimer) / DashTime);
                CreateAfterImage(alpha);
                afterImageIntervelTimer = afterImageIntervel;
            }
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isDash = false;
        anim.SetBool("Dash",false);
        rb.gravityScale = 3;

        yield return new WaitForSeconds(DashCooldown);
        isDashCooldown = false;

    }
    private void OnSkill1(InputAction.CallbackContext context)
    {
        Debug.Log("Skill1!");
        //Skill1 Logic
    }
    private void OnSkill2(InputAction.CallbackContext context)
    {
        Debug.Log("Skill2!");
        //Skill2 Logic
    }
    private void OnFallThrough(InputAction.CallbackContext context) //불리언 아래키 누르면 true 되면 space 입력받고 떨어지기    
    {
        Debug.Log("Fall!");
    }
    public void TakeDamage(float damage, Vector3 knockbackDirection)
    {
        Debug.Log($"플레이어 - 데이미 입음 : {damage}, 밀려남 : {knockbackDirection}");
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckHalfCircle);
        }
    }
    private void CreateAfterImage(float alpha)
    {
        GameObject clone = Instantiate(DashAfterImage, transform.position, Quaternion.identity); //복사 주기를 점차 줄인다.


        SpriteRenderer CtrlVsr = clone.GetComponent<SpriteRenderer>();
        CtrlVsr.sprite = spriter.sprite;

        CtrlVsr.flipX = spriter.flipX;

        CtrlVsr.sortingLayerID = spriter.sortingLayerID;

        CtrlVsr.sortingOrder = spriter.sortingOrder - 1;
        Color c = CtrlVsr.color;
        c.a = alpha;
        CtrlVsr.color = c;
    }
}
