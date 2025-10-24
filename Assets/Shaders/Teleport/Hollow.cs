using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Hollow : MonoBehaviour
{
    public Material MatRefer;//材质原本
    private Material material;
    public ParticleSystem ps;
    public SpriteRenderer Image;


    private ParticleSystem.Particle[] particles;//粒子数组
    private Vector4[] pos;//粒子位置数组
    private float[] sizes;//粒子大小数组
    private float[] initsizes;//粒子初始大小数组
    public int maxPartAmount = 20;//最大粒子数

    private void Awake()
    {
        material = new(MatRefer);//克隆材质
        Image.material = material;
    }

    private void Update()
    {
        //粒子系统的参数设置
        var psmain = ps.main;
        psmain.maxParticles = maxPartAmount;
        particles = new Particle[maxPartAmount];

        ps.GetParticles(particles);
        sizes = new float[maxPartAmount];
        pos = new Vector4[maxPartAmount];
        initsizes = new float[maxPartAmount];

        for (int i = 0; i < maxPartAmount; i++)
        {
            pos[i] = ps.transform.position + particles[i].position * transform.localScale.x;
            sizes[i] = particles[i].GetCurrentSize(ps) * transform.localScale.x;
            initsizes[i] = particles[i].startSize;
        }

        material.SetFloatArray("ParticleSize", sizes);
        material.SetFloatArray("ParticleInitSize", initsizes);
        material.SetVectorArray("ParticlePos", pos);
    }
}
