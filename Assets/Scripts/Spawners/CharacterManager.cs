using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ڹ������н�ɫ�Ľű�
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