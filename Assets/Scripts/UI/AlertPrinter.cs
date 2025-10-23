using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlertPrinter : MonoBehaviour
{
    [SerializeField]
    private GameObject LogOb;//日志文本物体

    private readonly List<AlertText> Logs = new();

    private float Offset;//垂直偏移量

    [SerializeField]
    private GameObject LogWindow;//Log窗口

    public static AlertPrinter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void PrintLog(string log, LogType type)
    {
        AlertText at = Instantiate(LogOb, LogWindow.transform).GetComponent<AlertText>();
        at.GetComponent<RectTransform>().localPosition += Offset * Vector3.down;//新的文本向下移动
        at.SetText(log + " (位置: " + SceneManager.GetActiveScene().name + ")", type);
        Logs.Add(at);
        Offset += at.ReturnHeight() + 20;
        DOTween.To(() => LogWindow.transform.localPosition.y, x => 
        {
            Vector3 pos = LogWindow.transform.localPosition;
            pos.y = x;
            LogWindow.transform.localPosition = pos; 
        }, Offset, 0.5f);//缓慢向上移动
    }


}