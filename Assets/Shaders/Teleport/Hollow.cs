using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Hollow : MonoBehaviour
{
    public Material MatRefer;//����ԭ��
    private Material material;
    public ParticleSystem ps;
    public SpriteRenderer Image;


    private ParticleSystem.Particle[] particles;//��������
    private Vector4[] pos;//����λ������
    private float[] sizes;//���Ӵ�С����
    private float[] initsizes;//���ӳ�ʼ��С����
    public int maxPartAmount = 20;//���������

    private void Awake()
    {
        material = new(MatRefer);//��¡����
        Image.material = material;
    }

    private void Update()
    {
        //����ϵͳ�Ĳ�������
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
