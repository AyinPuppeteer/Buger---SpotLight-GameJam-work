using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Key, I_Interacts
{
    public void TakeInteract()
    {
        //更新图片

        SetActive(!IsActive);//反转激活状态
    }
}