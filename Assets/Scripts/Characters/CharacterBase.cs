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
    [SerializeField] protected float groundCheckDistance = 0.1f;

    [Header("Force Settings")]
    [SerializeField] protected float gravity = 9.8f;
    [SerializeField] protected float jumpForce = 5.0f;

    [Header("Layer Settings")]
    [SerializeField] protected LayerMask obstacleLayerMask = 1 << 3; // Block图层
    [SerializeField] protected LayerMask groundLayerMask = 1 << 3;   // Block图层
    [SerializeField] protected LayerMask enemyLayerMask = 1 << 7;    // Player图层（敌人也用这个图层）

    protected Rigidbody2D rb;

    protected float speed;
    protected bool isWalking = false;
    protected bool isClimbing = false;
    protected bool isSneaking = false;
    protected bool isGrounded = false;
    protected bool toRight = true;
    protected bool canMove = true; // 默认可以移动
    protected bool canClimb = false;
    protected bool canJump = false;

    protected float horizontal = 0f;
    protected float vertical = 0f;

    protected Vector2 inputVector = Vector2.zero;
    protected Vector2 moveDir = Vector2.zero;

    protected float verticalVelocity = 0f;
    protected bool jumpRequested = false;
    protected bool wasGrounded = false; // 上一帧的地面状态

    // 暴露属性
    public bool IsGrounded_ { get => isGrounded; }
    public bool IsClimbing_ { get => isClimbing; }
    public bool IsSneaking_ { get => isSneaking; }
    public bool CanClimb_ { get => canClimb; }
    public bool CanJump_ { get => canJump; }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        // 设置碰撞图层
        gameObject.layer = LayerMask.NameToLayer("Player"); // 玩家和敌人都用Player图层
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

        // 只在非爬梯状态下应用垂直速度
        if (!canClimb)
        {
            move.y += verticalVelocity * Time.fixedDeltaTime;
        }
        else
        {
            // 爬梯时使用输入控制垂直移动
            move.y = vertical * climbSpeed * Time.fixedDeltaTime;
            verticalVelocity = 0f; // 爬梯时重置垂直速度
        }

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
            jumpRequested = false; // 确保跳跃请求被消耗
        }

        if (!isGrounded && !canClimb)
        {
            verticalVelocity -= gravity * Time.fixedDeltaTime;
        }

        // 在地面时确保垂直速度不会为负
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0;
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

            Debug.Log("Landed on ground");
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

    // 改进：只检查水平方向的碰撞
    protected virtual void CheckHorizontalCollision(ref Vector2 move)
    {
        // 只检查水平方向的碰撞
        Vector2 horizontalMove = new Vector2(move.x, 0);
        if (horizontalMove.magnitude > 0)
        {
            // 从角色中心偏上位置检测，避免检测到脚下的地面
            Vector2 boxCenter = (Vector2)transform.position + new Vector2(0, playerHeight / 4);

            RaycastHit2D hit = Physics2D.BoxCast(
                boxCenter,
                new Vector2(playerWidth, playerHeight / 2), // 更小的检测区域
                0,
                horizontalMove.normalized,
                horizontalMove.magnitude,
                obstacleLayerMask);

            if (hit.collider != null)
            {
                // 有碰撞，调整水平移动距离
                move.x = horizontalMove.normalized.x * hit.distance;

                // 调试信息
                Debug.Log($"Horizontal collision with: {hit.collider.name}, distance: {hit.distance}");
            }
        }
    }

    // 改进地面检测
    protected virtual void CheckGrounded()
    {
        Vector2 rayStart = (Vector2)transform.position;
        float rayLength = playerHeight / 2 + groundCheckDistance;

        // 发射多条射线提高检测准确性
        RaycastHit2D hitCenter = Physics2D.Raycast(rayStart, Vector2.down, rayLength, groundLayerMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(rayStart + Vector2.left * playerWidth / 3, Vector2.down, rayLength, groundLayerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(rayStart + Vector2.right * playerWidth / 3, Vector2.down, rayLength, groundLayerMask);

        isGrounded = hitCenter.collider != null || hitLeft.collider != null || hitRight.collider != null;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // 通过检测Ladder组件来判断是否为梯子
        Ladder ladder = other.GetComponent<Ladder>();
        if (ladder != null)
        {
            canClimb = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        // 通过检测Ladder组件来判断是否为梯子
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

    // 在编辑器中可视化检测区域
    protected virtual void OnDrawGizmosSelected()
    {
        // 绘制地面检测射线
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector2 rayStart = transform.position;
        float rayLength = playerHeight / 2 + groundCheckDistance;
        Gizmos.DrawLine(rayStart, rayStart + Vector2.down * rayLength);
        Gizmos.DrawLine(rayStart + Vector2.left * playerWidth / 3, rayStart + Vector2.left * playerWidth / 3 + Vector2.down * rayLength);
        Gizmos.DrawLine(rayStart + Vector2.right * playerWidth / 3, rayStart + Vector2.right * playerWidth / 3 + Vector2.down * rayLength);

        // 绘制水平碰撞检测区域
        Gizmos.color = Color.blue;
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(0, playerHeight / 4);
        Gizmos.DrawWireCube(boxCenter, new Vector2(playerWidth, playerHeight / 2));
    }
}