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
            GameManager.Instance.GameWin();
        }
        else
        {
            //����δ��ȡԿ�׵���ʾ
            Debug.Log("At least one key you haven't got exists!");
        }
    }
}