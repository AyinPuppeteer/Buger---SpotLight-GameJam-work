using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于管理主摄像机的脚本
public class MainCamera : MonoBehaviour
{
    public static MainCamera Instance { get; private set; }

    [SerializeField]
    private Camera Camera;

    private GameObject Player;//角色物体

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
            //跟随玩家移动
            transform.position = new(transform.position.x, Player.transform.position.y, transform.position.z);//只移动Y轴
        }

    }

    //翻转摄像头
    public void FlipCamera()
    {
        Matrix4x4 projection = Camera.projectionMatrix;
        projection *= Matrix4x4.Scale(new Vector3(1, -1, 1));
        Camera.projectionMatrix = projection;
    }
}