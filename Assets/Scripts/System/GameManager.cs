using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Animator AlertAnim;

    public SavePoint SavePoint;//��¼�Ĵ浵�㣨��ʱ��

    private readonly List<EnemySpawner> Spawners = new();//��ɫ�������б�

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //��ʼʱ�����ɽ�ɫ
        CreatePlayer();
        CreateAllEnemies();
    }

    #region ��ɫ����
    public void AddSpawner(EnemySpawner spawner)
    {
        Spawners.Add(spawner);
    }
    //�������
    private void CreatePlayer()
    {
        PlayerSpawner.Instance.SpawnAtPosition(SavePoint.Position);
    }
    //�������е���
    private void CreateAllEnemies()
    {
        foreach(var spawner in Spawners)
        {
            //�����еĵ�������������
            spawner.SpawnEnemy();
        }
    }
    #endregion

    public void SetSavePoint(SavePoint sp)
    {
        SavePoint = sp;
    }

    //��Ϸʤ��
    public void GameWin()
    {
        Debug.Log("You win the Game!");
    }
    //��Ϸʧ��
    public void GameOver()
    {
        Destroy(BaseMovement.Instance.gameObject);
    }
    //���¿�ʼ
    public void GameRestart()
    {
        CreatePlayer();
        CreateAllEnemies();
    }

    //����BUGʱ��ʾ������Ϣ
    public void BugAlert()
    {
        
    }
}