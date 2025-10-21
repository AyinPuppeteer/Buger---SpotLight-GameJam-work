using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����ؿ��յ�Ľű�
public class FinalPoint : Lock, I_PickItem
{
    [SerializeField]
    private SpriteRenderer Image;
    [SerializeField]
    private Sprite CloseImg;
    [SerializeField]
    private Sprite OpenImg;

    public override void SetActive(bool b)
    {
        if (b)
        {
            Image.sprite = OpenImg;
        }
        else
        {
            Image.sprite = CloseImg;
        }
    }

    public void Pick()
    {
        if (IsActive)
        {
            //�ؿ�ʤ��
            GameManager.Instance.GameWin();
        }
        else
        {
            //����δ��ȡԿ�׵���ʾ
            Debug.Log("At least one key you haven't got exists!");
        }
    }
}