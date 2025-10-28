using UnityEngine;
using System.Collections;

public class Guard : CharacterBase
{
    [Header("Guard Settings")]
    [SerializeField] private float patrolSpeed = 2.0f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float patrolRange = 5.0f;
    [SerializeField] private float jumpCooldown = 1.0f;
    [SerializeField] private float stoppingDistance = 0.5f; // 增加停止距离

    [Header("Proximity Detection")]
    [SerializeField] private float proximityRange = 3.0f; // 近距离检测范围
    [SerializeField] private float proximityDetectionFrequency = 0.3f; // 近距离检测频率
    [SerializeField] private bool requireGroundedForProximity = true; // 近距离检测是否需要玩家在地面
    [SerializeField] private bool requireMovingForProximity = true; // 近距离检测是否需要玩家在移动

    [Header("Obstacle Detection")]
    [SerializeField] private int horizontalDetectionRays = 5;
    [SerializeField] private float obstacleDetectionDistance = 0.05f;
    [SerializeField] private float maxJumpHeight = 1.0f;
    [SerializeField] private float minJumpHeight = 0.2f;

    [Header("Stuck Detection")]
    [SerializeField] private float stuckCheckTime = 2.0f;
    [SerializeField] private float minMovementThreshold = 0.01f;

    // 手电筒引用
    private FlashlightDetector flashlight;

    private Vector3 startPosition;
    private Vector3 currentPatrolCenter;
    private bool isChasing = false;
    private bool isActivated = false;
    private float lastJumpTime = 0f;
    private bool shouldJump = false;
    private float patrolDirection = 1f;

    // 卡住检测相关变量
    private float lastXPosition;
    private float stuckTimer = 0f;
    private int consecutiveJumps = 0;
    private const int MaxConsecutiveJumps = 3;

    // 临时反向相关变量
    private bool isTemporarilyReversed = false;
    private float tempReverseEndTime = 0f;

    // 玩家检测相关
    private BaseMovement playerMovement;
    private bool playerInSight = false;
    private bool playerInProximity = false;

    protected override void Awake()
    {
        base.Awake();
        startPosition = transform.position;
        currentPatrolCenter = startPosition;
        canJump = true;
        lastXPosition = currentPatrolCenter.x;

        // 获取手电筒组件
        flashlight = GetComponentInChildren<FlashlightDetector>();
        if (flashlight == null)
        {
            Debug.LogWarning("Guard: No FlashlightDetector found in children!");
        }

        // 查找玩家
        FindPlayer();

        originSpeed = patrolSpeed;

        // 开始玩家检测协程
        StartCoroutine(PlayerDetectionRoutine());
    }

    protected override void Update()
    {
        HandleEnemyBehavior();
        CheckForEdge();
        base.Update();
    }

    protected override void FixedUpdate()
    {
        CheckIfStuck();
        CheckTemporaryReverse();
        UpdateChaseState();
        base.FixedUpdate();
    }

    protected override void HandleVisualLayer()
    {
        base.HandleVisualLayer(); 
        if(isChasing) { animator.SetBool(param[0], true);}
        else animator.SetBool(param[0], false);
    }

    /// <summary>
    /// 玩家检测协程
    /// </summary>
    private IEnumerator PlayerDetectionRoutine()
    {
        while (true)
        {
            if (playerMovement != null)
            {
                DetectPlayer();
            }
            else
            {
                FindPlayer(); // 如果玩家丢失，重新查找
            }
            yield return new WaitForSeconds(proximityDetectionFrequency);
        }
    }

    /// <summary>
    /// 检测玩家
    /// </summary>
    private void DetectPlayer()
    {
        // 手电筒检测
        bool flashlightDetected = flashlight != null && flashlight.PlayerDetected;

        // 近距离检测
        bool proximityDetected = CheckProximityDetection();

        // 综合判断
        playerInSight = flashlightDetected;
        playerInProximity = proximityDetected;

        // 如果检测到玩家，激活敌人
        if ((playerInSight || playerInProximity) && !isActivated)
        {
            SetActive(true);
        }
    }

    /// <summary>
    /// 近距离检测逻辑
    /// </summary>
    private bool CheckProximityDetection()
    {
        if (playerMovement == null) return false;

        Vector3 directionToPlayer = playerMovement.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // 检查距离
        if (distanceToPlayer > proximityRange)
        {
            return false;
        }

        // 检查玩家是否在潜行状态
        // 如果玩家在潜行状态，近距离检测不生效
        if (playerMovement.IsSneaking_)
        {
            return false;
        }

        // 检查玩家是否在地面上（如果要求）
        if (requireGroundedForProximity && !playerMovement.IsGrounded_)
        {
            return false;
        }

        // 检查玩家是否在移动（如果要求）
        if (requireMovingForProximity && !IsPlayerMoving())
        {
            return false;
        }

        // 检查视线是否被遮挡
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            directionToPlayer.normalized,
            distanceToPlayer,
            obstacleLayerMask
        );

