using UnityEngine;
using System.Collections;

public class Guard : CharacterBase
{
    [Header("Guard Settings")]
    [SerializeField] private float patrolSpeed = 2.0f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float patrolRange = 5.0f;
    [SerializeField] private float jumpCooldown = 1.0f; // 跳跃冷却时间
    [SerializeField] private float stoppingDistance = 0.01f; // 停止距离

    [Header("Obstacle Detection")]
    [SerializeField] private int horizontalDetectionRays = 5; // 水平方向障碍检测射线数量
    [SerializeField] private float obstacleDetectionDistance = 0.05f; // 障碍检测距离
    [SerializeField] private float maxJumpHeight = 1.0f; // 最大可跳跃高度
    [SerializeField] private float minJumpHeight = 0.2f; // 最小可跳跃高度

    [Header("Stuck Detection")]
    [SerializeField] private float stuckCheckTime = 2.0f; // 卡住检测时间
    [SerializeField] private float minMovementThreshold = 0.01f; // X轴最小移动阈值

    private Vector3 startPosition;
    private Vector3 currentPatrolCenter; // 当前巡逻中心点
    private bool isChasing = false;
    private bool isActivated = false; // 是否被激活
    private Transform playerTransform;
    private float lastJumpTime = 0f;
    private bool shouldJump = false;
    private float patrolDirection = 1f; // 巡逻方向 (1 = 右, -1 = 左)

    // 卡住检测相关变量
    private float lastXPosition;
    private float stuckTimer = 0f;
    private int consecutiveJumps = 0; // 连续跳跃次数
    private const int MaxConsecutiveJumps = 3; // 最大连续跳跃次数

    // 临时反向相关变量
    private bool isTemporarilyReversed = false;
    private float tempReverseEndTime = 0f;

    protected override void Awake()
    {
        base.Awake();
        startPosition = transform.position;
        currentPatrolCenter = startPosition; // 初始巡逻中心为起始位置
        canJump = true; // 保安也可以跳跃
        lastXPosition = transform.position.x;

        // 查找玩家
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        originSpeed = patrolSpeed;
    }

    protected override void Update()
    {
        // 如果还没有找到玩家，继续尝试查找
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        HandleEnemyBehavior();
        base.Update();
    }

    protected override void FixedUpdate()
    {
        // 检查是否卡住
        CheckIfStuck();

        // 检查临时反向是否结束
        CheckTemporaryReverse();

        base.FixedUpdate();
    }

    protected override void GetInput()
    {
        // AI控制移动
        if (isChasing && playerTransform != null)
        {
            // 追逐玩家
            float targetDirection = Mathf.Sign(playerTransform.position.x - transform.position.x);

            // 如果处于临时反向状态，使用反向方向
            if (isTemporarilyReversed)
            {
                horizontal = -targetDirection;
            }
            else
            {
                horizontal = targetDirection;
            }

            vertical = 0f; // 简化处理，敌人不主动爬梯子
            originSpeed = chaseSpeed;
        }
        else
        {
            // 巡逻行为
            PatrolBehavior();
            originSpeed = patrolSpeed;
        }

        // 检查是否需要跳跃
        CheckJump();

        inputVector = new Vector2(horizontal, vertical);

        // 如果需要跳跃且在地面上且冷却时间已过
        if (shouldJump && isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            jumpRequested = true;
            shouldJump = false;
            lastJumpTime = Time.time;
            consecutiveJumps++;
        }
    }

    private void HandleEnemyBehavior()
    {
        if (playerTransform == null) return;

        // 如果敌人被激活，开始追逐
        if (isActivated)
        {
            isChasing = true;
        }
        else if (isChasing)
        {
            // 如果取消激活且正在追逐，将当前位置设为新的巡逻中心并开始巡逻
            isChasing = false;
            currentPatrolCenter = transform.position; // 将当前位置设为新的巡逻中心
            patrolDirection = 1f; // 重置巡逻方向
            lastXPosition = currentPatrolCenter.x;
        }
    }

    private void PatrolBehavior()
    {
        // 计算当前位置相对于巡逻中心的偏移
        float currentOffset = transform.position.x - currentPatrolCenter.x;

        // 如果到达巡逻边界，改变方向
        if (Mathf.Abs(currentOffset) >= patrolRange)
        {
            patrolDirection *= -1;
        }

        // 如果处于临时反向状态，使用反向方向
        if (isTemporarilyReversed)
        {
            horizontal = -patrolDirection;
        }
        else
        {
            horizontal = patrolDirection;
        }
    }

