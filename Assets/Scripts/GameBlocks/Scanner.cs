using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//扫描器的脚本
public class Scanner : Lock
{
    [SerializeField]
    private SpriteRenderer Image;
    [SerializeField]
    private Sprite OpenImg, CloseImg;

    [SerializeField]
    private GameObject WaveOb;//扫描波物体

    private const float ActTime = 5f;
    private float ActTimer;//行动时间，行动计时器

    public float Radius;//扫描半径

    private void Update()
    {
        if (IsActive)
        {
            if ((ActTimer += Time.deltaTime) >= ActTime)
            {
                ActTimer = 0;
                TakeScan();//计时扫描
            }
        }
        else ActTimer = 0;//关闭时清空计时
    }

    //进行扫描
    private void TakeScan()
    {
        GameObject ob = Instantiate(WaveOb, transform.position, Quaternion.identity, transform);//生成波物体
        ob.GetComponent<ScanWave>().SetRadius(Radius);
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