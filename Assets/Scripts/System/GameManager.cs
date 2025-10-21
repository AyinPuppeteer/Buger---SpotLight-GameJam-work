using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Animator AlertAnim;

    public SavePoint SavePoint;//记录的存档点（临时）

    private readonly List<EnemySpawner> Spawners = new();//角色生成器列表

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //开始时，生成角色
        CreatePlayer();
        CreateAllEnemies();
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
        Destroy(BaseMovement.Instance.gameObject);
    }
    //重新开始
    public void GameRestart()
    {
        CreatePlayer();
        CreateAllEnemies();
    }

    //触发BUG时显示警告信息
    public void BugAlert()
    {
        
    }
}