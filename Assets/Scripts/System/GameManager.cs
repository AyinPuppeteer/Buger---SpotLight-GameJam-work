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

    private SavePoint SavePoint;//记录的存档点
    [SerializeField]
    private SavePoint StartSavePoint;//起始记录点

    private readonly List<EnemySpawner> Spawners = new();//角色生成器列表

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //设置起始出生点
        SetSavePoint(StartSavePoint);
        //开始时，生成角色
        CreatePlayer();
        CreateAllEnemies();

        AlertPrinter.Instance.PrintLog("警告：检测到未知实体！", LogType.警告);
    }

    #region 角色生成
    public void AddSpawner(EnemySpawner spawner)
    {
        Spawners.Add(spawner);
    }
    //生成玩家
    private void CreatePlayer()
    {
        PlayerSpawner.Instance.SpawnAtPosition(SavePoint.Position);
    }
    //生成所有敌人
    private void CreateAllEnemies()
    {
        foreach(var spawner in Spawners)
        {
            //将所有的敌人生成器重置
            spawner.SpawnEnemy();
        }
    }
    #endregion

    public void SetSavePoint(SavePoint sp)
    {
        SavePoint = sp;
    }

    //游戏胜利
    public void GameWin()
    {
        Debug.Log("You win the Game!");
    }
    //游戏失败
    public void GameOver()
    {
        if(BaseMovement.Instance != null)
        {
            if (BaseMovement.Instance.bug2Active_)
            {
                BaseMovement.Instance.FlipAllSprites();
                BaseMovement.Instance.FlipCamera();
            }
        }

        Destroy(BaseMovement.Instance.gameObject);
        AlertPrinter.Instance.PrintLog("δ֪ʵ����������������ɡ�", LogType.����);
        PlayerDisexposed();

        FadeEvent.Instance.FakeFade();
        DOTween.To(() => 0, x => { }, 0, 0.8f).OnComplete(GameRestart);
    }
    //重新开始
    public void GameRestart()
    {
        CreatePlayer();
        CreateAllEnemies();
    }

    //角色处于暴露状态时修改
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