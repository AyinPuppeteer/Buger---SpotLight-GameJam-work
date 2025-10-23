using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlertPrinter : MonoBehaviour
{
    [SerializeField]
    private GameObject LogOb;//��־�ı�����

    private readonly List<AlertText> Logs = new();

    private float Offset;//��ֱƫ����

    [SerializeField]
    private GameObject LogWindow;//Log����

    public static AlertPrinter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void PrintLog(string log, LogType type)
    {
        AlertText at = Instantiate(LogOb, LogWindow.transform).GetComponent<AlertText>();
        at.GetComponent<RectTransform>().localPosition += Offset * Vector3.down;//�µ��ı������ƶ�
        at.SetText(log + " (λ��: " + SceneManager.GetActiveScene().name + ")", type);
        Logs.Add(at);
        Offset += at.ReturnHeight() + 20;
        DOTween.To(() => LogWindow.transform.localPosition.y, x => 
        {
            Vector3 pos = LogWindow.transform.localPosition;
            pos.y = x;
            LogWindow.transform.localPosition = pos; 
        }, Offset, 0.5f);//���������ƶ�
    }


}