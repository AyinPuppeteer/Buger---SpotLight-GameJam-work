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

    private bool IsPicked;//�Ƿ���ʰȡ

    public void Pick()
    {
        if (!IsPicked)//δʰȡʱ�Ž���ʰȡ����
        {
            //�л�ͼƬΪ�Ѽ�ȡ״̬
            (this as I_Key).SetActive(true);
        }
    }
}