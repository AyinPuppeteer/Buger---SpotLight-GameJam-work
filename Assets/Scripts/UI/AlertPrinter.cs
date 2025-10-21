using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertPrinter : MonoBehaviour
{
    [SerializeField]
    private GameObject LogOb;//日志文本物体

    private readonly List<AlertText> Logs = new();

    private float Offset;//垂直偏移量

    [SerializeField]
    private GameObject LogWindow;//Log窗口

    public void PrintLog(string log, LogType type)
    {
        AlertText at = Instantiate(LogOb, LogWindow.transform).GetComponent<AlertText>();
        at.GetComponent<RectTransform>().localPosition += Offset * Vector3.down;//新的文本向下移动
        at.SetText(log, type);
        Logs.Add(at);
        float delta = at.ReturnHeight();
        LogWindow.transform.DOLocalMoveY(delta, 0.5f);//缓慢向上移动
        Offset += delta;
    }


}