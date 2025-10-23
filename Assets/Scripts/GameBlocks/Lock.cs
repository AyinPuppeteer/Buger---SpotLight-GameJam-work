using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//���С������ĸ���
public abstract class Lock : MonoBehaviour
{
    //��ص�Կ���б�
    [SerializeField]
    protected List<Key> KeyList;

    public bool IsOr;//�Ƿ��ǻ��ţ�false��ʾ����Կ�׾���ʱ������true��ʾֻҪ��һ���򿪾ʹ�����

    protected bool IsActive { get; set; }//�Ƿ��ڼ���״̬
    public bool Active { get => IsActive; }

    private void Awake()
    {
        foreach(var key in KeyList)
        {
            key.Connectwith(this);
        }
    }

    //ˢ������״̬
    public virtual void Refresh()
    {
        if(IsOr)
        {
            foreach(var key in KeyList)
            {
                if(key.Active)
                {
                    SetActive(true);
                    return;
                }
            }
            SetActive(false);
        }
        else
        {
            foreach (var key in KeyList)
            {
                if (!key.Active)
                {
                    SetActive(false);
                    return;
                }
            }
            SetActive(true);
        }
    }

    public virtual void SetActive(bool b)
    {
        IsActive = b;
    }
}