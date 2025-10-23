using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//隐藏区域（草丛）的脚本
public class HideField : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        CharacterBase cb = other.GetComponent<CharacterBase>();
        if (cb != null)
        {
            cb.SetDetectable(false);//使目标进入“隐匿状态”
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CharacterBase cb = other.GetComponent<CharacterBase>();
        if (cb != null)
        {
            cb.SetDetectable(true);//使目标解除“隐匿状态”
        }
    }
}