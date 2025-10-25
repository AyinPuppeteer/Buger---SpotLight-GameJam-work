using System.Collections;
using UnityEngine;

public class ScanWaveCollision : MonoBehaviour
{
     private ParticleSystem scanParticleSystem;
    
    private string playerTag = "Player";

    //安全超时（秒），防止协程永远等待）
    private float safetyTimeout = 5f;
    //判定稳定状态所需连续帧数
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
        // 仅对玩家有效
        if (!other.CompareTag(playerTag)) return;
        if (isIgnoring) return;

        NotifyAllGuardsToChase();

        if (ignoreCoroutine != null) StopCoroutine(ignoreCoroutine);
        ignoreCoroutine = StartCoroutine(DisableCollisionUntilNextWave());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 仅对玩家有效
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
            // 后备：短等待避免永久禁用
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            float timeout = Mathf.Max(0.1f, safetyTimeout);
            int stable = 0;

            // 等待当前波消亡（particleCount -> 0）
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

            // 等待下一波开始（particleCount 连续 >0）
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
            //Debug.Log("触发追逐");
        }
    }
}
