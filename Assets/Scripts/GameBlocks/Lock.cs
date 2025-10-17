using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//所有“锁”的父类
public interface I_Lock
{
    //相关的钥匙列表
    protected List<I_Key> KeyList { get; }

    public bool IsOr { get; }//是否是或门（false表示所有钥匙均打开时触发，true表示只要有一个打开就触发）

    protected bool IsActive { get; set; }//是否处于激活状态
    public bool Active { get => IsActive; }

    private void Awake()
    {
        foreach(var key in KeyList)
        {
            key.Connectwith(this);
        }
    }

    //刷新自身状态
    public virtual void Refresh()
    {
        if(IsOr)
        {
            foreach(var key in KeyList)
            {
                if(key.Active)
                {
                    SetActive(true);
                }
            }
            SetActive(false);
        }
        else
        {
            foreach (var key in KeyList)
            {
                if (key.Active)
                {
                    SetActive(false);
                }
            }
            SetActive(true);
        }
    }

    public void SetActive(bool b)
    {

    }
}