using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertPrinter : MonoBehaviour
{
    private GameObject LogOb;//��־�ı�����

    private readonly List<AlertText> Logs = new();

    [SerializeField]
    private GameObject LogWindow;//Log����

    public void PrintLog(string log, LogType type)
    {
        AlertText at = Instantiate(LogOb).GetComponent<AlertText>();
        at.SetText(log, type);
        Logs.Add(at);
    }


}