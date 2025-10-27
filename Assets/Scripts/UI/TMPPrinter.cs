using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//管理打字机样式文本的脚本
public class TMPPrinter : MonoBehaviour
{
    private TextMeshProUGUI Text;

    private string AimText = "";

    private float PrintTimer;//打字计时器
    public float PrintTime = 0.05f;//打字间隔

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