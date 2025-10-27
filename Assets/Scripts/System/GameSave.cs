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
            return;
        }

        Data = new();
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
    public void GainWove(int Level, int ID)
    {
        if (Data.Woves_.ContainsKey(Level))
        {
            Data.Woves_[Level] &= (1 << ID);
        }
    }
    
    public bool CheckWove(int Level, int ID)
    {
        if (Data.Woves_.ContainsKey(Level) & (1 << ID) > 0) return true;
        else return false;
    }

    //查询给定ID关卡的织线收集情况
    public int WoveAtLevel(int ID)
    {
        if (!Data.Woves_.ContainsKey(ID)) return 0;
        int info = Data.Woves_[ID];
        int cnt = 0;
        while (info > 0)
        {
            if (info % 2 == 1) cnt++;
            info /= 2;
        }
        return cnt;
    }
    #endregion

    public bool LevelFinish(int id)
    {
        if (data.FinishLevel_.Contains(id.ToString()))
        {
            return true;
        }
        else return false;
    }
}

//存档文件
[Serializable]
public class SaveData
{
    //收集到的织线（以"场景名-编号"储存）
    private Dictionary<int, int> Woves = new();
    public Dictionary<int, int> Woves_ { get => Woves; }

    private HashSet<string> FinishLevel = new();
    public HashSet<string> FinishLevel_ { get => FinishLevel; }
}