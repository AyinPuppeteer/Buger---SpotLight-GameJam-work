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

    [SerializeField]
    private GameObject WaveOb;//ɨ�貨����

    private const float ActTime = 5f;
    private float ActTimer;//�ж�ʱ�䣬�ж���ʱ��

    public float Radius;//ɨ��뾶

    private void Start()
    {
        Refresh();
    }

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
        GameObject ob = Instantiate(WaveOb, transform.position, Quaternion.identity, transform);//���ɲ�����
        ob.GetComponent<ScanWave>().SetRadius(Radius);
    }

    protected override void SetActive(bool b)
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