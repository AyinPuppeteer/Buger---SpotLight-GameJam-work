using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [Header("Player Spawn Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private bool spawnPlayerAtStart = true;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Vector3 playerSpawnOffset = Vector3.zero;
    [SerializeField] private bool useWorldCenterAsFallback = true;

    [Header("Enemy Spawn Settings")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private List<Transform> enemySpawnPoints = new List<Transform>();
    [SerializeField] private Vector3 enemySpawnOffset = Vector3.zero;

    [Header("General Settings")]
    [SerializeField] private bool markAsDontDestroyOnLoad = true;

    private GameObject playerInstance;
    private List<GameObject> enemyInstances = new List<GameObject>();

    // 公共属性，用于外部访问
    public GameObject PlayerInstance { get => playerInstance; }
    public List<GameObject> EnemyInstances { get => new List<GameObject>(enemyInstances); }
    public GameObject PlayerPrefab { get => playerPrefab; set => playerPrefab = value; }

    private void Awake()
    {
        // 如果设置为游戏开始时生成玩家，则生成玩家
        if (spawnPlayerAtStart && playerPrefab != null)
        {
            SpawnPlayer();
        }
    }

    /// <summary>
    /// 获取玩家生成位置
    /// </summary>
    private Vector3 GetPlayerSpawnPosition()
    {
        Vector3 basePosition;

        if (playerSpawnPoint != null)
        {
            basePosition = playerSpawnPoint.position;
        }
        else if (useWorldCenterAsFallback)
        {
            basePosition = Vector3.zero;
        }
        else
        {
            basePosition = transform.position;
        }

        return basePosition + playerSpawnOffset;
    }

    /// <summary>
    /// 获取敌人生成位置
    /// </summary>
    private Vector3 GetEnemySpawnPosition(Transform spawnPoint)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("Enemy spawn point is null, using world center");
            return Vector3.zero + enemySpawnOffset;
        }

        return spawnPoint.position + enemySpawnOffset;
    }

    /// <summary>
    /// 生成玩家角色
    /// </summary>
    public void SpawnPlayer()
    {
        // 如果玩家实例已存在，先销毁
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }

        Vector3 spawnPos = GetPlayerSpawnPosition();
        playerInstance = SpawnCharacter(playerPrefab, spawnPos, "Player");

        if (markAsDontDestroyOnLoad)
        {
            DontDestroyOnLoad(playerInstance);
        }

        Debug.Log($"Player spawned at position: {spawnPos}");
    }

    /// <summary>
    /// 在指定位置生成玩家
    /// </summary>
    /// <param name="spawnPoint">生成点Transform</param>
    /// <param name="offset">位置偏移</param>
    public void SpawnPlayerAtPosition(Transform spawnPoint = null, Vector3? offset = null)
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }

        Vector3 spawnPos;
        if (spawnPoint != null)
        {
            spawnPos = spawnPoint.position + (offset ?? Vector3.zero);
        }
        else
        {
            spawnPos = GetPlayerSpawnPosition();
        }

        playerInstance = SpawnCharacter(playerPrefab, spawnPos, "Player");

        if (markAsDontDestroyOnLoad)
        {
            DontDestroyOnLoad(playerInstance);
        }

        Debug.Log($"Player spawned at custom position: {spawnPos}");
    }

    /// <summary>
    /// 重置玩家位置
    /// </summary>
    public void ResetPlayerPosition()
    {
        if (playerInstance != null)
        {
            Vector3 spawnPos = GetPlayerSpawnPosition();
            playerInstance.transform.position = spawnPos;
            Debug.Log($"Player position reset to: {spawnPos}");
        }
    }

    /// <summary>
    /// 设置玩家生成点
    /// </summary>
    /// <param name="spawnPoint">生成点Transform</param>
    /// <param name="offset">位置偏移</param>
    public void SetPlayerSpawnPoint(Transform spawnPoint, Vector3 offset = default)
    {
        playerSpawnPoint = spawnPoint;
        playerSpawnOffset = offset;
    }

    /// <summary>
    /// 在所有敌人生成点生成敌人
    /// </summary>
    public void SpawnEnemies()
    {
        // 清除现有敌人
        ClearEnemies();

        if (enemyPrefabs.Count == 0 || enemySpawnPoints.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs or spawn points defined!");
            return;
        }

        for (int i = 0; i < enemySpawnPoints.Count; i++)
        {
            // 循环使用敌人预制体
            GameObject enemyPrefab = enemyPrefabs[i % enemyPrefabs.Count];
            Transform spawnPoint = enemySpawnPoints[i];

            if (enemyPrefab != null && spawnPoint != null)
            {
                Vector3 spawnPos = GetEnemySpawnPosition(spawnPoint);
                GameObject enemy = SpawnCharacter(enemyPrefab, spawnPos, $"Enemy_{i}");
                enemyInstances.Add(enemy);
                Debug.Log($"Enemy spawned at position: {spawnPos}");
            }
        }
    }

    /// <summary>
    /// 在指定位置生成特定敌人
    /// </summary>
    /// <param name="enemyPrefab">敌人预制体</param>
    /// <param name="spawnPoint">生成点Transform</param>
    /// <param name="offset">位置偏移</param>
    /// <param name="enemyName">敌人名称（可选）</param>
    public GameObject SpawnEnemyAtPosition(GameObject enemyPrefab, Transform spawnPoint = null, Vector3? offset = null, string enemyName = "Enemy")
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is null!");
            return null;
        }

        Vector3 spawnPos;
        if (spawnPoint != null)
        {
            spawnPos = spawnPoint.position + (offset ?? Vector3.zero);
        }
        else
        {
            spawnPos = Vector3.zero + (offset ?? Vector3.zero);
        }

        GameObject enemy = SpawnCharacter(enemyPrefab, spawnPos, enemyName);
        enemyInstances.Add(enemy);
        Debug.Log($"Enemy '{enemyName}' spawned at position: {spawnPos}");

        return enemy;
    }

    /// <summary>
    /// 添加敌人生成点
    /// </summary>
    /// <param name="spawnPoint">生成点Transform</param>
    public void AddEnemySpawnPoint(Transform spawnPoint)
    {
        if (spawnPoint != null && !enemySpawnPoints.Contains(spawnPoint))
        {
            enemySpawnPoints.Add(spawnPoint);
        }
    }

    /// <summary>
    /// 移除敌人生成点
    /// </summary>
    /// <param name="spawnPoint">生成点Transform</param>
    public void RemoveEnemySpawnPoint(Transform spawnPoint)
    {
        enemySpawnPoints.Remove(spawnPoint);
    }

    /// <summary>
    /// 清除所有敌人实例
    /// </summary>
    public void ClearEnemies()
    {
        foreach (GameObject enemy in enemyInstances)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        enemyInstances.Clear();
        Debug.Log("All enemies cleared");
    }

    /// <summary>
    /// 通用角色生成函数
    /// </summary>
    /// <param name="prefab">角色预制体</param>
    /// <param name="position">生成位置</param>
    /// <param name="name">角色名称</param>
    /// <returns>生成的角色实例</returns>
    private GameObject SpawnCharacter(GameObject prefab, Vector3 position, string name)
    {
        if (prefab == null)
        {
            Debug.LogError($"Prefab for {name} is null!");
            return null;
        }

        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        instance.name = name;
        return instance;
    }

    // 在编辑器中可视化生成点
    private void OnDrawGizmos()
    {
        // 绘制玩家生成点
        Vector3 playerSpawnPos = GetPlayerSpawnPosition();

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerSpawnPos, 0.5f);
        Gizmos.DrawIcon(playerSpawnPos, "PlayerSpawnPoint", true);

        // 绘制敌人生成点
        Gizmos.color = Color.red;
        foreach (Transform spawnPoint in enemySpawnPoints)
        {
            if (spawnPoint != null)
            {
                Vector3 enemySpawnPos = GetEnemySpawnPosition(spawnPoint);
                Gizmos.DrawWireSphere(enemySpawnPos, 0.3f);
                Gizmos.DrawIcon(enemySpawnPos, "EnemySpawnPoint", true);
            }
        }

        // 绘制偏移量指示线
        if (playerSpawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(playerSpawnPoint.position, playerSpawnPos);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 绘制玩家生成点范围
        Vector3 playerSpawnPos = GetPlayerSpawnPosition();

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerSpawnPos, 0.7f);

        // 绘制敌人生成点范围
        Gizmos.color = Color.red;
        foreach (Transform spawnPoint in enemySpawnPoints)
        {
            if (spawnPoint != null)
            {
                Vector3 enemySpawnPos = GetEnemySpawnPosition(spawnPoint);
                Gizmos.DrawWireSphere(enemySpawnPos, 0.5f);
            }
        }
    }
}