using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, I_Key, I_Interacts
{
    #region I_Key
    List<I_Lock> I_Key.LockList => LockList;

    bool I_Key.IsActive { get => IsActive; set => IsActive = value; }

    protected readonly List<I_Lock> LockList = new();

    protected bool IsActive;
    #endregion

    public void TakeInteract()
    {
        //更新图片


        (this as I_Key).SetActive(!IsActive);//反转激活状态
    }
}