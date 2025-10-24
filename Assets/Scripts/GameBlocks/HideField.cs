using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//隐藏区域（柜子）的脚本
public class HideField : MonoBehaviour, I_Interacts
{
    public void TakeInteract()
    {
        if (BaseMovement.Instance != null) 
        {
            if (BaseMovement.Instance.IsDetectable_)
            {
                BaseMovement.Instance.SetDetectable(false);
            }
            else if (!BaseMovement.Instance.IsDetectable_)
            {
                BaseMovement.Instance.SetDetectable(true);
            }
        }
    }
}