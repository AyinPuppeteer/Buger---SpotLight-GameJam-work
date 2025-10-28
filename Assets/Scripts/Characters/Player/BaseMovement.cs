using UnityEngine;
using System.Collections.Generic;

public class BaseMovement : CharacterBase
{
    // 单例实例
    public static BaseMovement Instance { get; private set; }

    [Header("Detection Settings")]
    [SerializeField] protected bool isDetectable = true; // 是否可被敌人和扫描线探测
    public bool IsDetectable_ { get => isDetectable; }

    protected bool isExposed;//处于暴露状态（定位追捕）
    public bool IsExposed_ { get => isExposed; }

    [Header("Visual Settings")]
    [SerializeField] protected float detectableAlpha = 1f;    // 可被发现时的透明度
    [SerializeField] protected float undetectableAlpha = 0f;  // 不可被发现时的透明度（改为0，全透明）
    [SerializeField] protected float alphaLerpSpeed = 5f;     // 透明度变化速度

    [Header("Climb Settings")]
    [SerializeField] protected float climbSpeed = 1.0f;

    [Header("BUG Settings")]
    [SerializeField] private float penetrationSpeedThreshold = 15f; // 穿透所需的最小速度
    [SerializeField] private float penetrationCooldown = 1.0f; // 穿透冷却时间
    [SerializeField] private bool drawPenetrationDebug = true; // 穿透调试显示

    protected bool isClimbing = false;
    public bool IsClimbing_ { get => isClimbing; }
    protected bool canClimb = false;
    public bool CanClimb_ { get => canClimb; }
    protected bool canLeave = false;

    // SpriteRenderer相关
    protected List<SpriteRenderer> childSpriteRenderers = new List<SpriteRenderer>();
    protected Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();
    protected float currentAlpha = 1f;
    protected float targetAlpha = 1f;

    // 交互系统：使用列表管理多个交互对象
    private List<I_Interacts> interactables = new List<I_Interacts>();
    private I_Interacts currentInteractable = null;

    // BUG相关变量
    private bool canPenetrate = false;
    private bool isPenetrating = false;
    private float lastPenetrationTime = 0f;
    private Vector2 penetrationDirection = Vector2.zero;
    private LayerMask originalObstacleLayerMask;

    private bool firstBug = false;
    private Collider2D collider2d;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        canJump = true; // 玩家可以跳跃

        // 获取碰撞体组件
        collider2d = GetComponent<Collider2D>();

        // 保存原始的障碍物图层掩码
        originalObstacleLayerMask = obstacleLayerMask;

        // 收集所有子物体的SpriteRenderer
        CollectChildSpriteRenderers();

