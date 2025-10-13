using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//所有“钥匙”的父类
public abstract class Key : MonoBehaviour
{
    //相关的锁列表，由对应的锁自行注册，主要用于刷新判定
    private List<Lock> LockList;

    private bool IsActive;//是否处于激活状态
    public bool Active { get => IsActive; }

    public void Connectwith(Lock l)
    {
        LockList.Add(l);
    }

    protected virtual void SetActive(bool b)
    {
        if (IsActive == b) return;//值没变就不做操作
        IsActive = b;

        foreach(var l in LockList)
        {
            l.Refresh();
        }
    }
}