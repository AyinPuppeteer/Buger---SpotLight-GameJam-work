using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//������ֻ���ʽ�ı��Ľű�
public class TMPPrinter : MonoBehaviour
{
    private TextMeshProUGUI Text;

    private string AimText = "";

    private float PrintTimer;//���ּ�ʱ��
    public float PrintTime = 0.05f;//���ּ��

    public bool IsFinished { get => Text.text.Length >= AimText.Length; }

    private void Awake()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if(!IsFinished)
        {
            if ((PrintTimer += Time.deltaTime) >= PrintTime)
            {
                PrintTimer = 0;

                Text.text += AimText[Text.text.Length];
            }
        }
    }

    public void SetTextForce(string text)
    {
        Text.text = text;
        AimText = text;
    }

    public void SetText(string text)
    {
        AimText = text;
    }
    public void AddText(string text)
    {
        AimText += text;
    }
}