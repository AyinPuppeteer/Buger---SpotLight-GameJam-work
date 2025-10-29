using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

//������Ϸ�浵�Ľű�
public class GameSave : MonoBehaviour
{
    private SaveData Data;//��ǰ���еĴ浵
    public SaveData data { get => Data; }

    private const string SaveFileName = "GameSave.save";

    public static GameSave Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);//�����浵��
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Data = LoadData();
    }

    #region �浵���غͶ�ȡ
    private void SaveData()
    {
        // ���л�ΪJSON
        string json = JsonUtility.ToJson(Data);

        // ��������·��
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);

        if (File.Exists(path))
        {
            // д���ļ�
            File.WriteAllText(path, json);
            Debug.Log($"�ɹ����浽 {path}");
        }
        else Debug.LogError("�޷��浵������ϵAyin�޸���");
    }
    private SaveData LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);

        if (File.Exists(path))
        {
            // ��ȡ����
            string json = File.ReadAllText(path);
            Debug.Log($"�ɹ���ȡ�� {path}");
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.LogWarning($"�浵�ļ�������: {path}");
            return null;
        }
    }
    #endregion

    #region �����ղ�Ʒ
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

    //��ѯ����ID�ؿ���֯���ռ����
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

//�浵�ļ�
[Serializable]
public class SaveData
{
    //�ռ�����֯�ߣ���"������-���"���棩
    private Dictionary<int, int> Woves = new();
    public Dictionary<int, int> Woves_ { get => Woves; }

    private HashSet<string> FinishLevel = new();
    public HashSet<string> FinishLevel_ { get => FinishLevel; }
}