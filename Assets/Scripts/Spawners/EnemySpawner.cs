using UnityEngine;

public class EnemySpawner : CharacterSpawner
{
    [Header("Enemy Spawn Settings")]
    [SerializeField] private string enemyNamePrefix = "Enemy";

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    // 重写获取生成位置的方法，使用生成器自身位置
    protected override Vector3 GetSpawnPosition()
    {
        return transform.position + spawnOffset;
    }

    // 生成敌人
    public void SpawnEnemy()
    {
        // 保存原始名称并设置敌人名称
        string originalName = characterName;
        characterName = $"{enemyNamePrefix}_{characterPrefab?.name ?? "Unknown"}";

        Spawn();

        // 恢复原始名称
        characterName = originalName;

        Debug.Log($"Enemy {characterPrefab?.name} spawned at position: {characterInstance?.transform.position}");
    }

    // 清除敌人
    public void ClearEnemy()
    {
        DestroyCharacter();
        Debug.Log("Enemy cleared");
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
        characterPrefab = newEnemyPrefab;
        RespawnEnemy();
    }

    // 检查是否有敌人生成
    public bool HasEnemy()
    {
        return characterInstance != null;
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
        string enemyName = characterPrefab != null ? characterPrefab.name : "None";
        UnityEditor.Handles.Label(spawnPos + Vector3.up * 0.7f, $"Enemy: {enemyName}");
#endif
    }
}