using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] protected GameObject characterPrefab;
    [SerializeField] protected Transform spawnPoint;
    [SerializeField] protected Vector3 spawnOffset = Vector3.zero;
    [SerializeField] protected bool useWorldCenterAsFallback = true;
    [SerializeField] protected bool markAsDontDestroyOnLoad = false;

    [Header("General Settings")]
    [SerializeField] protected string characterName = "Character";

    protected GameObject characterInstance;

    // 公共属性，用于外部访问
    public GameObject CharacterInstance { get => characterInstance; }
    public GameObject CharacterPrefab { get => characterPrefab; set => characterPrefab = value; }

    protected virtual void Awake()
    { 

    }

    protected virtual void OnDestroy()
    {
        // 基类的OnDestroy方法，子类可以重写
    }

    // 获取生成位置
    protected virtual Vector3 GetSpawnPosition()
    {
        Vector3 basePosition;

        if (spawnPoint != null)
        {
            basePosition = spawnPoint.position;
        }
        else if (useWorldCenterAsFallback)
        {
            basePosition = Vector3.zero;
        }
        else
        {
            basePosition = transform.position;
        }

        return basePosition + spawnOffset;
    }

    // 生成角色
    public virtual void Spawn()
    {
        // 如果角色实例已存在，先销毁
        if (characterInstance != null)
        {
            Destroy(characterInstance);
        }

        Vector3 spawnPos = GetSpawnPosition();
        characterInstance = Instantiate(characterPrefab, spawnPos, Quaternion.identity);
        characterInstance.name = characterName;

        if (markAsDontDestroyOnLoad)
        {
            DontDestroyOnLoad(characterInstance);
        }
    }

    // 在指定位置生成角色
    public virtual void SpawnAtPosition(Vector3 position)
    {
        if (characterInstance != null)
        {
            Destroy(characterInstance);
        }

        characterInstance = Instantiate(characterPrefab, position, Quaternion.identity);
        characterInstance.name = characterName;

        if (markAsDontDestroyOnLoad)
        {
            DontDestroyOnLoad(characterInstance);
        }
    }

    // 重置角色位置
    public virtual void ResetPosition()
    {
        if (characterInstance != null)
        {
            Vector3 spawnPos = GetSpawnPosition();
            characterInstance.transform.position = spawnPos;
            Debug.Log($"{characterName} position reset to: {spawnPos}");
        }
    }

    // 设置生成点
    public virtual void SetSpawnPoint(Transform newSpawnPoint, Vector3 offset = default)
    {
        spawnPoint = newSpawnPoint;
        spawnOffset = offset;
    }

    // 销毁当前角色实例
    public virtual void DestroyCharacter()
    {
        if (characterInstance != null)
        {
            Destroy(characterInstance);
            characterInstance = null;
            Debug.Log($"{characterName} destroyed");
        }
    }

    // 在编辑器中可视化生成点
    protected virtual void OnDrawGizmos()
    {
        Vector3 spawnPos = GetSpawnPosition();

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPos, 0.5f);
        Gizmos.DrawIcon(spawnPos, "CharacterSpawnPoint", true);

        // 绘制偏移量指示线
        if (spawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(spawnPoint.position, spawnPos);
        }
    }
}