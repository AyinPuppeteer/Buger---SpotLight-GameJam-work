using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float originSpeed = 5.0f;
    [SerializeField] protected float sneakSpeed = 2.5f;
    [SerializeField] protected float climbSpeed = 2.0f;

    [Header("Collision Settings")]
    [SerializeField] protected float playerWidth = .35f;
    [SerializeField] protected float playerHeight = .5f;
    [SerializeField] protected float deadZone = 0.1f;
    [SerializeField] protected float groundCheckDistance = 0.001f;

    [Header("Force Settings")]
    [SerializeField] protected float gravity = 20f;
    [SerializeField] protected float jumpForce = 8f;
    [SerializeField] protected float maxFallSpeed = 20f; // 最大下落速度

    [Header("Layer Settings")]
    [SerializeField] protected LayerMask obstacleLayerMask = 1 << 3; // Block图层
    [SerializeField] protected LayerMask groundLayerMask = 1 << 3;   // Block图层
    [SerializeField] protected LayerMask enemyLayerMask = 1 << 7;    // Player图层

    [Header("Detection Settings")]
    [SerializeField] protected bool isDetectable = true; // 是否可被敌人和扫描线探测

    protected Rigidbody2D rb;

    protected float speed;
    protected bool isWalking = false;
    protected bool isClimbing = false;
    protected bool isSneaking = false;
    protected bool isGrounded = false;
    protected bool toRight = true;
    protected bool canMove = true;
    protected bool canClimb = false;
    protected bool canJump = false;

    protected float horizontal = 0f;
    protected float vertical = 0f;

    protected Vector2 inputVector = Vector2.zero;
    protected Vector2 moveDir = Vector2.zero;

    protected float verticalVelocity = 0f;
    protected bool jumpRequested = false;
    protected bool wasGrounded = false;

    // 外部施加的加速度
    protected Vector2 externalAcceleration = Vector2.zero;
    protected Vector2 externalVelocity = Vector2.zero;

    // 暴露属性
    public bool IsGrounded_ { get => isGrounded; }
    public bool IsClimbing_ { get => isClimbing; }
    public bool IsSneaking_ { get => isSneaking; }
    public bool CanClimb_ { get => canClimb; }
    public bool CanJump_ { get => canJump; }
    public bool IsDetectable_ { get => isDetectable; }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    protected virtual void Update()
    {
        HandleVisualLayer();
    }

    protected virtual void FixedUpdate()
    {
        // 先检测地面状态
        wasGrounded = isGrounded;
        CheckGrounded();

        // 处理刚落地的情况
        HandleLanding();

        // 处理移动和力
        HandleForce();
        HandleExternalForces(); // 处理外部施加的力
        HandleMovement();
    }

    protected virtual void HandleMovement()
    {
        GetInput();

        if (inputVector != Vector2.zero)
        {
            if (horizontal < -deadZone) toRight = false;
            else if (horizontal > deadZone) toRight = true;

            if (Mathf.Abs(vertical) > deadZone && canClimb)
                isClimbing = true;
            else if (vertical < -0.5f && isGrounded)
                isSneaking = true;
            else if (vertical > -deadZone || !isGrounded)
                isSneaking = false;

            if (!canClimb || Mathf.Abs(vertical) < deadZone)
                isClimbing = false;

            speed = isClimbing ? climbSpeed :
                   isSneaking ? sneakSpeed : originSpeed;
        }
        else
        {
            if (isWalking) isWalking = false;
            if (isClimbing) isClimbing = false;
            if (isSneaking) isSneaking = false;
            speed = 0;
        }

        Vector2 move = new Vector2(horizontal, 0) * speed * Time.fixedDeltaTime;

        // 只在非可爬梯状态下应用垂直速度
        if (!canClimb)
        {
            move.y += verticalVelocity * Time.fixedDeltaTime;
        }
        else
        {
            // 爬梯时使用输入控制垂直移动
            move.y = vertical * climbSpeed * Time.fixedDeltaTime;
            verticalVelocity = 0f;
        }

        // 添加外部速度的影响
        move += externalVelocity * Time.fixedDeltaTime;

        // 只检查水平方向的碰撞
        if (Mathf.Abs(horizontal) > deadZone)
        {
            CheckHorizontalCollision(ref move);
        }

        // 应用移动
        rb.MovePosition(rb.position + move);
    }

    protected virtual void HandleForce()
    {
        // 跳跃逻辑
        if (isGrounded && jumpRequested && canJump)
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
            jumpRequested = false;
        }

        // 只在非爬梯和非地面状态下应用重力
        if (!isGrounded && !canClimb)
        {
            verticalVelocity -= gravity * Time.fixedDeltaTime;

            // 限制最大下落速度
            if (verticalVelocity < -maxFallSpeed)
            {
                verticalVelocity = -maxFallSpeed;
            }
        }

        // 在地面时确保垂直速度不会为负
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0;
        }
    }

    protected virtual void HandleExternalForces()
    {
        // 应用外部加速度到外部速度
        externalVelocity += externalAcceleration * Time.fixedDeltaTime;

        // 逐渐衰减外部速度（模拟阻力）
        externalVelocity *= 0.95f;

        // 如果外部速度很小，直接设为0
        if (externalVelocity.magnitude < 0.1f)
        {
            externalVelocity = Vector2.zero;
        }
    }

    protected virtual void HandleLanding()
    {
        // 如果刚刚落地
        if (isGrounded && !wasGrounded)
        {
            // 确保跳跃请求被重置
            jumpRequested = false;

            // 重置垂直速度
            verticalVelocity = 0;

            // 落地时重置外部Y轴速度（防止弹跳）
            externalVelocity = new Vector2(externalVelocity.x, 0);
        }
    }

    protected virtual void GetInput()
    {
        // 由子类实现具体的输入获取
    }

    protected virtual void HandleVisualLayer()
    {
        Vector3 localScale = transform.localScale;
        if (toRight)
            localScale.x = Mathf.Abs(localScale.x);
        else
            localScale.x = -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }

    protected virtual void CheckHorizontalCollision(ref Vector2 move)
    {
        Vector2 horizontalMove = new Vector2(move.x, 0);
        if (horizontalMove.magnitude > 0)
        {
            Vector2 boxCenter = (Vector2)transform.position + new Vector2(0, playerHeight / 4);

            RaycastHit2D hit = Physics2D.BoxCast(
                boxCenter,
                new Vector2(playerWidth, playerHeight / 2),
                0,
                horizontalMove.normalized,
                horizontalMove.magnitude,
                obstacleLayerMask);

            if (hit.collider != null)
            {
                move.x = horizontalMove.normalized.x * hit.distance;
            }
        }
    }

    protected virtual void CheckGrounded()
    {
        Vector2 rayStart = (Vector2)transform.position;
        float rayLength = playerHeight / 2 + groundCheckDistance;

        RaycastHit2D hitCenter = Physics2D.Raycast(rayStart, Vector2.down, rayLength, groundLayerMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(rayStart + Vector2.left * playerWidth / 3, Vector2.down, rayLength, groundLayerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(rayStart + Vector2.right * playerWidth / 3, Vector2.down, rayLength, groundLayerMask);

        isGrounded = hitCenter.collider != null || hitLeft.collider != null || hitRight.collider != null;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Ladder ladder = other.GetComponent<Ladder>();
        if (ladder != null)
        {
            canClimb = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        Ladder ladder = other.GetComponent<Ladder>();
        if (ladder != null)
        {
            canClimb = false;
            isClimbing = false;
        }
    }

    // 公共方法，允许外部触发跳跃
    public void RequestJump()
    {
        if (canJump && isGrounded)
        {
            jumpRequested = true;
        }
    }

    // ========== 新增功能 ==========

    /// 设置角色是否可被敌人和扫描线探测，
    public void SetDetectable(bool detectable)
    {
        isDetectable = detectable;
    }

    /// 施加一个持续的加速度
    public void ApplyAcceleration(Vector2 acceleration)
    {
        externalAcceleration = acceleration;
    }

    /// 施加一个瞬间的力（直接改变速度）
    public void ApplyImpulse(Vector2 force)
    {
        externalVelocity += force;
    }

    /// 清除所有外部施加的力
    public void ClearExternalForces()
    {
        externalAcceleration = Vector2.zero;
        externalVelocity = Vector2.zero;
    }

    /// 获取当前外部速度
    public Vector2 GetExternalVelocity()
    {
        return externalVelocity;
    }

    // ==============================

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector2 rayStart = transform.position;
        float rayLength = playerHeight / 2 + groundCheckDistance;
        Gizmos.DrawLine(rayStart, rayStart + Vector2.down * rayLength);
        Gizmos.DrawLine(rayStart + Vector2.left * playerWidth / 3, rayStart + Vector2.left * playerWidth / 3 + Vector2.down * rayLength);
        Gizmos.DrawLine(rayStart + Vector2.right * playerWidth / 3, rayStart + Vector2.right * playerWidth / 3 + Vector2.down * rayLength);

        Gizmos.color = Color.blue;
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(0, playerHeight / 4);
        Gizmos.DrawWireCube(boxCenter, new Vector2(playerWidth, playerHeight / 2));

        // 绘制外部速度向量
        if (externalVelocity.magnitude > 0.1f)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, externalVelocity * 0.5f);
        }
    }
}