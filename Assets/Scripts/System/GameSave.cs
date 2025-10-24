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
        }
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

//�浵�ļ�
[Serializable]
public class SaveData
{
    //�ռ�����֯�ߣ���"������-���"���棩
    private HashSet<string> Threads = new();
    public HashSet<string> Threads_ { get => Threads; }

    private HashSet<string> FinishLevel = new();
    public HashSet<string> FinishLevel_ { get => FinishLevel; }
}