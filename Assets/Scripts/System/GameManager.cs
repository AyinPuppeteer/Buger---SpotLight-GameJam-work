using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Animator AlertAnim;

    private SavePoint SavePoint;//��¼�Ĵ浵��
    [SerializeField]
    private SavePoint StartSavePoint;//��ʼ��¼��

    private readonly List<EnemySpawner> Spawners = new();//��ɫ�������б�

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //������ʼ������
        SetSavePoint(StartSavePoint);
        //��ʼʱ�����ɽ�ɫ
        CreatePlayer();
        CreateAllEnemies();

        AlertPrinter.Instance.PrintLog("���棺��⵽δ֪ʵ�壡", LogType.����);
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
        AlertPrinter.Instance.PrintLog("�����ɾ���ʵ��: " + Spawners.Count, LogType.����);
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
        AlertPrinter.Instance.PrintLog("�ѿ���λ��ʵ�壬ִ�дݻٲ���", LogType.����);
        PlayerDisexposed();

        DOTween.To(() => 0, x => { }, 0, 1f).OnComplete(() =>
        {
            FadeEvent.Instance.FakeFade();
            DOTween.To(() => 0, x => { }, 0, 0.8f).OnComplete(GameRestart);
        });
    }
    //���¿�ʼ
    public void GameRestart()
    {
        MainCamera.Instance.Reset();

        CreatePlayer();
        AlertPrinter.Instance.PrintLog("����Ŀ��ʵ��ݻ�ʧ�ܣ�ԭ��δӵ��Ȩ��", LogType.����);
        CreateAllEnemies();
    }

    //��ɫ���ڱ�¶״̬ʱ�޸�
    public void PlayerExposed()
    {
        AlertAnim.SetInteger("Anim", 1);

        foreach(var guard in CharacterManager.Instance.ReturnAllEnemies())
        {
            guard.SetActive(true);
        }
    }
    public void PlayerDisexposed()
    {
        AlertAnim.SetInteger("Anim", 0);

        foreach (var guard in CharacterManager.Instance.ReturnAllEnemies())
        {
            guard.SetActive(false);
        }
    }
}