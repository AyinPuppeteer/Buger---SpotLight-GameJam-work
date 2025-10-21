using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachKey : Key, I_PickItem
{
    [SerializeField]
    private SpriteRenderer Image;

    private bool IsPicked;//�Ƿ���ʰȡ

    public void Pick()
    {
        if (!IsPicked)//δʰȡʱ�Ž���ʰȡ����
        {
            //�л�ͼƬΪ�Ѽ�ȡ״̬
            Image.enabled = false;
            SetActive(true);
            IsPicked = true;
        }
    }
}