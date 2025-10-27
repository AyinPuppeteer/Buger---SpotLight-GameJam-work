using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//关卡管理（选关）的脚本
public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private TMPPrinter Text;

    private Animator Anim;

    private LevelPack LevelNow;//当前选择的关卡
    public LevelPack LevelNow_ { get => LevelNow; }

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Anim = GetComponent<Animator>();
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
            "订单情况：" + GameSave.Instance.LevelFinish(LevelNow.ID) + "\n" +
            "收集情况：" + GameSave.Instance.WoveAtLevel(LevelNow.ID) + "/" + LevelNow.MaxWoves + "\n\n";
        Text.SetTextForce(text);
        Text.AddText(LevelNow.Description);
    }

    //根据关卡编号查找配置包
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

//储存关卡信息的类
public class LevelPack
{
    public string Name;//关卡名

    public int ID;//编号

    public SecurityLevel Security;

    public int MaxWoves;//最大织线数量

    public string Description;//描述文本
}

public enum SecurityLevel
{
    无, 低, 中等, 高, 极高, 无法评估
}