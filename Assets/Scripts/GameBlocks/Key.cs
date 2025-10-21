using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���С�Կ�ס��ĸ���
public class Key : MonoBehaviour
{
    //��ص����б��ɶ�Ӧ��������ע�ᣬ��Ҫ����ˢ���ж�
    protected readonly List<Lock> LockList = new();

    [SerializeField]
    protected bool IsActive;//�Ƿ��ڼ���״̬
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