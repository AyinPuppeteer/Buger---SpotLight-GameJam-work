using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���С������ĸ���
public interface I_Lock
{
    //��ص�Կ���б�
    [SerializeField]
    protected List<I_Key> KeyList { get; set; }

    [SerializeField]
    protected bool IsOr { get; set; }//�Ƿ��ǻ��ţ�false��ʾ����Կ�׾���ʱ������true��ʾֻҪ��һ���򿪾ʹ�����

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

    public virtual void SetActive(bool b)
    {

    }
}