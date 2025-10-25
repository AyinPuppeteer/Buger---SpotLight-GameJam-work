using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ScanWave : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Image;

    private const float RawRadius = 1.28f;//ԭʼ�뾶
    private float Radius;//ɨ��뾶
    public void SetRadius(float r) {  Radius = r; }

    private void Start()
    {
        transform.localScale = new(0, 0, 1);
        transform.DOScale(new Vector3(Radius / RawRadius, Radius / RawRadius, 1), 1f);
        Image.DOColor(new(1, 1, 1, 0), 0.6f).OnComplete(() => Destroy(gameObject)).SetDelay(0.5f);
    }

    private void OnTriggerStay2D(Collider2D another)
    {
        BaseMovement bm = another.GetComponent<BaseMovement>();
        if(bm != null)
        {
            if (bm.IsDetectable_ && !bm.IsExposed_)//�ܹ�������Ҳ�δ��¶������²Ž�����һ��
            {
                if (Vector2.Distance(another.transform.position, transform.position) > transform.localScale.x * RawRadius - 0.3f)//��Ŀ���ڱ�Ե���Ż���
                {
                    bm.SetExposed(true);
                }
            }
        }
    }
}