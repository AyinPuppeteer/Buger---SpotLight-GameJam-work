using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//�ؿ�����ѡ�أ��Ľű�
public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private TMPPrinter Text;

    [SerializeField]
    private Button ContinueButton;//���������İ�ť

    private Animator Anim;

    private LevelPack LevelNow;//��ǰѡ��Ĺؿ�
    public LevelPack LevelNow_ { get => LevelNow; }

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Anim = GetComponent<Animator>();

        ContinueButton.enabled = false;
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
            "���������" + (GameSave.Instance.CheckLevel(LevelNow.ID) ? "���ʹ�" : "δ�ʹ�") + "\n" +
            "�ռ������" + GameSave.Instance.WoveAtLevel(LevelNow.ID) + "/" + LevelNow.MaxWoves + "\n\n";
        Text.SetTextForce(text);
        Text.AddText(LevelNow.Description);

        ContinueButton.enabled = true;
    }

    //���ݹؿ���Ų������ð�
    private static LevelPack ReturnLevelPack(int t)
    {
        LevelPack pack = new();
        pack.ID = t;
        switch (t)
        {
            case 1:
                {
                    pack.Name = "����";
                    pack.Security = SecurityLevel.��;
                    pack.MaxWoves = 1;
                    pack.Description = "�������������ưܵ�¥����������ʹﶥ�ˣ�������Ψһ��Ŀ�ġ�";
                    break;
                }
            case 2:
                {
                    pack.Name = "��";
                    pack.Security = SecurityLevel.�е�;
                    pack.MaxWoves = 1;
                    pack.Description = "�������˫�ۣ��޾��ĳ���������ǰ���졣����ȱ�ģ�����������֮�����ֵ�������";
                    break;
                }
            case 3:
                {
                    pack.Name = "����";
                    pack.Security = SecurityLevel.�е�;
                    pack.MaxWoves = 2;
                    pack.Description = "³ç�������ܳ��¡��Ż�������������;�е�һ�У����ܷ������ص�����";
                    break;
                }
            case 4:
                {
                    pack.Name = "����";
                    pack.Security = SecurityLevel.��;
                    pack.MaxWoves = 1;
                    pack.Description = "�����ǡ�����Ұ�����ӵ�ÿ�����䣬�������ж��֮����";
                    break;
                }
            case 6:
                {
                    pack.Name = "����";
                    pack.Security = SecurityLevel.��;
                    pack.MaxWoves = 1;
                    pack.Description = "һζ�Ķ���޷�ӭ��ʤ����Ψ�С��������������л�ʤ�Ļ��ᡣ";
                    break;
                }
            case 7:
                {
                    pack.Name = "������";
                    pack.Security = SecurityLevel.�е�;
                    pack.MaxWoves = 1;
                    pack.Description = "����֮�ŵı�����ʤ�������������ֻ��ǵߵ���ת�����硣";
                    break;
                }
            case 8:
                {
                    pack.Name = "�µ�";
                    pack.Security = SecurityLevel.��;
                    pack.MaxWoves = 2;
                    pack.Description = "���ϵ�һ�ж�������ڵ����壬�κ��谭��Σ�ն�������ڵ����塣";
                    break;
                }
            case 9:
                {
                    pack.Name = "�е���ӭ";
                    pack.Security = SecurityLevel.��;
                    pack.MaxWoves = 1;
                    pack.Description = "������Ϊ��ĵ���������ȸԾ�����������������ǵġ����顱���Һã���Щ����ķ����ܹ������㡣";
                    break;
                }
            case 10:
                {
                    pack.Name = "��Ϸ��";
                    pack.Security = SecurityLevel.��;
                    pack.MaxWoves = 1;
                    pack.Description = "��ˣ����ܹ����ݵĳ��ԡ����ǡ������ַ�ʽ��";
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

    public LevelPack()
    {
        Name = "δ֪����";
        ID = -1;
        Security = SecurityLevel.�޷�����;
        MaxWoves = -1;
        Description = "��������δ֪�Ŀռ䣬����������Ϊ��������ա���������BUG������Ayin��������";
    }
}

public enum SecurityLevel
{
    ��, ��, �е�, ��, ����, �޷�����
}