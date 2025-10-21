using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//所有“钥匙”的父类
public class Key : MonoBehaviour
{
    //相关的锁列表，由对应的锁自行注册，主要用于刷新判定
    protected readonly List<Lock> LockList = new();

    [SerializeField]
    protected bool IsActive;//是否处于激活状态
    public bool Active { get => IsActive; }

    protected void Start()
    {
        SetActive(IsActive);
    }

    public void Connectwith(Lock l)
    {
        LockList.Add(l);
    }

    public virtual void SetActive(bool b)
    {
        IsActive = b;

        foreach(var l in LockList)
        {
            l.Refresh();
        }
    }
}