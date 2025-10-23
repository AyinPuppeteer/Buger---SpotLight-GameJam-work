using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ڹ�����������Ľű�
public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private Camera Camera;

    private GameObject Player;//��ɫ����

    private void LateUpdate()
    {
        if(Player == null)//û�н�ɫ���������õ���������
        {
            if(BaseMovement.Instance != null)
            {
                Player = BaseMovement.Instance.gameObject;
            }
        }
        else
        {
            //��������ƶ�
            transform.position = Player.transform.position;
        }
    }
}