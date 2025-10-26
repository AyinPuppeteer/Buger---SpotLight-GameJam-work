using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//�������л��Ľű�
public class FadeEvent : MonoBehaviour
{
    private Animator Anim;

    private string AimScene;//Ŀ�곡��

    public static FadeEvent Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Anim = GetComponent<Animator>();
    }

    //�����е��õ������л������ĺ���
    private void LoadScene()
    {
        if(AimScene != null)
        {
            SceneManager.LoadScene(AimScene);
            AimScene = null;
        }
    }

    //�л���ָ������
    public void Fadeto(string scene)
    {
        AimScene = scene;
        Anim.SetTrigger("Anim");
    }
   
    //����л���ֻ����Ч�����ı䳡����
    public void FakeFade()
    {
        Anim.SetTrigger("Anim");
    }
}