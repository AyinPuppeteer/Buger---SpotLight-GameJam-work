using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������򣨲ݴԣ��Ľű�
public class HideField : MonoBehaviour, I_Interacts
{
    public void TakeInteract()
    {
        if (true)//�Ƿ��ڡ�����״̬��
        {
            if (BaseMovement.Instance != null)
            {
                BaseMovement.Instance.SetDetectable(false);//ʹĿ����롰����״̬��
            }
        }
        else
        {
            if (BaseMovement.Instance != null)
            {
                BaseMovement.Instance.SetDetectable(false);//ʹĿ����������״̬��
            }
        }
    }
}