using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������򣨲ݴԣ��Ľű�
public class HideField : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CharacterBase cb = other.GetComponent<CharacterBase>();
        if (cb != null)
        {
            //ʹĿ����롰����״̬��
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterBase cb = other.GetComponent<CharacterBase>();
        if (cb != null)
        {
            //ʹĿ����������״̬��
        }
    }
}