        // 设置初始透明度
        currentAlpha = isDetectable ? detectableAlpha : undetectableAlpha;
        targetAlpha = currentAlpha;
        UpdateSpriteAlpha();
    }

    protected override void Update()
    {
        // 确保只有单例实例执行更新逻辑
        if (Instance != this) return;

        HandleTriggerButton();
        UpdateAlphaTransition();
        UpdateCurrentInteractable(); // 更新当前交互对象

        // 检查是否可以触发穿透BUG
        CheckPenetrationBug();

        base.Update();

        if (!isDetectable)
        {
            gameObject.gameObject.layer = 8;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = 8;
            }
        }
        else
        {
            gameObject.gameObject.layer = 7;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = 7;
            }
        }
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

    protected override void HandleVisualLayer()
    {
        base.HandleVisualLayer();
        if (isWalking) animator.SetBool(param[0], true);
        else animator.SetBool(param[0], false);
        if (isSneaking) animator.SetBool(param[1], true);
        else animator.SetBool(param[1], false);
    }

    /// <summary>
    /// 检查是否可以触发穿透BUG
    /// </summary>
    private void CheckPenetrationBug()
    {
        // 计算总速度（包括外部速度）
        Vector2 totalVelocity = new Vector2(horizontal * speed, verticalVelocity) + externalVelocity;
        float totalSpeed = totalVelocity.magnitude;

        // 检查冷却时间
        bool cooldownOver = Time.time - lastPenetrationTime >= penetrationCooldown;

        // 如果总速度超过阈值且不在冷却中，可以触发穿透
        if (totalSpeed >= penetrationSpeedThreshold && cooldownOver && !isPenetrating)
        {
            canPenetrate = true;

            // 绘制调试信息
            if (drawPenetrationDebug)
            {
                Debug.DrawRay(transform.position, totalVelocity.normalized * 2f, Color.magenta, 0.1f);
            }
        }
        else
        {
            canPenetrate = false;
        }
    }

    /// <summary>
    /// 处理穿透BUG
    /// </summary>
    private void HandlePenetrationBug()
    {
        if (!canPenetrate || isPenetrating) return;

        // 计算总速度方向
        Vector2 totalVelocity = new Vector2(horizontal * speed, verticalVelocity) + externalVelocity;
        penetrationDirection = totalVelocity.normalized;

        // 检查前方是否有Block
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            penetrationDirection,
            playerWidth * 2f,
            obstacleLayerMask
        );

        if (hit.collider != null)
        {
            // 开始穿透
            StartPenetration();
        }
    }

    /// <summary>
    /// 开始穿透状态
    /// </summary>
    private void StartPenetration()
    {
        isPenetrating = true;
        canPenetrate = false;

        // 记录穿透时间
        lastPenetrationTime = Time.time;

        // 临时忽略障碍物碰撞
        obstacleLayerMask = 0;

        // 打印BUG信息
        AlertPrinter.Instance.PrintLog("错误：实体速度异常，发生穿透现象！", LogType.错误);

        // 绘制调试射线
        if (drawPenetrationDebug)
        {
            Debug.DrawRay(transform.position, penetrationDirection * playerWidth * 2f, Color.red, 1f);
        }
    }

    /// <summary>
    /// 结束穿透状态
    /// </summary>
    private void EndPenetration()
    {
        if (!isPenetrating) return;

        // 恢复障碍物碰撞检测
        obstacleLayerMask = originalObstacleLayerMask;

        // 重置所有速度
        verticalVelocity = 0f;
        externalVelocity = Vector2.zero;
        horizontal = 0f;
        vertical = 0f;

        // 结束穿透状态
        isPenetrating = false;

        // 打印结束信息
        AlertPrinter.Instance.PrintLog("错误修正：实体速度恢复正常", LogType.错误);
    }

    /// <summary>
    /// 检查是否应该结束穿透状态
    /// </summary>
    private void CheckPenetrationEnd()
    {
        if (!isPenetrating) return;

        // 检查是否已经穿过障碍物
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            penetrationDirection,
            playerWidth * 2f,
            originalObstacleLayerMask
        );

        // 如果前方没有障碍物，结束穿透状态
        if (hit.collider == null)
        {
            EndPenetration();
        }
    }

    /// <summary>
    /// 收集所有子物体的SpriteRenderer
    /// </summary>
    protected virtual void CollectChildSpriteRenderers()
    {
        childSpriteRenderers.Clear();
        originalColors.Clear();

        // 获取所有子物体（包括孙子物体）的SpriteRenderer
        SpriteRenderer[] allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in allSpriteRenderers)
        {
            // 跳过自身的SpriteRenderer（如果有），只处理子物体
            if (spriteRenderer.gameObject != gameObject)
            {
                childSpriteRenderers.Add(spriteRenderer);
                originalColors[spriteRenderer] = spriteRenderer.color;
            }
        }
    }

    /// <summary>
    /// 处理透明度过渡
    /// </summary>
    protected virtual void UpdateAlphaTransition()
    {
        // 根据可探测状态设置目标透明度
        targetAlpha = isDetectable ? detectableAlpha : undetectableAlpha;

        // 平滑过渡透明度
        if (Mathf.Abs(currentAlpha - targetAlpha) > 0.01f)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, alphaLerpSpeed * Time.deltaTime);
            UpdateSpriteAlpha();
        }
    }

    /// <summary>
    /// 更新所有子物体SpriteRenderer的透明度
    /// </summary>
    protected virtual void UpdateSpriteAlpha()
    {
        foreach (SpriteRenderer spriteRenderer in childSpriteRenderers)
        {
            if (spriteRenderer != null)
            {
                Color newColor = originalColors[spriteRenderer];
                newColor.a = currentAlpha;
                spriteRenderer.color = newColor;
            }
        }
    }

    protected override void GetInput()
    {
        // 如果不可探测，禁止移动输入
        if (!isDetectable)
        {
            inputVector = Vector2.zero;
            horizontal = 0f;
            vertical = 0f;
        }
        else
        {
            inputVector = GameInput.Instance.GetMovementInput();
            horizontal = inputVector.x;
            vertical = inputVector.y;
        }
    }

    private void HandleTriggerButton()
    {
        // 处理交互按钮逻辑 - 无论是否可探测都应该可以交互
        if (GameInput.Instance.InteractInputDown())
        {
            if (currentInteractable != null)
            {
                currentInteractable.TakeInteract();
            }
        }

        // 如果不可探测，禁止跳跃
        if (!isDetectable) return;

        // 接收跳跃按钮
        if (GameInput.Instance.GetJumpInputDown())
        {
            jumpRequested = true;
        }
    }

    /// <summary>
    /// 处理角色移动逻辑（添加攀爬功能）
    /// </summary>
    protected override void HandleMovement()
    {
        GetInput();

        // 如果不可探测，禁止移动
        if (!isDetectable)
        {
            speed = 0;
            verticalVelocity = 0;
            return;
        }

        if (inputVector != Vector2.zero)
        {
            if (horizontal < -deadZone) toRight = false;
            else if (horizontal > deadZone) toRight = true;

            if (Mathf.Abs(horizontal) > deadZone && canLeave)
            {
                canClimb = false;
                isClimbing = false;
                canLeave = false;
            }

            // 攀爬逻辑
            if (Mathf.Abs(vertical) > deadZone && canClimb)
            {
                isClimbing = true;
                if (canLeave && firstBug)
                {
                    AlertPrinter.Instance.PrintLog("错误：实体试图攀爬，但未检测到物体：梯子！", LogType.错误);
                    firstBug = false;
                }
            }
            else if (isGrounded && Mathf.Abs(horizontal) > deadZone)
            {
                if (vertical < -0.5f)
                    isSneaking = true;
                else
                {
                    isSneaking = false;
                    isWalking = true;
                }
            }
            else
            {
                isSneaking = false;
                isWalking = false;
            }

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

        // 检查并处理穿透BUG
        if (!isPenetrating)
        {
            HandlePenetrationBug();
        }
        else
        {
            // 如果正在穿透，检查是否应该结束穿透
            CheckPenetrationEnd();
        }

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

        move.y *= bug2Active ? -1f : 1f;

        // 应用移动
        rb.MovePosition(rb.position + move);
    }

    /// <summary>
    /// 处理物理力和重力（添加攀爬时的重力豁免）
    /// </summary>
    protected override void HandleForce()
    {
        // 如果不可探测，禁止跳跃和重力
        if (!isDetectable)
        {
            verticalVelocity = 0;
            return;
        }

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
    /// 激活BUG2效果
    /// </summary>
    public virtual void ActivateBUG2()
    {
        // 翻转所有sprite子物体
        FlipAllSprites();

        // 翻转摄像机
        FlipCamera();

        bug2Active = !bug2Active;

        AlertPrinter.Instance.PrintLog("错误：加速度过快，翻转单位重力！", LogType.错误);
    }

    /// <summary>
    /// 翻转所有sprite子物体
    /// </summary>
    public virtual void FlipAllSprites()
    {
        foreach (SpriteRenderer spriteRenderer in childSpriteRenderers)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipY = !spriteRenderer.flipY;
            }
        }
    }

    /// <summary>
    /// 翻转摄像机
    /// </summary>
    public virtual void FlipCamera()
    {
        if (MainCamera.Instance != null)
        {
            MainCamera.Instance.FlipCamera();
        }
    }

    /// <summary>
    /// 更新当前交互对象
    /// </summary>
    private void UpdateCurrentInteractable()
    {
        if (interactables.Count == 0)
        {
            currentInteractable = null;
            return;
        }

        // 优先选择非梯子的交互对象（如柜子）
        I_Interacts preferred = null;
        foreach (I_Interacts interactable in interactables)
        {
            // 柜子（HideField）优先于梯子
            if (!(interactable is Ladder))
            {
                preferred = interactable;
                break;
            }
        }

        // 如果没有柜子，选择第一个交互对象
        currentInteractable = preferred ?? interactables[0];
    }

    /// <summary>
    /// 触发器进入事件处理（梯子检测）
    /// </summary>
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Ladder ladder = other.GetComponent<Ladder>();
        I_PickItem pickItem = other.GetComponent<I_PickItem>();
        I_Interacts interacts = other.GetComponent<I_Interacts>();

        if (ladder != null)
        {
            canClimb = true;
        }
        if (pickItem != null)
        {
            pickItem.Pick();
        }
        // 添加交互对象到列表
        if (interacts != null && !interactables.Contains(interacts))
        {
            interactables.Add(interacts);
        }

        HandleEnemyCollision(other.gameObject);
    }

    /// <summary>
    /// 触发器退出事件处理（梯子检测）
    /// </summary>
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        I_Interacts interacts = other.GetComponent<I_Interacts>();
        // 从交互对象列表中移除
        if (interacts != null && interactables.Contains(interacts))
        {
            interactables.Remove(interacts);
        }

        Ladder ladder = other.GetComponent<Ladder>();
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
        if (guard != null && isDetectable)
        {
            // 直接调用 GameManager 的 GameOver 方法
            GameManager.Instance.GameOver();
        }
    }

    /// <summary>
    /// 设置角色是否可被敌人和扫描线探测
    /// </summary>
    public void SetDetectable(bool detectable)
    {
        isDetectable = detectable;

        // 当恢复可探测状态时，确保碰撞体恢复正常
        if (isDetectable && collider2d != null)
        {
            collider2d.isTrigger = false;
        }

        //如果不可探测，则取消暴露状态
        if (!isDetectable) SetExposed(false);
    }

    //设置角色是否处于暴露状态
    public void SetExposed(bool exposed)
    {
        if (isExposed != exposed)
        {
            isExposed = exposed;
            if (exposed)
            {
                GameManager.Instance.PlayerExposed();
                AlertPrinter.Instance.PrintLog("警告：检测到未知实体！所在位置：(" + (int)(transform.position.x / 0.16f) + ", " + (int)(transform.position.y / 0.16f) + ")", LogType.警告);
            }
            else
            {
                GameManager.Instance.PlayerDisexposed();
            }
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // 绘制穿透BUG相关的调试信息
        if (Application.isPlaying)
        {
            // 绘制总速度向量
            Vector2 totalVelocity = new Vector2(horizontal * speed, verticalVelocity) + externalVelocity;
            Gizmos.color = canPenetrate ? Color.magenta : Color.cyan;
            Gizmos.DrawRay(transform.position, totalVelocity.normalized * 2f);

            // 绘制穿透阈值圆
            Gizmos.color = canPenetrate ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, penetrationSpeedThreshold * 0.1f);

            // 显示穿透状态
            GUIStyle style = new GUIStyle();
            style.normal.textColor = isPenetrating ? Color.red : (canPenetrate ? Color.yellow : Color.white);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1f,
                isPenetrating ? "PENETRATING" : (canPenetrate ? "CAN PENETRATE" : "Normal"), style);
#endif
        }
    }
}