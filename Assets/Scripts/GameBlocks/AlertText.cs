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
    }
}

//��־����
public enum LogType
{
    ����, ����, ����
}