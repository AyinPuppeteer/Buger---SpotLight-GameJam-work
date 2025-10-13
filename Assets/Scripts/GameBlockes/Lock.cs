using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//所有“锁”的父类
public abstract class Lock : MonoBehaviour
{
    //相关的钥匙列表
    [SerializeField]
    private List<Key> KeyList;

    [SerializeField]
    private bool IsOr;//是否是或门（false表示所有钥匙均打开时触发，true表示只要有一个打开就触发）

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

                }
            }
        }
        else
        {

        }
    }

    protected virtual void SetActive(bool b)
    {

    }
}