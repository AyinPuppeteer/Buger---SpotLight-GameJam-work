using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于管理主摄像机的脚本
public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private Camera Camera;

    private GameObject Player;//角色物体

    private void LateUpdate()
    {
        if(Player == null)//没有角色物体则利用单例化查找
        {
            if(BaseMovement.Instance != null)
            {
                Player = BaseMovement.Instance.gameObject;
            }
        }
        else
        {
            //跟随玩家移动
            transform.position = Player.transform.position;
        }
    }
}