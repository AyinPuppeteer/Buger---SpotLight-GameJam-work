using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ɨ�����Ľű�
public class Scanner : MonoBehaviour, I_Lock
{
    #region I_Lock
    List<I_Key> I_Lock.KeyList { get => KeyList; }
    public List<I_Key> KeyList = new();

    bool I_Lock.IsOr { get => IsOr; }
    public bool IsOr;

    bool I_Lock.IsActive { get => IsActive; set => IsActive = value; }

    protected bool IsActive;
    #endregion

    private const float ActTime = 10f;
    private float ActTimer;//�ж�ʱ�䣬�ж���ʱ��

    public float Radius;//ɨ��뾶

    private void Update()
    {
        if (IsActive)
        {
            if ((ActTimer += Time.deltaTime) >= ActTime)
            {
                ActTimer = 0;
                TakeScan();//��ʱɨ��
            }
        }
        else ActTimer = 0;//�ر�ʱ��ռ�ʱ
    }

    //����ɨ��

    private void TakeScan()
    {

    }
}