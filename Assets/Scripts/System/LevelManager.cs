using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//�ؿ�����ѡ�أ��Ľű�
public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private TMPPrinter Text;

    private Animator Anim;

    private LevelPack LevelNow;//��ǰѡ��Ĺؿ�
    public LevelPack LevelNow_ { get => LevelNow; }

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Anim = GetComponent<Animator>();
    }

    //��ѡ�ؽ���
    public void SetActive(bool active)
    {
        Anim.SetInteger("Anim", active ? 1 : 0);
    }

    public void ChooseLevel(int t)
    {
        LevelNow = ReturnLevelPack(t);

        //�޸Ĺؿ���Ϣ
        string text = "��ѡ�ؿ���" + LevelNow.Name + "\n" +
            "�ؿ���ţ�" + LevelNow.ID + "\n" +
            "�����ȼ���" + LevelNow.Security + "\n" +
            "���������" + GameSave.Instance.LevelFinish(LevelNow.ID) + "\n" +
            "�ռ������" + GameSave.Instance.WoveAtLevel(LevelNow.ID) + "/" + LevelNow.MaxWoves + "\n\n";
        Text.SetTextForce(text);
        Text.AddText(LevelNow.Description);
    }

    //���ݹؿ���Ų������ð�
    private LevelPack ReturnLevelPack(int t)
    {
        LevelPack pack = new();
        pack.ID = t;
        switch (t)
        {
            case 1:
                {

                    break;
                }
        }
        return pack;
    }

    public void EnterGame()
    {
        GameManager.SetPack(LevelNow);
        FadeEvent.Instance.Fadeto("Level" + LevelNow.ID);
    }
}

//����ؿ���Ϣ����
public class LevelPack
{
    public string Name;//�ؿ���

    public int ID;//���

    public SecurityLevel Security;

    public int MaxWoves;//���֯������

    public string Description;//�����ı�
}

public enum SecurityLevel
{
    ��, ��, �е�, ��, ����, �޷�����
}