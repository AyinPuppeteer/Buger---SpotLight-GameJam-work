using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于管理所有角色的脚本
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Guard[] ReturnAllEnemies()
    {
        return GetComponentsInChildren<Guard>();
    }
}