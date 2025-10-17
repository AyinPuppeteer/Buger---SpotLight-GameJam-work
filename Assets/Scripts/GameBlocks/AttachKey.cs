using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachKey : MonoBehaviour, I_PickItem, I_Key
{
    #region I_Key
    List<I_Lock> I_Key.LockList => LockList;

    bool I_Key.IsActive { get => IsActive; set => IsActive = value; }

    protected readonly List<I_Lock> LockList = new();

    protected bool IsActive;
    #endregion

    [SerializeField]
    private SpriteRenderer Image;

    private bool IsPicked;//是否已拾取

    public void Pick()
    {
        if (!IsPicked)//未拾取时才进行拾取操作
        {
            //切换图片为已捡取状态
            (this as I_Key).SetActive(true);
        }
    }
}