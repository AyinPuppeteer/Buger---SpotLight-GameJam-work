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

    // 缓存粒子数组以避免每帧频繁分配
    private ParticleSystem.Particle[] particleCache = new ParticleSystem.Particle[0];

    private CircleCollider2D circleCollider;

    [SerializeField]
    [Header("粒子系统控制")]
    [Tooltip("每秒钟发射的粒子数量")]
    [Range(0, 5000)]
    private int burstParticleCount = 50;

    [SerializeField]
    [Tooltip("Burst 间隔（秒）")]
    [Range(0f, 10f)]
    private float burstRepeatInterval = 1f;

    [SerializeField]
    [Tooltip("粒子生命周期（秒，固定值）")]
    [Range(0.1f, 10f)]
    private float lifetime = 2f;

    [SerializeField]
    [Tooltip("粒子速度大小（单位/秒）")]
    [Range(0, 50f)]
    private float startSpeed = 5f;

    [SerializeField]
    [Tooltip("粒子初始颜色")]
    private Color startColor = Color.white;

    private void Awake()
    {
        // 获取粒子系统组件
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

        //自动获取 CircleCollider2D
        if (circleCollider == null)
        {
            circleCollider = GetComponent<CircleCollider2D>();
        }
    }

    // 确保在运行时开始时自动播放粒子系统
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
        // 编辑模式下实时更新
        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        if (particleSystem != null)
        {
            mainModule = particleSystem.main;
            emissionModule = particleSystem.emission;

            // 在编辑器中也初始化 bursts 字段
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

        // 确保有至少一个 burst 条目（若没有则创建）
        if (bursts == null || bursts.Length == 0)
        {
            bursts = new ParticleSystem.Burst[1];
            // 默认 time 为 0
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
            // 更新第一个 burst 的数量与间隔
            bursts[0] = new ParticleSystem.Burst(
                bursts[0].time,
                (short)Mathf.Clamp(burstParticleCount, short.MinValue, short.MaxValue),
                (short)Mathf.Clamp(burstParticleCount, short.MinValue, short.MaxValue),
                bursts[0].cycleCount,
                burstRepeatInterval
            );
            emissionModule.SetBursts(bursts);
        }

        // 更新初始生命周期
        mainModule.startLifetime = lifetime;

        // 更新初始速度
        mainModule.startSpeed = startSpeed;

        // 更新初始颜色（使用 MinMaxGradient 以确保兼容）
        mainModule.startColor = new ParticleSystem.MinMaxGradient(startColor);
    }

    // 直接同步 CircleCollider2D.radius 为当前存活粒子的最大本地距离
    private void UpdateColliderRadius()
    {
        if (circleCollider == null || particleSystem == null) return;

        int alive = particleSystem.particleCount;
        float targetRadius = 0f;

        if (alive > 0)
        {
            // 确保缓存大小足够
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

                // 将粒子位置转换为相对于该 transform 的本地位置
                if (simSpace == ParticleSystemSimulationSpace.World)
                {
                    p = transform.InverseTransformPoint(p);
                }

                // 2D 半径只考虑 X/Y 平面
                float d = new Vector2(p.x, p.y).magnitude;
                if (d > maxDist) maxDist = d;
            }

            targetRadius = maxDist;
        }
        else
        {
            targetRadius = 0f;
        }

        // 同步半径
        circleCollider.radius = targetRadius;
    }
}