    /// <summary>
    /// 检查是否需要跳跃（障碍物和边缘）
    /// </summary>
    private void CheckJump()
    {
        Vector2 rayDirection = toRight ? Vector2.right : Vector2.left;

        // 追逐玩家时，如果玩家在较高位置，尝试跳跃
        if (isChasing && playerTransform != null && isGrounded)
        {
            float heightDifference = playerTransform.position.y - transform.position.y;

            // 如果玩家在较高位置（需要跳跃才能到达）
            if (heightDifference > minJumpHeight && heightDifference < maxJumpHeight)
            {
                shouldJump = true;
                return; // 优先处理追逐跳跃
            }
        }

        // 检查前方是否有障碍物需要跳跃
        for (int i = 0; i < horizontalDetectionRays; i++)
        {
            float lerpAmount = (horizontalDetectionRays == 1) ? 0.5f : (float)i / (horizontalDetectionRays - 1);
            Vector2 rayOrigin = (Vector2)transform.position +
                               new Vector2(0, Mathf.Lerp(-playerHeight / 2, playerHeight / 2, lerpAmount));

            // 添加微小的水平偏移
            rayOrigin.x += rayDirection.x * playerWidth / 2;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, obstacleDetectionDistance, obstacleLayerMask);

            if (hit.collider != null)
            {
                // 修复障碍物高度计算
                float obstacleHeight = GetObstacleHeight(hit);
                float myBottom = transform.position.y - playerHeight / 2; // 敌人底部位置

                // 如果障碍物底部在敌人上方且在可跳跃范围内
                if (obstacleHeight > myBottom && obstacleHeight < myBottom + maxJumpHeight)
                {
                    // 只要有一条射线探测到障碍物就跳起
                    shouldJump = true;
                    return; // 找到一条就跳出循环
                }
            }
        }

