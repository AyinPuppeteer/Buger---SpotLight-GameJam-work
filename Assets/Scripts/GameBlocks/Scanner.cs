using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ɨ�����Ľű�
public class Scanner : Lock
{
    [SerializeField]
    private SpriteRenderer Image;
    [SerializeField]
    private Sprite OpenImg, CloseImg;

    private const float ActTime = 10f;
    private float ActTimer;//�ж�ʱ�䣬�ж���ʱ��

    public float Radius;//ɨ��뾶

    private void Update()
    {
        if (IsActive)
        {
            if ((ActTimer += Time.deltaTime) >= ActTime)
            {
                ActTimer = 0;
                TakeScan();//��ʱɨ��
            }
        }
        else ActTimer = 0;//�ر�ʱ��ռ�ʱ
    }

    //����ɨ��
    private void TakeScan()
    {

    }

    public override void SetActive(bool b)
    {
        base.SetActive(b);
        if (b)
        {
            Image.sprite = OpenImg;
        }
        else
        {
            Image.sprite = CloseImg;
        }
    }
}