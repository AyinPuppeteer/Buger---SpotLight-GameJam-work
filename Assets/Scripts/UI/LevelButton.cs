using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//关卡选择按钮的脚本
public class LevelButton : MonoBehaviour
{
    private Button Button;

    [SerializeField]
    private TextMeshProUGUI Text;

    [SerializeField]
    private int Level;//关卡编号

    private void Awake()
    {
        Button = GetComponent<Button>();
    }

    private void Start()
    {
        Text.text = Level.ToString();

        //根据解锁情况判断是否激活按钮
    }

    //点击时
    public void WhenClick()
    {
        LevelManager.Instance.ChooseLevel(Level);
    }
}