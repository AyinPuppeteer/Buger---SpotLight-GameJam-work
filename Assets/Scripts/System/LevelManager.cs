using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//关卡管理（选关）的脚本
public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private TMPPrinter Text;

    [SerializeField]
    private Button ContinueButton;//“继续”的按钮

    private Animator Anim;

    private LevelPack LevelNow;//当前选择的关卡
    public LevelPack LevelNow_ { get => LevelNow; }

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Anim = GetComponent<Animator>();

        ContinueButton.enabled = false;
    }

    //打开选关界面
    public void SetActive(bool active)
    {
        Anim.SetInteger("Anim", active ? 1 : 0);
    }

    public void ChooseLevel(int t)
    {
        LevelNow = ReturnLevelPack(t);

        //修改关卡信息
        string text = "所选关卡：" + LevelNow.Name + "\n" +
            "关卡编号：" + LevelNow.ID + "\n" +
            "安保等级：" + LevelNow.Security + "\n" +
            "订单情况：" + (GameSave.Instance.CheckLevel(LevelNow.ID) ? "已送达" : "未送达") + "\n" +
            "收集情况：" + GameSave.Instance.WoveAtLevel(LevelNow.ID) + "/" + LevelNow.MaxWoves + "\n\n";
        Text.SetTextForce(text);
        Text.AddText(LevelNow.Description);

        ContinueButton.enabled = true;
    }

    //根据关卡编号查找配置包
    private static LevelPack ReturnLevelPack(int t)
    {
        LevelPack pack = new();
        pack.ID = t;
        switch (t)
        {
            case 1:
                {
                    pack.Name = "新生";
                    pack.Security = SecurityLevel.低;
                    pack.MaxWoves = 1;
                    pack.Description = "你来到了这座破败的楼房，将快递送达顶端，这是你唯一的目的。";
                    break;
                }
            case 2:
                {
                    pack.Name = "梯";
                    pack.Security = SecurityLevel.中等;
                    pack.MaxWoves = 1;
                    pack.Description = "当你紧闭双眼，无尽的长梯在你面前延伸。你所缺的，是向那无形之物伸手的勇气。";
                    break;
                }
            case 3:
                {
                    pack.Name = "悬崖";
                    pack.Security = SecurityLevel.中等;
                    pack.MaxWoves = 2;
                    pack.Description = "鲁莽往往不能成事。放缓步伐，享受旅途中的一切，才能发现隐藏的美。";
                    break;
                }
            case 4:
                {
                    pack.Name = "观者";
                    pack.Security = SecurityLevel.低;
                    pack.MaxWoves = 1;
                    pack.Description = "“它们”的视野已蔓延到每个角落，但你仍有躲藏之所。";
                    break;
                }
            case 6:
                {
                    pack.Name = "天网";
                    pack.Security = SecurityLevel.高;
                    pack.MaxWoves = 1;
                    pack.Description = "一味的躲避无法迎来胜利，唯有“主动出击”才有获胜的机会。";
                    break;
                }
            case 7:
                {
                    pack.Name = "传送门";
                    pack.Security = SecurityLevel.中等;
                    pack.MaxWoves = 1;
                    pack.Description = "深邃之门的背后，是胜利还是死亡，抑或是颠倒反转的世界。";
                    break;
                }
            case 8:
                {
                    pack.Name = "孤岛";
                    pack.Security = SecurityLevel.低;
                    pack.MaxWoves = 2;
                    pack.Description = "世上的一切都有其存在的意义，任何阻碍与危险都有其存在的意义。";
                    break;
                }
            case 9:
                {
                    pack.Name = "夹道欢迎";
                    pack.Security = SecurityLevel.高;
                    pack.MaxWoves = 1;
                    pack.Description = "保安们为你的到来“欢呼雀跃”，你难以忍受他们的“热情”。幸好，那些奇异的方块能够帮助你。";
                    break;
                }
            case 10:
                {
                    pack.Name = "游戏机";
                    pack.Security = SecurityLevel.低;
                    pack.MaxWoves = 1;
                    pack.Description = "借此，你能够短暂的尝试“它们”的娱乐方式。";
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

//储存关卡信息的类
public class LevelPack
{
    public string Name;//关卡名

    public int ID;//编号

    public SecurityLevel Security;

    public int MaxWoves;//最大织线数量

    public string Description;//描述文本

    public LevelPack()
    {
        Name = "未知区域";
        ID = -1;
        Security = SecurityLevel.无法评估;
        MaxWoves = -1;
        Description = "你落入了未知的空间，或许有人能为你清除烦恼。（这是真BUG，请找Ayin帮助修理）";
    }
}

public enum SecurityLevel
{
    无, 低, 中等, 高, 极高, 无法评估
}