using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���С������ĸ���
public interface I_Lock
{
    //��ص�Կ���б�
    protected List<I_Key> KeyList { get; }

    public bool IsOr { get; }//�Ƿ��ǻ��ţ�false��ʾ����Կ�׾���ʱ������true��ʾֻҪ��һ���򿪾ʹ�����

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