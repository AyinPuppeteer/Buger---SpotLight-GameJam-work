using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ڹ�����������Ľű�
public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private Camera Camera;

    [SerializeField]
    private GameObject Player;//��ɫ����

    private void Update()
    {
        if(Player == null)//û�н�ɫ���������õ���������
        {
            
        }
        else
        {
            //��������ƶ�
            transform.position = Player.transform.position;
        }
    }
}