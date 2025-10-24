using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//隐藏区域（草丛）的脚本
public class HideField : MonoBehaviour, I_Interacts
{
    public void TakeInteract()
    {
        if (true)//是否处于“隐匿状态”
        {
            if (BaseMovement.Instance != null)
            {
                BaseMovement.Instance.SetDetectable(false);//使目标进入“隐匿状态”
            }
        }
        else
        {
            if (BaseMovement.Instance != null)
            {
                BaseMovement.Instance.SetDetectable(false);//使目标解除“隐匿状态”
            }
        }
    }
}