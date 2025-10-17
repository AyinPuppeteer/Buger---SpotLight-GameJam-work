using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Animator AlertAnim;

    public SavePoint SavePoint;//��¼�Ĵ浵�㣨��ʱ��

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //��ʼʱ�����ɽ�ɫ
        PlayerSpawner.Instance.SpawnAtPosition(SavePoint.Position);
    }

    public void SetSavePoint(SavePoint sp)
    {
        SavePoint = sp;
    }

    //����BUGʱ���ž��涯��
    public void BugAlert()
    {
        AlertAnim.Play("BUG����Ч��");
    }
}