using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ScanWave : MonoBehaviour
{
    // Start is called before the first frame update

    private ParticleSystem particleSystem;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.Burst[] bursts;

    // �������������Ա���ÿ֡Ƶ������
    private ParticleSystem.Particle[] particleCache = new ParticleSystem.Particle[0];

    private CircleCollider2D circleCollider;

    [SerializeField]
    [Header("����ϵͳ����")]
    [Tooltip("ÿ���ӷ������������")]
    [Range(0, 5000)]
    private int burstParticleCount = 50;

    [SerializeField]
    [Tooltip("Burst ������룩")]
    [Range(0f, 10f)]
    private float burstRepeatInterval = 1f;

    [SerializeField]
    [Tooltip("�����������ڣ��룬�̶�ֵ��")]
    [Range(0.1f, 10f)]
    private float lifetime = 2f;

    [SerializeField]
    [Tooltip("�����ٶȴ�С����λ/�룩")]
    [Range(0, 50f)]
    private float startSpeed = 5f;

    [SerializeField]
    [Tooltip("���ӳ�ʼ��ɫ")]
    private Color startColor = Color.white;

    private void Awake()
    {
        // ��ȡ����ϵͳ���
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null) return;

        mainModule = particleSystem.main;
        emissionModule = particleSystem.emission;

        int burstCount = Mathf.Max(0, emissionModule.burstCount);
        if (burstCount > 0)
        {
            bursts = new ParticleSystem.Burst[burstCount];
            emissionModule.GetBursts(bursts);
        }
        else
        {
            bursts = null;
        }

        //�Զ���ȡ CircleCollider2D
        if (circleCollider == null)
        {
            circleCollider = GetComponent<CircleCollider2D>();
        }
    }

    // ȷ��������ʱ��ʼʱ�Զ���������ϵͳ
    private void Start()
    {
        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
            if (particleSystem == null) return;
            mainModule = particleSystem.main;
            emissionModule = particleSystem.emission;
        }

        if (!particleSystem.isPlaying)
        {
            particleSystem.Play();
        }
    }

    void OnValidate()
    {
        // �༭ģʽ��ʵʱ����
        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        if (particleSystem != null)
        {
            mainModule = particleSystem.main;
            emissionModule = particleSystem.emission;

            // �ڱ༭����Ҳ��ʼ�� bursts �ֶ�
            int burstCount = Mathf.Max(0, emissionModule.burstCount);
            if (burstCount > 0)
            {
                bursts = new ParticleSystem.Burst[burstCount];
                emissionModule.GetBursts(bursts);
            }
            else
            {
                bursts = null;
            }
        }

        if (circleCollider == null)
        {
            circleCollider = GetComponent<CircleCollider2D>();
        }

        UpdateParticleSettings();
    }
    void Update()
    {
        UpdateParticleSettings();
        UpdateColliderRadius();
    }
    private void UpdateParticleSettings()
    {
        if (particleSystem == null) return;

        // ȷ��������һ�� burst ��Ŀ����û���򴴽���
        if (bursts == null || bursts.Length == 0)
        {
            bursts = new ParticleSystem.Burst[1];
            // Ĭ�� time Ϊ 0
            bursts[0] = new ParticleSystem.Burst(
                0f,
                (short)Mathf.Clamp(burstParticleCount, short.MinValue, short.MaxValue),
                (short)Mathf.Clamp(burstParticleCount, short.MinValue, short.MaxValue),
                0,
                burstRepeatInterval
            );
            emissionModule.SetBursts(bursts);
        }
        else
        {
            // ���µ�һ�� burst ����������
            bursts[0] = new ParticleSystem.Burst(
                bursts[0].time,
                (short)Mathf.Clamp(burstParticleCount, short.MinValue, short.MaxValue),
                (short)Mathf.Clamp(burstParticleCount, short.MinValue, short.MaxValue),
                bursts[0].cycleCount,
                burstRepeatInterval
            );
            emissionModule.SetBursts(bursts);
        }

        // ���³�ʼ��������
        mainModule.startLifetime = lifetime;

        // ���³�ʼ�ٶ�
        mainModule.startSpeed = startSpeed;

        // ���³�ʼ��ɫ��ʹ�� MinMaxGradient ��ȷ�����ݣ�
        mainModule.startColor = new ParticleSystem.MinMaxGradient(startColor);
    }

    // ֱ��ͬ�� CircleCollider2D.radius Ϊ��ǰ������ӵ���󱾵ؾ���
    private void UpdateColliderRadius()
    {
        if (circleCollider == null || particleSystem == null) return;

        int alive = particleSystem.particleCount;
        float targetRadius = 0f;

        if (alive > 0)
        {
            // ȷ�������С�㹻
            if (particleCache == null || particleCache.Length < alive)
            {
                particleCache = new ParticleSystem.Particle[alive];
            }

            int got = particleSystem.GetParticles(particleCache);
            float maxDist = 0f;

            ParticleSystemSimulationSpace simSpace = mainModule.simulationSpace;
            for (int i = 0; i < got; i++)
            {
                Vector3 p = particleCache[i].position;

                // ������λ��ת��Ϊ����ڸ� transform �ı���λ��
                if (simSpace == ParticleSystemSimulationSpace.World)
                {
                    p = transform.InverseTransformPoint(p);
                }

                // 2D �뾶ֻ���� X/Y ƽ��
                float d = new Vector2(p.x, p.y).magnitude;
                if (d > maxDist) maxDist = d;
            }

            targetRadius = maxDist;
        }
        else
        {
            targetRadius = 0f;
        }

        // ͬ���뾶
        circleCollider.radius = targetRadius;
    }
}
