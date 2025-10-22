using UnityEngine;

public class BaseMovement : CharacterBase
{
    // 单例实例
    public static BaseMovement Instance { get; private set; }

    [Header("Climb Settings")]
    [SerializeField] protected float climbSpeed = 1.0f;

    protected bool isClimbing = false;
    protected bool canClimb = false;
    protected bool canLeave = false;

    private I_Interacts i_interacts;

    private bool firstBug = false;

    // 暴露属性
    public bool IsClimbing_ { get => isClimbing; }
    public bool CanClimb_ { get => canClimb; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        canJump = true; // 玩家可以跳跃
    }

    protected override void Update()
    {
        // 确保只有单例实例执行更新逻辑
        if (Instance != this) return;

        HandleTriggerButton();
        base.Update();
    }

    protected override void FixedUpdate()
    {
        // 确保只有单例实例执行固定更新逻辑
        if (Instance != this) return;

        base.FixedUpdate();
    }

    protected void OnDestroy()
    {
        // 清理单例引用
        if (Instance == this)
        {
            Instance = null;
        }
    }

    protected override void GetInput()
    {
        inputVector = GameInput.Instance.GetMovementInput();
        horizontal = inputVector.x;
        vertical = inputVector.y;
    }

    private void HandleTriggerButton()
    {
        // 接收跳跃按钮
        if (GameInput.Instance.GetJumpInputDown())
        {
            jumpRequested = true;
        }

        // 处理交互按钮逻辑
        if (GameInput.Instance.InteractInputDown())
        {
            if(i_interacts != null)
            {
                i_interacts.TakeInteract();
            }
        }
    }

    /// <summary>
    /// 处理角色移动逻辑（添加攀爬功能）
    /// </summary>
    protected override void HandleMovement()
    {
        GetInput();

        if (inputVector != Vector2.zero)
        {
            if (horizontal < -deadZone) toRight = false;
            else if (horizontal > deadZone) toRight = true;

            if (Mathf.Abs(horizontal) > deadZone && canLeave)
            {
                canClimb = false;
                isClimbing = false;
            }

            // 攀爬逻辑
            if (Mathf.Abs(vertical) > deadZone && canClimb)
            {
                isClimbing = true;
                if (canLeave && firstBug)
                {
                    AlertPrinter.Instance.PrintLog("错误：未检测到物体：梯子。", LogType.错误);
                    firstBug = false;
                }
                }
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

        // 简化的碰撞检测：先水平后垂直
        if (Mathf.Abs(move.x) > deadZone)
        {
            CheckHorizontalCollision(ref move);
        }

        // 垂直碰撞检测只在上方有障碍物时阻止移动
        if (move.y > 0) // 只在上移时检测上方碰撞
        {
            CheckVerticalCollision(ref move);

        }

        // 应用移动
        rb.MovePosition(rb.position + move);
    }

    /// <summary>
    /// 处理物理力和重力（添加攀爬时的重力豁免）
    /// </summary>
    protected override void HandleForce()
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

    /// <summary>
    /// 触发器进入事件处理（梯子检测）
    /// </summary>
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Ladder ladder = other.GetComponent<Ladder>();
        I_PickItem pickItem = other.GetComponent<I_PickItem>();
        i_interacts = other.GetComponent<I_Interacts>();

        if (ladder != null)
        {
            canLeave = false;
            canClimb = true;
        }
        if (pickItem != null)
        {
            pickItem.Pick();
        }

        // 敌人碰撞检测
        HandleEnemyCollision(other.gameObject);
    }

    /// <summary>
    /// 触发器退出事件处理（梯子检测）
    /// </summary>
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        Ladder ladder = other.GetComponent<Ladder>();
        i_interacts = null;
        if (ladder != null)
        {
            canLeave = true;
            firstBug = true;
        }
    }

    /// <summary>
    /// 碰撞进入事件处理
    /// </summary>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        HandleEnemyCollision(collision.gameObject);
    }

    /// <summary>
    /// 处理敌人碰撞逻辑
    /// </summary>
    private void HandleEnemyCollision(GameObject other)
    {
        Guard guard = other.GetComponent<Guard>();
        if (guard != null)
        {
            // 直接调用 GameManager 的 GameOver 方法
            GameManager.Instance.GameOver();
        }
    }
}