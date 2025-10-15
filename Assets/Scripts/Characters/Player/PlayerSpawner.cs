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
        //��ʼʱ�ٻ��������
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

    // �ڱ༭���п��ӻ����ɵ�
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        Gizmos.DrawIcon(Vector3.zero, "PlayerSpawnPoint", true);
    }
}
