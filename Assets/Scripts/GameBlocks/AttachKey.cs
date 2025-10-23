using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachKey : Key, I_PickItem
{
    [SerializeField]
    private SpriteRenderer Image;

    private bool IsPicked;//是否已拾取

    public void Pick()
    {
        if (!IsPicked)//未拾取时才进行拾取操作
        {
            //切换图片为已捡取状态
            Image.enabled = false;
            SetActive(true);
            IsPicked = true;
        }
    }
}