        // 检查前方是否有边缘需要跳跃
        CheckForEdge(rayDirection);
    }

    /// <summary>
    /// 修复障碍物高度计算方法
    /// </summary>
    private float GetObstacleHeight(RaycastHit2D hit)
    {
        // 直接使用碰撞点的Y坐标作为障碍物底部高度
        float obstacleBottom = hit.point.y;

        // 从碰撞点向上发射射线，检测障碍物顶部
        Vector2 topRayStart = new Vector2(hit.point.x, hit.point.y);
        float maxHeight = hit.point.y + maxJumpHeight;

        RaycastHit2D topHit = Physics2D.Raycast(topRayStart, Vector2.up, maxJumpHeight, obstacleLayerMask);

        if (topHit.collider != null)
        {
            // 返回障碍物顶部高度
            return topHit.point.y;
        }

        // 如果没有检测到顶部，返回碰撞点高度加上保守估计
        return hit.point.y + playerHeight / 2;
    }

    /// <summary>
    /// 检查前方是否有边缘需要跳跃
    /// </summary>
    private void CheckForEdge(Vector2 rayDirection)
    {
        // 检查前方是否有悬崖需要跳跃
        Vector2 edgeCheckPos = (Vector2)transform.position + rayDirection * 0.5f;
        RaycastHit2D groundCheck = Physics2D.Raycast(edgeCheckPos, Vector2.down, playerHeight * 3, groundLayerMask);

        if (groundCheck.collider == null && isGrounded)
        {
            // 前方是边缘，直接跳起并前进
            shouldJump = true;
        }
    }

    /// <summary>
    /// 检查敌人是否卡住 - 基于X轴位置变化
    /// </summary>
    private void CheckIfStuck()
    {
        // 计算当前位置与上一帧位置的X轴变化
        float xMovement = Mathf.Abs(transform.position.x - lastXPosition);

        // 如果X轴移动距离小于阈值且敌人正在尝试移动，增加卡住计时器
        if (xMovement < minMovementThreshold && Mathf.Abs(horizontal) > 0.1f)
        {
            stuckTimer += Time.fixedDeltaTime;
        }
        else
        {
            // 如果移动正常，重置卡住计时器和连续跳跃计数
            stuckTimer = 0f;
            consecutiveJumps = 0;
        }

        // 记录当前X位置用于下一帧比较
        lastXPosition = transform.position.x;

        // 如果卡住时间超过阈值或连续跳跃次数过多，改变方向
        if (stuckTimer >= stuckCheckTime || consecutiveJumps >= MaxConsecutiveJumps)
        {
            ReverseDirection();
        }
    }

    /// <summary>
    /// 检查临时反向是否结束
    /// </summary>
    private void CheckTemporaryReverse()
    {
        if (isTemporarilyReversed && Time.time >= tempReverseEndTime)
        {
            isTemporarilyReversed = false;
        }
    }

    /// <summary>
    /// 改变巡逻方向
    /// </summary>
    private void ReverseDirection()
    {
        // 根据当前状态执行不同的反向逻辑
        if (isChasing)
        {
            // 追逐状态下，暂时反向一小段时间
            StartTemporaryReverse(.5f);
        }
        else
        {
            // 巡逻状态下，永久反向
            patrolDirection *= -1;
            stuckTimer = 0f;
            consecutiveJumps = 0;
        }
    }

    /// <summary>
    /// 开始临时反向
    /// </summary>
    private void StartTemporaryReverse(float duration)
    {
        isTemporarilyReversed = true;
        tempReverseEndTime = Time.time + duration;
        stuckTimer = 0f;
        consecutiveJumps = 0;
    }

    /// <summary>
    /// 设置敌人激活状态
    /// </summary>
    /// <param name="active">是否激活敌人</param>
    public void SetActive(bool active)
    {
        isActivated = active;

        // 如果取消激活且正在追逐，将当前位置设为新的巡逻中心并开始巡逻
        if (!active && isChasing)
        {
            isChasing = false;
            currentPatrolCenter = transform.position; // 将当前位置设为新的巡逻中心
            patrolDirection = 1f; // 重置巡逻方向
            lastXPosition = currentPatrolCenter.x;
        }
    }

    // 在编辑器中可视化巡逻范围和检测射线
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.yellow;
        Vector3 patrolStart = Application.isPlaying ? currentPatrolCenter : transform.position;
        Gizmos.DrawWireSphere(patrolStart, patrolRange);

        // 绘制障碍检测射线
        Vector2 rayDirection = toRight ? Vector2.right : Vector2.left;
        Gizmos.color = Color.blue;

        // 绘制水平检测射线
        for (int i = 0; i < horizontalDetectionRays; i++)
        {
            float lerpAmount = (horizontalDetectionRays == 1) ? 0.5f : (float)i / (horizontalDetectionRays - 1);
            Vector2 rayOrigin = (Vector2)transform.position +
                               new Vector2(0, Mathf.Lerp(-playerHeight / 2, playerHeight / 2, lerpAmount));

            rayOrigin.x += rayDirection.x * playerWidth / 2;

            Gizmos.DrawRay(rayOrigin, rayDirection * obstacleDetectionDistance);
        }

        // 绘制巡逻范围线
        Gizmos.color = Color.yellow;
        Vector3 basePos = Application.isPlaying ? currentPatrolCenter : transform.position;
        Gizmos.DrawLine(
            new Vector3(basePos.x - patrolRange, basePos.y, basePos.z),
            new Vector3(basePos.x + patrolRange, basePos.y, basePos.z)
        );

        // 绘制边缘检测
        Gizmos.color = Color.red;
        Vector2 edgeCheckPos = (Vector2)transform.position + rayDirection * 0.5f;
        Gizmos.DrawRay(edgeCheckPos, Vector2.down * playerHeight * 3);

        // 绘制卡住检测区域
        if (Application.isPlaying)
        {
            Gizmos.color = stuckTimer > 0 ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.2f);

            // 显示卡住计时器
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f,
                $"Stuck: {stuckTimer:F1}/{stuckCheckTime}", style);
#endif

            // 显示临时反向状态
            if (isTemporarilyReversed)
            {
                GUIStyle reverseStyle = new GUIStyle();
                reverseStyle.normal.textColor = Color.magenta;
#if UNITY_EDITOR
                UnityEditor.Handles.Label(transform.position + Vector3.up * 2.5f,
                    "TEMP REVERSE", reverseStyle);
#endif
            }

            // 显示当前巡逻中心
            GUIStyle patrolStyle = new GUIStyle();
            patrolStyle.normal.textColor = Color.cyan;
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 3f,
                $"Patrol Center: {currentPatrolCenter}", patrolStyle);
#endif
        }

        // 绘制当前状态
        GUIStyle style2 = new GUIStyle();
        style2.normal.textColor = Color.white;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f,
            isChasing ? "Chasing" : "Patrolling", style2);
#endif
    }

    // 重置敌人状态
    public void ResetGuard()
    {
        isChasing = false;
        isActivated = false;
        transform.position = startPosition;
        currentPatrolCenter = startPosition; // 重置巡逻中心到起始位置
        stuckTimer = 0f;
        consecutiveJumps = 0;
        patrolDirection = 1f;
        lastXPosition = startPosition.x;
        isTemporarilyReversed = false;
    }
}