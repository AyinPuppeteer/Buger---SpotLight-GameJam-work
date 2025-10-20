using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//警告文本
public class AlertText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Text;

    public void SetText(string text, LogType type)
    {
        Text.text = text;
        switch (type)
        {
            case LogType.调试:
                {
                    Text.color = Color.grey;
                    break;
                }
            case LogType.警告:
                {
                    Text.color = Color.yellow;
                    break;
                }
            case LogType.错误:
                {
                    Text.color = Color.red;
                    break;
                }
        }
    }
}

//日志类型
public enum LogType
{
    调试, 警告, 错误
}