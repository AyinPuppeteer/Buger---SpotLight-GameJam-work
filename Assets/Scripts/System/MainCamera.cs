using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于管理主摄像机的脚本
public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private Camera Camera;

    private GameObject Player;//角色物体

    private bool IsFlipped;//是否反转
    public bool IsFlipped_ { get => IsFlipped; }

    public static MainCamera Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        if (GameManager.Instance == null) return;//没在关卡中就跳出

        if(Player == null)//没有角色物体则利用单例化查找
        {
            if(BaseMovement.Instance != null)
            {
                Player = BaseMovement.Instance.gameObject;
            }
        }
        else
        {
            if(Player.transform.position.y > 0f)
            {
                //跟随玩家移动
                transform.position = new(transform.position.x, Player.transform.position.y, transform.position.z);//只移动Y轴
            }
            else if(Player.transform.position.y < -3f)
            {
                //跟随玩家移动
                transform.position = new(transform.position.x, Player.transform.position.y + 3f, transform.position.z);//只移动Y轴
            }
            else
            {
                //回归原点
                transform.position = new(transform.position.x, 0, transform.position.z);
            }
        }

    }

    //翻转摄像头
    public void FlipCamera()
    {
        IsFlipped = !IsFlipped;
        transform.Rotate(new(0, 180, 180));

        if(BaseMovement.Instance != null)
        {
            BaseMovement.Instance.ActivateBUG2();
        }
    }
}