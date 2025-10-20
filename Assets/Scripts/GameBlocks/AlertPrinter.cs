using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertPrinter : MonoBehaviour
{
    private GameObject LogOb;//日志文本物体

    private readonly List<AlertText> Alerts = new();

    public void PrintLog(string log, LogType type)
    {
        AlertText at = Instantiate(LogOb).GetComponent<AlertText>();
        at.SetText(log, type);
        Alerts.Add(at);
    }
}