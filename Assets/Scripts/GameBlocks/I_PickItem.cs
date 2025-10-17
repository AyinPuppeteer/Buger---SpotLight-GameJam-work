using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理所有拾取物的接口
public interface I_PickItem
{
    //拾取时函数（接触即拾取）
    public void Pick();
}