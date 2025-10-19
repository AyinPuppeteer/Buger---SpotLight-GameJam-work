using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于管理主摄像机的脚本
public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private Camera Camera;

    [SerializeField]
    private GameObject Player;//角色物体

    private void Update()
    {
        if(Player == null)//没有角色物体则利用单例化查找
        {
            
        }
        else
        {
            //跟随玩家移动
            transform.position = Player.transform.position;
        }
    }
}