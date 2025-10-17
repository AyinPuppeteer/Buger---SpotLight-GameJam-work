using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : CharacterSpawner
{
    [Header("Enemy Spawn Settings")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private List<Transform> enemySpawnPoints = new List<Transform>();

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    // 单例实例
    public static EnemySpawner Instance { get; private set; }

    // 公共属性
    public List<GameObject> SpawnedEnemies { get => new List<GameObject>(spawnedEnemies); }

    protected override void Awake()
    {
        base.Awake();
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple EnemySpawner instances detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    protected void Start()
    {
        //生成所有敌人
        SpawnAll();
    }

    protected override void OnDestroy()
    {
        // 清理单例引用
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // 生成所有敌人
    public void SpawnAll()
    { //在敌人列表中的敌人只能在生成点列表中生成，而父类中的角色预制体生成方式跟玩家类似，可以作为boss
        // 清除现有敌人
        ClearAll();

        if (enemyPrefabs.Count == 0 || enemySpawnPoints.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs or spawn points defined!");
            return;
        }

        for (int i = 0; i < enemySpawnPoints.Count; i++)
        {
            Transform spawnPoint = enemySpawnPoints[i];
            if (spawnPoint != null)
            {
                // 循环使用敌人预制体
                GameObject enemyPrefab = enemyPrefabs[i % enemyPrefabs.Count];
                Vector3 spawnPos = spawnPoint.position + spawnOffset;

                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                enemy.name = $"Enemy_{i}";
                spawnedEnemies.Add(enemy);

                Debug.Log($"Enemy spawned at position: {spawnPos}");
            }
        }
    }

    // 在指定位置生成敌人
    public GameObject SpawnAtPosition(GameObject enemyPrefab, Vector3 position)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is null!");
            return null;
        }

        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        enemy.name = "Enemy";
        spawnedEnemies.Add(enemy);

        Debug.Log($"Enemy spawned at position: {position}");
        return enemy;
    }

    // 清除所有敌人
    public void ClearAll()
    {
        // 清除父类中的角色实例
        DestroyCharacter();

        // 清除额外生成的敌人实例
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();

        Debug.Log("All enemies cleared");
    }

    // 在编辑器中可视化生成点
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // 绘制敌人生成点
        Gizmos.color = Color.red;
        foreach (Transform spawnPoint in enemySpawnPoints)
        {
            if (spawnPoint != null)
            {
                Vector3 enemySpawnPos = spawnPoint.position + spawnOffset;
                Gizmos.DrawWireSphere(enemySpawnPos, 0.3f);
                Gizmos.DrawIcon(enemySpawnPos, "EnemySpawnPoint", true);
            }
        }
    }
}