        // 如果没有障碍物遮挡，或者障碍物后面就是玩家
        return hit.collider == null ||
               (hit.collider != null && hit.collider.gameObject == playerMovement.gameObject);
    }

    /// <summary>
    /// 检查玩家是否在移动
    /// </summary>
    private bool IsPlayerMoving()
    {
        if (playerMovement == null) return false;

        // 使用玩家移动状态的公开属性
        // 注意：我们需要在CharacterBase中公开isWalking字段
        // 如果BaseMovement中没有公开IsWalking_属性，我们需要添加它

        // 临时解决方案：检查玩家的输入向量是否大于死区
        Vector2 playerInput = GameInput.Instance.GetMovementInput();
        return playerInput.magnitude > deadZone;
    }

    /// <summary>
    /// 查找玩家
    /// </summary>
    private void FindPlayer()
    {
        playerMovement = FindObjectOfType<BaseMovement>();
    }

    protected override void GetInput()
    {
        // AI控制移动
        if ((isChasing || playerInSight || playerInProximity) && playerMovement != null)
        {
            // 记录玩家最后位置
            Vector3 playerPosition = playerMovement.transform.position;

            // 检查水平距离是否小于停止距离
            float horizontalDistance = Mathf.Abs(playerPosition.x - transform.position.x);

            // 如果水平距离小于停止距离，停止移动
            if (horizontalDistance <= stoppingDistance)
            {
                horizontal = 0f;
            }
            else
            {
                // 追逐玩家
                float targetDirection = Mathf.Sign(playerPosition.x - transform.position.x);

                // 如果处于临时反向状态，使用反向方向
                if (isTemporarilyReversed)
                {
                    horizontal = -targetDirection;
                }
                else
                {
                    horizontal = targetDirection;
                }
            }

            vertical = 0f;
            originSpeed = chaseSpeed;
        }
        else
        {
            // 巡逻行为
            PatrolBehavior();
            originSpeed = patrolSpeed;
        }

        //CheckJump();
        inputVector = new Vector2(horizontal, vertical);

        //if (shouldJump && isGrounded && Time.time - lastJumpTime > jumpCooldown)
        //{
        //    jumpRequested = true;
        //    shouldJump = false;
        //    lastJumpTime = Time.time;
        //    consecutiveJumps++;
        //}
    }

    private void UpdateChaseState()
    {
        // 如果玩家在视野内或近距离内，开始追逐
        if (playerInSight || playerInProximity)
        {
            isChasing = true;
        }
        // 只有在玩家躲进草丛（不可探测）时才停止追逐
        else if (isChasing && playerMovement != null && !playerMovement.IsDetectable_)
        {
            isChasing = false;
            SetActive(false);
            currentPatrolCenter = transform.position; // 将当前位置设为新的巡逻中心
            patrolDirection = 1f;
            lastXPosition = currentPatrolCenter.x;
        }
        // 如果敌人被激活，保持追逐状态
        else if (isActivated)
        {
            isChasing = true;
        }
    }

    private void HandleEnemyBehavior()
    {
        // 如果敌人被激活，开始追逐
        if (isActivated)
        {
            isChasing = true;
        }
    }

    private void PatrolBehavior()
    {
        float currentOffset = transform.position.x - currentPatrolCenter.x;

        if (Mathf.Abs(currentOffset) >= patrolRange)
        {
            patrolDirection *= -1;
        }

        if (isTemporarilyReversed)
        {
            horizontal = -patrolDirection;
        }
        else
        {
            horizontal = patrolDirection;
        }
    }

    private void CheckJump()
    {
        Vector2 rayDirection = toRight ? Vector2.right : Vector2.left;

        // 追逐玩家时，如果玩家在较高位置，尝试跳跃
        if (isChasing && playerMovement != null && isGrounded)
        {
            float heightDifference = playerMovement.transform.position.y - transform.position.y;

            if (heightDifference > minJumpHeight && heightDifference < maxJumpHeight)
            {
                shouldJump = true;
                return;
            }
        }

        // 检查前方是否有障碍物需要跳跃
        for (int i = 0; i < horizontalDetectionRays; i++)
        {
            float lerpAmount = (horizontalDetectionRays == 1) ? 0.5f : (float)i / (horizontalDetectionRays - 1);
            Vector2 rayOrigin = (Vector2)transform.position +
                               new Vector2(0, Mathf.Lerp(-playerHeight / 2, playerHeight / 2, lerpAmount));

            rayOrigin.x += rayDirection.x * playerWidth / 2;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, obstacleDetectionDistance, obstacleLayerMask);

            if (hit.collider != null)
            {
                float obstacleHeight = GetObstacleHeight(hit);
                float myBottom = transform.position.y - playerHeight / 2;

                if (obstacleHeight > myBottom && obstacleHeight < myBottom + maxJumpHeight)
                {
                    shouldJump = true;
                    return;
                }
            }
        }

        CheckForEdge();
    }

    private float GetObstacleHeight(RaycastHit2D hit)
    {
        float obstacleBottom = hit.point.y;

        Vector2 topRayStart = new Vector2(hit.point.x, hit.point.y);
        float maxHeight = hit.point.y + maxJumpHeight;

        RaycastHit2D topHit = Physics2D.Raycast(topRayStart, Vector2.up, maxJumpHeight, obstacleLayerMask);

        if (topHit.collider != null)
        {
            return topHit.point.y;
        }

        return hit.point.y + playerHeight / 2;
    }

    private void CheckForEdge()
    {
        Vector2 rayDirection = toRight ? Vector2.right : Vector2.left;
        Vector2 edgeCheckPos = (Vector2)transform.position + rayDirection * 0.5f * playerWidth;
        RaycastHit2D groundCheck = Physics2D.Raycast(edgeCheckPos, Vector2.down, playerHeight * .6f, groundLayerMask);

        //if (groundCheck.collider == null && isGrounded)
        //{
        //    shouldJump = true;
        //}
        if (!groundCheck.collider)
        {
            // 如果在巡逻状态下，遇到悬崖则反向
            patrolDirection *= -1;
            horizontal = patrolDirection;
            stuckTimer = 0f;
            consecutiveJumps = 0;
        }
    }

    private void CheckIfStuck()
    {
        float xMovement = Mathf.Abs(transform.position.x - lastXPosition);

        if (xMovement < minMovementThreshold && Mathf.Abs(horizontal) > 0.1f)
        {
            stuckTimer += Time.fixedDeltaTime;
        }
        else
        {
            stuckTimer = 0f;
            consecutiveJumps = 0;
        }

        lastXPosition = transform.position.x;

        if (stuckTimer >= stuckCheckTime || consecutiveJumps >= MaxConsecutiveJumps)
        {
            ReverseDirection();
        }
    }

    private void CheckTemporaryReverse()
    {
        if (isTemporarilyReversed && Time.time >= tempReverseEndTime)
        {
            isTemporarilyReversed = false;
        }
    }

    private void ReverseDirection()
    {
        if (isChasing)
        {
            StartTemporaryReverse(.5f);
        }
        else
        {
            patrolDirection *= -1;
            stuckTimer = 0f;
            consecutiveJumps = 0;
        }
    }

    private void StartTemporaryReverse(float duration)
    {
        isTemporarilyReversed = true;
        tempReverseEndTime = Time.time + duration;
        stuckTimer = 0f;
        consecutiveJumps = 0;
    }

    public void SetActive(bool active)
    {
        isActivated = active;

        if (!active && isChasing)
        {
            isChasing = false;
            currentPatrolCenter = transform.position;
            patrolDirection = 1f;
            lastXPosition = currentPatrolCenter.x;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.yellow;
        Vector3 patrolStart = Application.isPlaying ? currentPatrolCenter : transform.position;
        Gizmos.DrawWireSphere(patrolStart, patrolRange);

        Vector2 rayDirection = toRight ? Vector2.right : Vector2.left;
        Gizmos.color = Color.blue;

        for (int i = 0; i < horizontalDetectionRays; i++)
        {
            float lerpAmount = (horizontalDetectionRays == 1) ? 0.5f : (float)i / (horizontalDetectionRays - 1);
            Vector2 rayOrigin = (Vector2)transform.position +
                               new Vector2(0, Mathf.Lerp(-playerHeight / 2, playerHeight / 2, lerpAmount));

            rayOrigin.x += rayDirection.x * playerWidth / 2;

            Gizmos.DrawRay(rayOrigin, rayDirection * obstacleDetectionDistance);
        }

        Gizmos.color = Color.yellow;
        Vector3 basePos = Application.isPlaying ? currentPatrolCenter : transform.position;
        Gizmos.DrawLine(
            new Vector3(basePos.x - patrolRange, basePos.y, basePos.z),
            new Vector3(basePos.x + patrolRange, basePos.y, basePos.z)
        );

        Gizmos.color = Color.red;
        Vector2 edgeCheckPos = (Vector2)transform.position + rayDirection * 0.5f;
        Gizmos.DrawRay(edgeCheckPos, Vector2.down * playerHeight * 3);

        // 绘制近距离检测范围
        Gizmos.color = playerInProximity ? Color.magenta : new Color(1, 0, 1, 0.3f); // 紫色
        Gizmos.DrawWireSphere(transform.position, proximityRange);

        // 绘制停止距离
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        // 绘制追逐状态和计时器
        if (Application.isPlaying)
        {
            Gizmos.color = stuckTimer > 0 ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.2f);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f,
                $"Stuck: {stuckTimer:F1}/{stuckCheckTime}", style);

            // 显示追逐状态
            GUIStyle chaseStyle = new GUIStyle();
            chaseStyle.normal.textColor = isChasing ? Color.red : Color.green;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f,
                isChasing ? "Chasing" : "Patrolling", chaseStyle);
