using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Key, I_Interacts
{
    [SerializeField]
    private SpriteRenderer Image;
    [SerializeField]
    private Sprite OpenImg, CloseImg;

    public void TakeInteract()
    {
        SetActive(!IsActive);//��ת����״̬
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