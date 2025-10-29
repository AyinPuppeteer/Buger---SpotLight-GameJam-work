using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于管理BUG方块的公共钥匙
public class BUGBlockKey : Key
{
    private void Awake()
    {
        GameManager.Instance.SubscribeWhenBUGAppear(() =>//BUG出现时翻转状态
        {
            SetActive(!IsActive);
            if(LockList.Count != 0) AlertPrinter.Instance.PrintLog("以下数量的方块状态被反转:" + LockList.Count, LogType.调试);
        });
    }
}