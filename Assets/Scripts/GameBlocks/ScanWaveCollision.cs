using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanWaveCollision : MonoBehaviour
{
    // 当触发器发生碰撞时，通知所有守卫开始追逐
    private void OnTriggerEnter2D(Collider2D other)
    {
        NotifyAllGuardsToChase();
    }

    // 如果你使用非触发式碰撞（非 isTrigger），也可以使用这个回调
    private void OnCollisionEnter2D(Collision2D collision)
    {
        NotifyAllGuardsToChase();
    }

    private void NotifyAllGuardsToChase()
    {
        Guard[] guards = FindObjectsOfType<Guard>();
        for (int i = 0; i < guards.Length; i++)
        {
            guards[i].SetActive(true);
        }
    }
}
