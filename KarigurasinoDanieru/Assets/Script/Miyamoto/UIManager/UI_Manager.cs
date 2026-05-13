using TMPro;
using UnityEngine;
using System;

public class UI_Manager : MonoBehaviour
{
   public static UI_Manager instance;
   [SerializeField] private ScoreManager scoreManager; //スコアの情報を保持している
   [SerializeField] BackGroundMover backGroundMover;  //背景を動かすスクリプト
    public static Action OnCountFinished;
    public TextMeshProUGUI score_text;                 //スコアのテキスト
    private bool isCounting = false;                   //スコアのカウントを開始するかどうか
    private void Awake()
   {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //スコアの更新
    public void Update()
    {
        if (!isCounting) return;
        float score = ScoreManager.instance.UpdatePresentationScore();
        score_text.text = score.ToString("N2") + "m";
        if (ScoreManager.instance.IsPresentationFinished())
        {
            score_text.text = ScoreManager.instance.GetScore().ToString("N2") + "m";
            backGroundMover.ScrollEnd();
            OnCountFinished?.Invoke();
            isCounting = false;
        }
    }
    //テキストのカウントを開始する
    public void StartCount()
    {
        isCounting = true;
    }
    //UIの表示
    public void ShowUI(Canvas ui)
    {
        ui.enabled = true;
    }
    //UIの非表示
    public void CloseUI(Canvas ui)
    {
        ui.enabled = false;
    }
}

