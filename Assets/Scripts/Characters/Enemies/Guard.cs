using UnityEngine;

public class Guard : CharacterBase
{
    [Header("Guard Settings")]
    [SerializeField] private float patrolSpeed = 3.0f;
    [SerializeField] private float chaseSpeed = 6.0f;
    [SerializeField] private float patrolRange = 5.0f;
    [SerializeField] private float sightRange = 8.0f;
    [SerializeField] private float jumpCooldown = 2.0f; // 跳跃冷却时间

    private Vector3 startPosition;
    private bool isChasing = false;
    private Transform playerTransform;
    private float currentPatrolPoint;
    private float lastJumpTime = 0f;
    private bool shouldJump = false;

    protected override void Awake()
    {
        base.Awake();
        startPosition = transform.position;
        canJump = true; // 保安也可以跳跃

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

    protected override void GetInput()
    {
        // AI控制移动
        if (isChasing && playerTransform != null)
        {
            // 追逐玩家
            horizontal = Mathf.Sign(playerTransform.position.x - transform.position.x);
            vertical = 0f; // 简化处理，敌人不主动爬梯子
            originSpeed = chaseSpeed;

            // 追逐时检查是否需要跳跃
            CheckJumpInChase();
        }
        else
        {
            // 巡逻行为
            PatrolBehavior();
            originSpeed = patrolSpeed;

            // 巡逻时检查是否需要跳跃
            CheckJumpInPatrol();
        }

        inputVector = new Vector2(horizontal, vertical);

        // 如果需要跳跃且在地面上且冷却时间已过
        if (shouldJump && isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            jumpRequested = true;
            shouldJump = false;
            lastJumpTime = Time.time;
        }
    }

    private void HandleEnemyBehavior()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // 检测玩家是否在视线范围内
        if (distanceToPlayer <= sightRange && CanSeePlayer())
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }
    }

    private bool CanSeePlayer()
    {
        // 视线检测，考虑障碍物
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // 使用图层掩码检测，只检测Block图层作为障碍物
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleLayerMask);

        return hit.collider == null; // 没有障碍物阻挡
    }

    private void PatrolBehavior()
    {
        // 简单的巡逻逻辑
        currentPatrolPoint = Mathf.PingPong(Time.time, 2f) - 1f; // -1 到 1 之间振荡
        horizontal = Mathf.Sign(currentPatrolPoint);

        // 到达巡逻边界时转向
        if (Mathf.Abs(transform.position.x - startPosition.x) >= patrolRange)
        {
            horizontal *= -1;
        }
    }

    private void CheckJumpInPatrol()
    {
        // 检查前方是否有障碍物需要跳跃
        Vector2 rayDirection = toRight ? Vector2.right : Vector2.left;
        float rayDistance = 1.0f;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, rayDistance, obstacleLayerMask);

        // 如果有障碍物且高度可以跳跃
        if (hit.collider != null && isGrounded)
        {
            // 检查障碍物高度
            float obstacleHeight = hit.collider.bounds.max.y;
            float myHeight = transform.position.y + playerHeight / 2;

            // 如果障碍物高度在可跳跃范围内
            if (obstacleHeight > myHeight && obstacleHeight < myHeight + 2.0f)
            {
                shouldJump = true;
            }
        }

        // 检查前方是否有悬崖需要跳跃
        Vector2 edgeCheckPos = (Vector2)transform.position + rayDirection * 0.5f;
        RaycastHit2D groundCheck = Physics2D.Raycast(edgeCheckPos, Vector2.down, 1.0f, groundLayerMask);

        if (groundCheck.collider == null && isGrounded)
        {
            // 前方是悬崖，停止移动但不跳跃（或者可以跳跃，根据设计需求）
            horizontal = 0;
        }
    }

    private void CheckJumpInChase()
    {
        // 追逐玩家时，如果玩家在较高位置，尝试跳跃
        if (playerTransform != null && isGrounded)
        {
            float heightDifference = playerTransform.position.y - transform.position.y;

            // 如果玩家在较高位置（需要跳跃才能到达）
            if (heightDifference > 0.5f && heightDifference < 3.0f)
            {
                shouldJump = true;
            }
        }

        // 同样检查障碍物
        CheckJumpInPatrol();
    }

    // 在编辑器中可视化巡逻范围和视线范围
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // 绘制视线方向
        if (isChasing && playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }

        // 绘制跳跃检测射线
        Vector2 rayDirection = toRight ? Vector2.right : Vector2.left;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, rayDirection * 1.0f);
    }
}