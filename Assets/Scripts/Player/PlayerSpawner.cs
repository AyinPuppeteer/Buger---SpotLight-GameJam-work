using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject PlayerPrefab;

    private GameObject Player;

    private void Start()
    {
        //开始时召唤玩家物体
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        Player = Instantiate(PlayerPrefab);
    }

    private void SetPlayerPosition(Vector2 pos)
    {
        Player.transform.position = pos;
    }

    // 在编辑器中可视化生成点
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        Gizmos.DrawIcon(Vector3.zero, "PlayerSpawnPoint", true);
    }
}
