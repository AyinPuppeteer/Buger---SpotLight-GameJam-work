using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���С�Կ�ס��ĸ���
public class Key : MonoBehaviour
{
    //��ص����б��ɶ�Ӧ��������ע�ᣬ��Ҫ����ˢ���ж�
    protected List<Lock> LockList { get; }

    protected bool IsActive { get; set; }//�Ƿ��ڼ���״̬
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