using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ڹ���BUG����Ĺ���Կ��
public class BUGBlockKey : Key
{
    private void Awake()
    {
        GameManager.Instance.SubscribeWhenBUGAppear(() =>//BUG����ʱ��ת״̬
        {
            SetActive(!IsActive);
            if(LockList.Count != 0) AlertPrinter.Instance.PrintLog("���������ķ���״̬����ת:" + LockList.Count, LogType.����);
        });
    }
}