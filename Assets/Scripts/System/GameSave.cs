using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理游戏存档的脚本
public class GameSave : MonoBehaviour
{
    private SaveData Data;//当前运行的存档
    public SaveData data { get => Data; }

    public static GameSave Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);//永续存档类
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region 存档加载和读取
    private void LoadData()
    {

    }
    private void SaveData()
    {

    }
    #endregion

    #region 管理收藏品
    public void GainThread(string SceneName, int ID)
    {
        Data.Threads_.Add(SceneName + "-" + ID);
    }
    
    public bool CheckThread(string SceneName, int ID)
    {
        if (Data.Threads_.Contains(SceneName + "-" + ID)) return true;
        else return false;
    }
    #endregion
}

//存档文件
[Serializable]
public class SaveData
{
    //收集到的织线（以"场景名-编号"储存）
    private HashSet<string> Threads = new();
    public HashSet<string> Threads_ { get => Threads; }

    private HashSet<string> FinishLevel = new();
    public HashSet<string> FinishLevel_ { get => FinishLevel; }
}