using UnityEngine;

public class EnemySpawner : CharacterSpawner
{
    [Header("Enemy Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;

    private GameObject spawnedEnemy;

    // 公共属性
    public GameObject SpawnedEnemy { get => spawnedEnemy; }
    public GameObject EnemyPrefab { get => enemyPrefab; set => enemyPrefab = value; }

    protected override void Awake()
    {
        base.Awake();
    }

    protected void Start()
    {
        // 生成敌人
        SpawnEnemy();
    }

    protected override void OnDestroy()
    {
        // 清理生成的敌人
        if (spawnedEnemy != null)
        {
            Destroy(spawnedEnemy);
        }
    }

    // 重写获取生成位置的方法，使用生成器自身位置
    protected override Vector3 GetSpawnPosition()
    {
        return transform.position + spawnOffset;
    }

    // 生成敌人
    public void SpawnEnemy()
    {
        // 清除现有敌人
        ClearEnemy();

        if (enemyPrefab == null)
        {
            Debug.LogWarning("No enemy prefab defined!");
            return;
        }

        Vector3 spawnPos = GetSpawnPosition();
        spawnedEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        spawnedEnemy.name = $"Enemy_{enemyPrefab.name}";

        Debug.Log($"Enemy {enemyPrefab.name} spawned at position: {spawnPos}");
    }

    // 清除敌人
    public void ClearEnemy()
    {
        if (spawnedEnemy != null)
        {
            Destroy(spawnedEnemy);
            spawnedEnemy = null;
            Debug.Log("Enemy cleared");
        }
    }

    // 重新生成敌人
    public void RespawnEnemy()
    {
        ClearEnemy();
        SpawnEnemy();
    }

    // 设置敌人预制体并重新生成
    public void SetEnemyPrefab(GameObject newEnemyPrefab)
    {
        enemyPrefab = newEnemyPrefab;
        RespawnEnemy();
    }

    // 检查是否有敌人生成
    public bool HasEnemy()
    {
        return spawnedEnemy != null;
    }

    // 在编辑器中可视化生成点
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // 绘制敌人生成点
        Gizmos.color = Color.red;
        Vector3 spawnPos = GetSpawnPosition();

        // 绘制一个矩形区域表示敌人生成区域
        Gizmos.DrawWireCube(spawnPos, new Vector3(1f, 1f, 0f));
        Gizmos.DrawIcon(spawnPos, "EnemySpawnPoint", true);

        // 显示敌人名称的文本
#if UNITY_EDITOR
        string enemyName = enemyPrefab != null ? enemyPrefab.name : "None";
        UnityEditor.Handles.Label(spawnPos + Vector3.up * 0.7f, $"Enemy: {enemyName}");
#endif
    }
}