using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���С������ĸ���
public abstract class Lock : MonoBehaviour
{
    //��ص�Կ���б�
    [SerializeField]
    private List<Key> KeyList;

    [SerializeField]
    private bool IsOr;//�Ƿ��ǻ��ţ�false��ʾ����Կ�׾���ʱ������true��ʾֻҪ��һ���򿪾ʹ�����

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