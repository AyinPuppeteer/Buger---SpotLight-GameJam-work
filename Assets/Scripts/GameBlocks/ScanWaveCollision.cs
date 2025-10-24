using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanWaveCollision : MonoBehaviour
{
    // ��������������ײʱ��֪ͨ����������ʼ׷��
    private void OnTriggerEnter2D(Collider2D other)
    {
        NotifyAllGuardsToChase();
    }

    // �����ʹ�÷Ǵ���ʽ��ײ���� isTrigger����Ҳ����ʹ������ص�
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
