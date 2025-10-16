using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//扫描器的脚本
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
    private float ActTimer;//行动时间，行动计时器

    public float Radius;//扫描半径

    private void Update()
    {
        if (IsActive)
        {
            if ((ActTimer += Time.deltaTime) >= ActTime)
            {
                ActTimer = 0;
                TakeScan();//计时扫描
            }
        }
        else ActTimer = 0;//关闭时清空计时
    }

    //进行扫描

    private void TakeScan()
    {

    }
}