using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

//管理游戏存档的脚本
public class GameSave : MonoBehaviour
{
    private SaveData Data;//当前运行的存档
    public SaveData data { get => Data; }

    private const string SaveFileName = "GameSave.save";

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

        Data = LoadData();
    }

    #region 存档加载和读取
    private void SaveData()
    {
        // 序列化为JSON
        string json = JsonUtility.ToJson(Data);

        // 构建保存路径
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);

        if (File.Exists(path))
        {
            // 写入文件
            File.WriteAllText(path, json);
            Debug.Log($"成功保存到 {path}");
        }
        else Debug.LogError("无法存档，请联系Ayin修复！");
    }
    private SaveData LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);

        if (File.Exists(path))
        {
            // 读取内容
            string json = File.ReadAllText(path);
            Debug.Log($"成功读取到 {path}");
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.LogWarning($"存档文件不存在: {path}");
            return null;
        }
    }
    #endregion

    #region 管理收藏品
    public void GainWove(int Level, int ID)
    {
        if (Data.Woves_.ContainsKey(Level))
        {
            Data.Woves_[Level] &= (1 << ID);
            SaveData();
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

    public void LevelFinish(int id)
    {
        if (!data.FinishLevel_.Contains(id.ToString()))
        {
            data.FinishLevel_.Add(id.ToString());
            SaveData();
        }
    }
    public bool CheckLevel(int id) => data.FinishLevel_.Contains(id.ToString());
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