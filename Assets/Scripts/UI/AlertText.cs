using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//�����ı�
public class AlertText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Text;

    public void SetText(string text, LogType type)
    {
        Text.text = text;
        switch (type)
        {
            case LogType.����:
                {
                    Text.color = Color.grey;
                    break;
                }
            case LogType.����:
                {
                    Text.color = Color.yellow;
                    break;
                }
            case LogType.����:
                {
                    Text.color = Color.red;
                    break;
                }
        }
        //ǿ��ˢ�£�ȷ���߶ȶ�ȡ��ȷ
        Text.ForceMeshUpdate();
    }

    //�����ı�����
    public float ReturnHeight()
    {
        return Text.preferredHeight;
    }
}

//��־����
public enum LogType
{
    ����, ����, ����
}