using System.Collections;
using UnityEngine;

public class ScanWaveCollision : MonoBehaviour
{
     private ParticleSystem scanParticleSystem;
    
    private string playerTag = "Player";

    //��ȫ��ʱ���룩����ֹЭ����Զ�ȴ���
    private float safetyTimeout = 5f;
    //�ж��ȶ�״̬��������֡��
    private int stableFrames = 2;

    private Collider2D myCollider;
    private bool isIgnoring = false;
    private Coroutine ignoreCoroutine;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        if (scanParticleSystem == null)
            scanParticleSystem = GetComponent<ParticleSystem>()
                                 ?? GetComponentInParent<ParticleSystem>()
                                 ?? GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���������Ч
        if (!other.CompareTag(playerTag)) return;
        if (isIgnoring) return;

        NotifyAllGuardsToChase();

        if (ignoreCoroutine != null) StopCoroutine(ignoreCoroutine);
        ignoreCoroutine = StartCoroutine(DisableCollisionUntilNextWave());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���������Ч
        if (!collision.gameObject.CompareTag(playerTag)) return;
        if (isIgnoring) return;

        NotifyAllGuardsToChase();

        if (ignoreCoroutine != null) StopCoroutine(ignoreCoroutine);
        ignoreCoroutine = StartCoroutine(DisableCollisionUntilNextWave());
    }

    private IEnumerator DisableCollisionUntilNextWave()
    {
        isIgnoring = true;
        if (myCollider != null) myCollider.enabled = false;

        if (scanParticleSystem == null)
        {
            // �󱸣��̵ȴ��������ý���
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            float timeout = Mathf.Max(0.1f, safetyTimeout);
            int stable = 0;

            // �ȴ���ǰ��������particleCount -> 0��
            if (scanParticleSystem.particleCount > 0)
            {
                float start = Time.time;
                stable = 0;
                while (Time.time - start < timeout)
                {
                    if (scanParticleSystem.particleCount == 0) stable++; else stable = 0;
                    if (stable >= stableFrames) break;
                    yield return null;
                }
            }

            // �ȴ���һ����ʼ��particleCount ���� >0��
            float start2 = Time.time;
            stable = 0;
            while (Time.time - start2 < timeout)
            {
                if (scanParticleSystem.particleCount > 0) stable++; else stable = 0;
                if (stable >= stableFrames) break;
                yield return null;
            }
        }

        if (myCollider != null) myCollider.enabled = true;
        isIgnoring = false;
        ignoreCoroutine = null;
    }

    private void NotifyAllGuardsToChase()
    {
        var guards = FindObjectsOfType<Guard>();
        for (int i = 0; i < guards.Length; i++)
        {
            guards[i].SetActive(true);
            //Debug.Log("����׷��");
        }
    }
}
