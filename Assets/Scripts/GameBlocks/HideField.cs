using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//隐藏区域（草丛）的脚本
public class HideField : MonoBehaviour, I_Interacts
{
    public void TakeInteract()
    {
        if (BaseMovement.Instance != null && BaseMovement.Instance.IsDetectable_)
        {
            BaseMovement.Instance.SetDetectable(false);
        }
        else if (BaseMovement.Instance != null && !BaseMovement.Instance.IsDetectable_)
        {
            BaseMovement.Instance.SetDetectable(true);
        }
    }
}