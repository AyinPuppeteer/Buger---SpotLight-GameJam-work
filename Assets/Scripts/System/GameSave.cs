using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������Ϸ�浵�Ľű�
public class GameSave : MonoBehaviour
{
    private SaveData Data;//��ǰ���еĴ浵
    public SaveData data { get => Data; }

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

        Data = new();
    }

    #region �浵���غͶ�ȡ
    private void LoadData()
    {

    }
    private void SaveData()
    {

    }
    #endregion

    #region �����ղ�Ʒ
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

    public bool LevelFinish(int id)
    {
        if (data.FinishLevel_.Contains(id.ToString()))
        {
            return true;
        }
        else return false;
    }
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