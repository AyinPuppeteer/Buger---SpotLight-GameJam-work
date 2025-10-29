using UnityEngine;
using System.Collections;

public class FlashlightDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 8.0f;
    [SerializeField] private float detectionAngle = 60f;
    [SerializeField] private float detectionFrequency = 0.2f;
    [SerializeField] private LayerMask obstacleLayerMask = 1 << 3; // Block图层
    [SerializeField] private bool drawDebugRays = true;
    [SerializeField] private bool showVisionInGame = true; // 游戏内显示视野

    [Header("Visual Settings")]
    [SerializeField] private Material visionMaterial;
    [SerializeField] private Color normalVisionColor = new Color(1, 1, 0, 0.3f);
    [SerializeField] private Color detectedVisionColor = new Color(1, 0, 0, 0.5f);

    private Guard parentGuard;
    private BaseMovement player;
    private bool playerDetected = false;
    private Mesh visionMesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private bool isFacingRight = true;

    public bool PlayerDetected => playerDetected;
    public BaseMovement DetectedPlayer => player;

    private void Start()
    {
        parentGuard = GetComponentInParent<Guard>();
        if (parentGuard == null)
        {
            Debug.LogError("FlashlightDetector: No Guard component found in parent!");
        }

        // 查找玩家
        FindPlayer();

        // 创建游戏内视野可视化
        if (showVisionInGame)
        {
            CreateVisionMesh();
        }

        // 开始检测协程
        StartCoroutine(DetectionRoutine());
    }

    private void Update()
    {
        // 更新手电筒方向，跟随父物体旋转
        UpdateFlashlightDirection();

        // 更新视野可视化
        if (showVisionInGame && visionMesh != null)
        {
            UpdateVisionMesh();
        }
    }

    private void UpdateFlashlightDirection()
    {
        if (parentGuard != null)
        {
            // 根据保安的朝向调整手电筒方向
            bool facingRight = parentGuard.ToRight_;

            // 如果朝向发生变化，重新生成视野网格
            if (facingRight != isFacingRight)
            {
                isFacingRight = facingRight;
                // 强制重新生成网格
                if (visionMesh != null)
                {
                    UpdateVisionMesh();
                }
            }

            Vector3 localScale = transform.localScale;
            localScale.x = facingRight ? 1 : -1;
            transform.localScale = localScale;
            // 确保手电筒的旋转角度正确
            float angleZ = facingRight ? 0 : 180;
            transform.localRotation = Quaternion.Euler(0, 0, angleZ);
        }
    }

    private IEnumerator DetectionRoutine()
    {
        while (true)
        {
            if (player != null)
            {
                DetectPlayer();
            }
            else
            {
                FindPlayer(); // 如果玩家丢失，重新查找
            }
            yield return new WaitForSeconds(detectionFrequency);
        }
    }

    private void FindPlayer()
    {
        player = FindObjectOfType<BaseMovement>();
    }

    private void DetectPlayer()
    {
        if (player == null) return;

        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // 检查距离
        if (distanceToPlayer > detectionRange)
        {
            playerDetected = false;
            return;
        }

        // 检查角度（手电筒前方扇形区域）
        float angleToPlayer = Vector3.Angle(transform.right, directionToPlayer);
        if (angleToPlayer > detectionAngle / 2)
        {
            playerDetected = false;
            return;
        }

        // 检查玩家是否在草丛中（不可探测）
        if (!player.IsDetectable_)
        {
            playerDetected = false;
            return;
        }

        // 关键修改：检查从手电筒到玩家的路径上是否有障碍物
        // 使用射线检测，从手电筒位置向玩家位置发射射线
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            directionToPlayer.normalized,
            distanceToPlayer,
            obstacleLayerMask
        );

        // 如果没有障碍物遮挡
        if (hit.collider == null)
        {
            playerDetected = true;

            // 绘制调试射线
            if (drawDebugRays)
            {
                Debug.DrawRay(transform.position, directionToPlayer, Color.green, detectionFrequency);
            }
        }
        else
        {
            playerDetected = false;

            // 绘制调试射线
            if (drawDebugRays)
            {
                Debug.DrawRay(transform.position, directionToPlayer, Color.red, detectionFrequency);
            }
        }
    }

    private void CreateVisionMesh()
    {
        // 创建Mesh组件
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        //渲染层级设置为Player
        meshRenderer.sortingLayerName = "Player";

        // 创建材质
        if (visionMaterial == null)
        {
            visionMaterial = new Material(Shader.Find("Sprites/Default"));
            visionMaterial.color = normalVisionColor;
        }
        meshRenderer.material = visionMaterial;

        // 创建Mesh
        visionMesh = new Mesh();
        meshFilter.mesh = visionMesh;
    }

    private void UpdateVisionMesh()
    {
        if (visionMesh == null) return;

        int segments = 20; // 扇形细分段数
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // 中心点（本地坐标）
        vertices[0] = Vector3.zero;

        // 计算扇形顶点，考虑障碍物遮挡
        float angleStep = detectionAngle / segments;

        for (int i = 0; i <= segments; i++)
        {
            // 计算角度（基于当前朝向）
            float angle;
            if (isFacingRight)
            {
                // 面向右：从 -angle/2 到 +angle/2
                angle = -detectionAngle / 2 + i * angleStep;
            }
            else
            {
                // 面向左：从 +angle/2 到 -angle/2
                angle = detectionAngle / 2 - i * angleStep;
            }

            // 使用世界坐标方向进行射线检测
            Vector3 worldDir = Quaternion.Euler(0, 0, angle) * transform.right;

            // 进行射线检测，检查是否有障碍物
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                worldDir,
                detectionRange,
                obstacleLayerMask
            );

            // 计算顶点位置
            Vector3 vertexPos;
            if (hit.collider != null)
            {
                // 将世界坐标的碰撞点转换为本地坐标
                vertexPos = transform.InverseTransformPoint(hit.point);

                // 绘制调试射线
                if (drawDebugRays)
                {
                    Debug.DrawRay(transform.position, worldDir * hit.distance, Color.yellow, detectionFrequency);
                }
            }
            else
            {
                // 计算本地坐标中的最大距离点
                vertexPos = (Quaternion.Euler(0, 0, isFacingRight ? angle : (180 + angle)) * transform.right) * detectionRange;

                // 绘制调试射线
                if (drawDebugRays)
                {
                    Debug.DrawRay(transform.position, worldDir * detectionRange, Color.blue, detectionFrequency);
                }
            }

            vertices[i + 1] = vertexPos;
        }

        // 创建三角形 - 确保三角形顺序正确
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // 更新Mesh
        visionMesh.Clear();
        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateNormals();
        visionMesh.RecalculateBounds();

        // 根据检测状态更新颜色
        meshRenderer.material.color = playerDetected ? detectedVisionColor : normalVisionColor;
    }

    private void OnDrawGizmosSelected()
    {
        // 绘制检测范围
        Gizmos.color = playerDetected ? Color.red : new Color(1, 0.5f, 0, 0.3f);

        // 绘制扇形区域
        Vector3 forward = transform.right;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, detectionAngle / 2) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -detectionAngle / 2) * forward;

        Gizmos.DrawRay(transform.position, leftBoundary * detectionRange);
        Gizmos.DrawRay(transform.position, rightBoundary * detectionRange);

        // 绘制扇形弧线
#if UNITY_EDITOR
        UnityEditor.Handles.color = playerDetected ? Color.red : new Color(1, 0.5f, 0, 0.3f);
        UnityEditor.Handles.DrawWireArc(transform.position, Vector3.forward,
                                      rightBoundary, detectionAngle, detectionRange);
#endif

        // 显示检测状态
        GUIStyle style = new GUIStyle();
        style.normal.textColor = playerDetected ? Color.red : Color.green;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f,
            playerDetected ? "Player Detected" : "Searching", style);
#endif
    }
}