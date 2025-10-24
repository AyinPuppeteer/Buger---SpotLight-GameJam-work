using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//警告文本
public class AlertText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Text;

    private float ExistTimer, ExistTime = 1f;

    private void Update()
    {
        ExistTimer += Time.deltaTime;
        if(ExistTimer > ExistTime)
        {
            DOTween.To(() => Text.color.a, x =>
            {
                Color c = Text.color;
                c.a = x;
                Text.color = c;
            }, 0, 0.5f).OnComplete(() => Destroy(gameObject));
        }
    }

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
        //强制刷新，确保高度读取正确
        Text.ForceMeshUpdate();
    }

    //返回文本距离
    public float ReturnHeight()
    {
        return Text.preferredHeight;
    }
}

//日志类型
public enum LogType
{
    调试, 警告, 错误
}