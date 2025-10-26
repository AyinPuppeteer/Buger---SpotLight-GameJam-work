using UnityEngine;
using System.Collections.Generic;

public class CharacterBase : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float originSpeed = 2.5f;
    [SerializeField] protected float sneakSpeed = 0.5f;

    [Header("Collision Settings")]
    [SerializeField] protected float playerWidth = .16f;
    [SerializeField] protected float playerHeight = .32f;
    [SerializeField] protected float deadZone = 0.1f;
    [SerializeField] protected float groundCheckDistance = 0.01f;

    [Header("Improved Collision Detection")]
    [SerializeField] protected int horizontalRays = 3;    // 水平方向射线数量
    [SerializeField] protected int verticalRays = 3;      // 垂直方向射线数量

    [Header("Force Settings")]
    [SerializeField] protected float gravity = 18f;
    [SerializeField] protected float jumpForce = 5f;
    [SerializeField] protected float maxFallSpeed = 5f; // 最大下落速度

    [Header("Layer Settings")]
    [SerializeField] protected LayerMask obstacleLayerMask = 1 << 3; // Block图层
    [SerializeField] protected LayerMask groundLayerMask = 1 << 3;   // Block图层
    [SerializeField] protected LayerMask enemyLayerMask = 1 << 7;    // Player图层
    [SerializeField] protected LayerMask playerHiddenLayerMask = 1 << 8;

    [Header("BUG2 Settings")]
    [SerializeField] protected bool bug2Active = false;     // BUG2是否激活

    [Header("Animtor Setting")]
    [SerializeField] protected Animator animator;             // 动画器
    [SerializeField] protected string[] param;

    protected Rigidbody2D rb;

    protected float speed;
    protected bool isWalking = false;
    protected bool isSneaking = false;
    protected bool isGrounded = false;
    protected bool toRight = true;
    protected bool canMove = true;
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
    public bool IsSneaking_ { get => isSneaking; }
    public bool CanJump_ { get => canJump; }

    /// <summary>
    /// 初始化组件和参数
    /// </summary>
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    /// <summary>
    /// 每帧更新视觉表现
    /// </summary>
    protected virtual void Update()
    {
        HandleVisualLayer();
    }

    /// <summary>
    /// 固定时间步长更新物理相关逻辑
    /// </summary>
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

    /// <summary>
    /// 处理角色移动逻辑
    /// </summary>
    protected virtual void HandleMovement()
    {
        GetInput();

        if (inputVector != Vector2.zero)
        {
            if (horizontal < -deadZone) toRight = false;
            else if (horizontal > deadZone) toRight = true;

            if (vertical < -0.5f && isGrounded)
                isSneaking = true;
            else if (vertical > -deadZone || !isGrounded)
                isSneaking = false;

            speed = isSneaking ? sneakSpeed : originSpeed;
        }
        else
        {
            if (isWalking) isWalking = false;
            if (isSneaking) isSneaking = false;
            speed = 0;
        }

        Vector2 move = new Vector2(horizontal, 0) * speed * Time.fixedDeltaTime;

        // 应用垂直速度（重力影响）
        move.y += verticalVelocity * Time.fixedDeltaTime;

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
    /// 处理物理力和重力
    /// </summary>
    protected virtual void HandleForce()
    {
        // 跳跃逻辑
        if (isGrounded && jumpRequested && canJump)
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
            jumpRequested = false;
        }

        // 只在非地面状态下应用重力
        if (!isGrounded)
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
    /// 处理外部施加的力
    /// </summary>
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

    /// <summary>
    /// 处理角色落地时的逻辑
    /// </summary>
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

    /// <summary>
    /// 获取输入信息（由子类实现）
    /// </summary>
    protected virtual void GetInput()
    {
        // 由子类实现具体的输入获取
    }

    /// <summary>
    /// 处理视觉层表现（如角色朝向）
    /// </summary>
    protected virtual void HandleVisualLayer()
    {
        Vector3 localScale = transform.localScale;
        if (toRight)
            localScale.x = Mathf.Abs(localScale.x);
        else
            localScale.x = -Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }

    /// <summary>
    /// 简化的水平碰撞检测：检测到碰撞则将该方向速度设为0
    /// </summary>
    protected virtual void CheckHorizontalCollision(ref Vector2 move)
    {
        if (move.x == 0) return;

        float direction = Mathf.Sign(move.x);
        float rayLength = Mathf.Abs(move.x) + groundCheckDistance;

        // 在角色高度范围内发射多个水平射线
        for (int i = 0; i < horizontalRays; i++)
        {
            float lerpAmount = (horizontalRays == 1) ? 0.5f : (float)i / (horizontalRays - 1);
            Vector2 rayOrigin = (Vector2)transform.position +
                               new Vector2(0, Mathf.Lerp(-playerHeight / 2, playerHeight / 2, lerpAmount));

            // 添加微小的水平偏移
            rayOrigin.x += direction * playerWidth / 2;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,
                new Vector2(direction, 0),
                rayLength,
                obstacleLayerMask);

            if (hit.collider != null)
            {
                // 检测到碰撞，将该方向速度设为0
                move.x = 0;
                externalVelocity.x = 0; // 同时清除外部速度的水平分量
                break; // 找到一个碰撞就退出
            }
        }
    }

    /// <summary>
    /// 简化的垂直碰撞检测：只检测上方碰撞，防止跳跃时撞到天花板
    /// </summary>
    protected virtual void CheckVerticalCollision(ref Vector2 move)
    {
        if (move.y <= 0 && !bug2Active || bug2Active && move.y >= 0) return; // 只处理向上的移动

        float rayLength = Mathf.Abs(move.y) + groundCheckDistance;

        // 在角色宽度范围内发射多个垂直射线
        for (int i = 0; i < verticalRays; i++)
        {
            float lerpAmount = (verticalRays == 1) ? 0.5f : (float)i / (verticalRays - 1);
            Vector2 rayOrigin = (Vector2)transform.position +
                               new Vector2(Mathf.Lerp(-playerWidth / 2, playerWidth / 2, lerpAmount), 0);

            // 添加微小的垂直偏移
            rayOrigin.y += playerHeight / 2; // 从角色顶部发射

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,
                bug2Active ? Vector2.down : Vector2.up,
                rayLength,
                obstacleLayerMask);

            if (hit.collider != null)
            {
                // 检测到上方碰撞，将垂直速度设为0
                move.y = 0;
                verticalVelocity = 0; // 重置垂直速度
                break; // 找到一个碰撞就退出
            }
        }
    }

    /// <summary>
    /// 地面检测
    /// </summary>
    protected virtual void CheckGrounded()
    {
        Vector2 rayStart = (Vector2)transform.position;
        float rayLength = playerHeight / 2 + groundCheckDistance;

        RaycastHit2D hitCenter = Physics2D.Raycast(rayStart, bug2Active ? Vector2.up : Vector2.down, rayLength, groundLayerMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(rayStart + Vector2.left * playerWidth / 2, bug2Active ? Vector2.up : Vector2.down, rayLength, groundLayerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(rayStart + Vector2.right * playerWidth / 2, bug2Active ? Vector2.up : Vector2.down, rayLength, groundLayerMask);

        isGrounded = hitCenter.collider != null || hitLeft.collider != null || hitRight.collider != null;

        // 如果检测到地面，确保不会陷入地面
        if (isGrounded)
        {
            // 找到最近的碰撞点
            float minDistance = float.MaxValue;
            if (hitCenter.collider != null && hitCenter.distance < minDistance) minDistance = hitCenter.distance;
            if (hitLeft.collider != null && hitLeft.distance < minDistance) minDistance = hitLeft.distance;
            if (hitRight.collider != null && hitRight.distance < minDistance) minDistance = hitRight.distance;

            // 微调位置以确保正好在地面上
            float adjustment = minDistance - (playerHeight / 2);
            if (adjustment > 0.001f) // 只有需要调整时才进行
            {
                rb.position = new Vector2(rb.position.x, rb.position.y +
                    adjustment * (bug2Active ? 1f : -1f));
            }
        }
    }

    /// <summary>
    /// 请求跳跃
    /// </summary>
    public void RequestJump()
    {
        if (canJump && isGrounded)
        {
            jumpRequested = true;
        }
    }

    // ========== 新增功能 ==========

    /// <summary>
    /// 施加一个持续的加速度
    /// </summary>
    public void ApplyAcceleration(Vector2 acceleration)
    {
        externalAcceleration = acceleration;
    }

    /// <summary>
    /// 施加一个瞬间的力（直接改变速度）
    /// </summary>
    public void ApplyImpulse(Vector2 force)
    {
        externalVelocity += force;
    }

    /// <summary>
    /// 清除所有外部施加的力
    /// </summary>
    public void ClearExternalForces()
    {
        externalAcceleration = Vector2.zero;
        externalVelocity = Vector2.zero;
    }

    /// <summary>
    /// 获取当前外部速度
    /// </summary>
    public Vector2 GetExternalVelocity()
    {
        return externalVelocity;
    }

    // ==============================

    /// <summary>
    /// 绘制调试Gizmos
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector2 rayStart = transform.position;
        float rayLength = playerHeight / 2 + groundCheckDistance;
        Gizmos.DrawLine(rayStart, rayStart + Vector2.down * rayLength);
        Gizmos.DrawLine(rayStart + Vector2.left * playerWidth / 2, rayStart + Vector2.left * playerWidth / 2 + Vector2.down * rayLength);
        Gizmos.DrawLine(rayStart + Vector2.right * playerWidth / 2, rayStart + Vector2.right * playerWidth / 2 + Vector2.down * rayLength);

        Gizmos.color = Color.blue;
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(0, playerHeight / 4);
        Gizmos.DrawWireCube(boxCenter, new Vector2(playerWidth, playerHeight / 2));

        // 绘制外部速度向量
        if (externalVelocity.magnitude > 0.1f)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, externalVelocity * 0.5f);
        }

        // 绘制碰撞检测射线
        Gizmos.color = Color.cyan;

        // 绘制水平检测射线
        if (Application.isPlaying && Mathf.Abs(horizontal) > deadZone)
        {
            float direction = Mathf.Sign(horizontal);
            float rayLengthDebug = Mathf.Abs(horizontal * speed * Time.fixedDeltaTime) + groundCheckDistance;

            for (int i = 0; i < horizontalRays; i++)
            {
                float lerpAmount = (horizontalRays == 1) ? 0.5f : (float)i / (horizontalRays - 1);
                Vector2 rayOrigin = (Vector2)transform.position +
                                   new Vector2(0, Mathf.Lerp(-playerHeight / 2, playerHeight / 2, lerpAmount));
                rayOrigin.x += direction * playerWidth / 2;

                Gizmos.DrawRay(rayOrigin, new Vector2(direction * rayLengthDebug, 0));
            }
        }

        // 绘制垂直检测射线（只绘制向上的）
        if (Application.isPlaying && verticalVelocity > 0)
        {
            float rayLengthDebug = Mathf.Abs(verticalVelocity * Time.fixedDeltaTime) + groundCheckDistance;

            for (int i = 0; i < verticalRays; i++)
            {
                float lerpAmount = (verticalRays == 1) ? 0.5f : (float)i / (verticalRays - 1);
                Vector2 rayOrigin = (Vector2)transform.position +
                                   new Vector2(Mathf.Lerp(-playerWidth / 2, playerWidth / 2, lerpAmount), 0);
                rayOrigin.y += playerHeight / 2;

                Gizmos.DrawRay(rayOrigin, Vector2.up * rayLengthDebug);
            }
        }
    }

    public bool bug2Active_ { get => bug2Active; }
}