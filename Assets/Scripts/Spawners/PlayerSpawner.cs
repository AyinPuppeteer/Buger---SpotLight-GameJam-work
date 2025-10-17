using UnityEngine;

public class PlayerSpawner : CharacterSpawner
{
    // 单例实例
    public static PlayerSpawner Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            // 玩家生成器通常需要跨场景持久化
            markAsDontDestroyOnLoad = true;
        }
        else Destroy(gameObject);
    }

    protected override void OnDestroy()
    {
        // 清理单例引用
        if (Instance == this)
        {
            Instance = null;
        }
    }
}