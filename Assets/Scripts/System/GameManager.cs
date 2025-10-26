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
        AlertPrinter.Instance.PrintLog("已生成警卫实体: " + Spawners.Count, LogType.调试);
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
        Destroy(BaseMovement.Instance.gameObject);
        AlertPrinter.Instance.PrintLog("已控制位置实体，执行摧毁操作", LogType.调试);
        PlayerDisexposed();

        DOTween.To(() => 0, x => { }, 0, 1f).OnComplete(() =>
        {
            FadeEvent.Instance.FakeFade();
            DOTween.To(() => 0, x => { }, 0, 0.8f).OnComplete(GameRestart);
        });
    }
    //重新开始
    public void GameRestart()
    {
        MainCamera.Instance.Reset();

        CreatePlayer();
        AlertPrinter.Instance.PrintLog("错误：目标实体摧毁失败！原因：未拥有权限", LogType.错误);
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