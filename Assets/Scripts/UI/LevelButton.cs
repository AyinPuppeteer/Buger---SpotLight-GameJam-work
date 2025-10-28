using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//�ؿ�ѡ��ť�Ľű�
public class LevelButton : MonoBehaviour
{
    private Button Button;

    [SerializeField]
    private TextMeshProUGUI Text;

    [SerializeField]
    private int Level;//�ؿ����

    private void Awake()
    {
        Button = GetComponent<Button>();
    }

    private void Start()
    {
        Text.text = Level.ToString();

        //���ݽ�������ж��Ƿ񼤻ť
    }

    //���ʱ
    public void WhenClick()
    {
        LevelManager.Instance.ChooseLevel(Level);
    }
}