using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����ؿ��յ�Ľű�
public class FinalPoint : Lock, I_PickItem
{
    public void Pick()
    {
        if (IsActive)
        {
            //�ؿ�ʤ��
            Debug.Log("�ؿ�ʤ��");
        }
        else
        {
            //����δ��ȡԿ�׵���ʾ
        }
    }
}