#endif

            if (isTemporarilyReversed)
            {
                GUIStyle reverseStyle = new GUIStyle();
                reverseStyle.normal.textColor = Color.magenta;
#if UNITY_EDITOR
                UnityEditor.Handles.Label(transform.position + Vector3.up * 2.5f,
                    "TEMP REVERSE", reverseStyle);
#endif
            }

            GUIStyle patrolStyle = new GUIStyle();
            patrolStyle.normal.textColor = Color.cyan;
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 3f,
                $"Patrol Center: {currentPatrolCenter}", patrolStyle);
#endif

            // 显示检测状态
            GUIStyle detectionStyle = new GUIStyle();
            detectionStyle.normal.textColor = Color.white;
#if UNITY_EDITOR
            string detectionStatus = "";
            if (playerInSight) detectionStatus += "Flashlight ";
            if (playerInProximity) detectionStatus += "Proximity ";
            if (string.IsNullOrEmpty(detectionStatus)) detectionStatus = "None";

            UnityEditor.Handles.Label(transform.position + Vector3.down * 1f,
                $"Detection: {detectionStatus}", detectionStyle);

            // 显示玩家状态
            if (playerMovement != null)
            {
                GUIStyle sneakStyle = new GUIStyle();
                sneakStyle.normal.textColor = playerMovement.IsSneaking_ ? Color.yellow : Color.white;
                UnityEditor.Handles.Label(transform.position + Vector3.down * 1.5f,
                    $"Sneaking: {playerMovement.IsSneaking_}", sneakStyle);

                GUIStyle groundStyle = new GUIStyle();
                groundStyle.normal.textColor = playerMovement.IsGrounded_ ? Color.green : Color.red;
                UnityEditor.Handles.Label(transform.position + Vector3.down * 2f,
                    $"Grounded: {playerMovement.IsGrounded_}", groundStyle);

                GUIStyle detectStyle = new GUIStyle();
                detectStyle.normal.textColor = playerMovement.IsDetectable_ ? Color.white : Color.gray;
                UnityEditor.Handles.Label(transform.position + Vector3.down * 2.5f,
                    $"Detectable: {playerMovement.IsDetectable_}", detectStyle);

                // 显示玩家移动状态
                bool isMoving = IsPlayerMoving();
                GUIStyle moveStyle = new GUIStyle();
                moveStyle.normal.textColor = isMoving ? Color.cyan : Color.white;
                UnityEditor.Handles.Label(transform.position + Vector3.down * 3f,
                    $"Moving: {isMoving}", moveStyle);

                // 显示与玩家的水平距离
                float horizontalDistance = Mathf.Abs(playerMovement.transform.position.x - transform.position.x);
                GUIStyle distanceStyle = new GUIStyle();
                distanceStyle.normal.textColor = horizontalDistance <= stoppingDistance ? Color.green : Color.white;
                UnityEditor.Handles.Label(transform.position + Vector3.down * 3.5f,
                    $"Horizontal Distance: {horizontalDistance:F2}", distanceStyle);
            }
#endif
        }
    }

    public void ResetGuard()
    {
        isChasing = false;
        isActivated = false;
        playerInSight = false;
        playerInProximity = false;
        transform.position = startPosition;
        currentPatrolCenter = startPosition;
        stuckTimer = 0f;
        consecutiveJumps = 0;
        patrolDirection = 1f;
        lastXPosition = startPosition.x;
        isTemporarilyReversed = false;
    }
}