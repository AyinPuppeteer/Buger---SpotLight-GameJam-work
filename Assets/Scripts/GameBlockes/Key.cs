using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���С�Կ�ס��ĸ���
public abstract class Key : MonoBehaviour
{
    //��ص����б��ɶ�Ӧ��������ע�ᣬ��Ҫ����ˢ���ж�
    private readonly List<Lock> LockList = new();

    private bool IsActive;//�Ƿ��ڼ���״̬
    public bool Active { get => IsActive; }

    public void Connectwith(Lock l)
    {
        LockList.Add(l);
    }

    public virtual void SetActive(bool b)
    {
        if (IsActive == b) return;//ֵû��Ͳ�������
        IsActive = b;

        foreach(var l in LockList)
        {
            l.Refresh();
        }
    }
}