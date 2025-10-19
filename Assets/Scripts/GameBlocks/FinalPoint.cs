using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理关卡终点的脚本
public class FinalPoint : Lock, I_PickItem
{
    public void Pick()
    {
        if (IsActive)
        {
            //关卡胜利
            Debug.Log("关卡胜利");
        }
        else
        {
            //给出未拿取钥匙的提示
        }
    }
}