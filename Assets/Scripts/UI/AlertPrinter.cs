using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertPrinter : MonoBehaviour
{
    [SerializeField]
    private GameObject LogOb;//��־�ı�����

    private readonly List<AlertText> Logs = new();

    private float Offset;//��ֱƫ����

    [SerializeField]
    private GameObject LogWindow;//Log����

    public void PrintLog(string log, LogType type)
    {
        AlertText at = Instantiate(LogOb, LogWindow.transform).GetComponent<AlertText>();
        at.GetComponent<RectTransform>().localPosition += Offset * Vector3.down;//�µ��ı������ƶ�
        at.SetText(log, type);
        Logs.Add(at);
        float delta = at.ReturnHeight();
        LogWindow.transform.DOLocalMoveY(delta, 0.5f);//���������ƶ�
        Offset += delta;
    }


}