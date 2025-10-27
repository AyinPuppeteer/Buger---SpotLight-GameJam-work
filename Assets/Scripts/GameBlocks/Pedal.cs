using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedal : Key
{
    [SerializeField]
    private SpriteRenderer Image;
    [SerializeField]
    private Sprite OpenImg, CloseImg;

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

    private void Update()
    {
        Debug.DrawRay(transform.position + new Vector3(-0.08f, 0.05f, 0), Vector3.right * 0.16f, Color.red);
        foreach (var info in Physics2D.LinecastAll(transform.position + new Vector3(-0.08f, 0.05f, 0), transform.position + new Vector3(0.08f, 0.05f, 0)))
        {
            if(info.collider.GetComponent<CharacterBase>() != null)//��⵽��ɫ���л�Ϊ����״̬ 
            {
                if (!IsActive) SetActive(true);
                return;
            }
        }

        if (IsActive) SetActive(false);//��ⲻ����ɫ���л�Ϊ�ر�״̬ 
    }
}