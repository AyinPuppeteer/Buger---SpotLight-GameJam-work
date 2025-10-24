using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//�����ı�
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