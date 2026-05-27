using TMPro;
using UnityEngine;
using System;

public class UI_Manager : MonoBehaviour
{
   public static UI_Manager instance;
   [SerializeField] private ScoreController scoreController; //スコアのプレゼンテーションを管理している
   [SerializeField] private TextMeshProUGUI scoreText;                 //スコアのテキスト
    public static Action OnCountFinished;
    private void Awake()
   {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //スコアの更新
    public void StartScoreEvent()
    {
        scoreController.OnScoreChanged +=
            UpdateScoreText;

        scoreController.OnFinished +=
            FinishText;
    }

    // ========================================
    // ここではスコアのテキストの更新を行う
    // ========================================

    private void UpdateScoreText(float score)
    {
        scoreText.text =
            score.ToString("N2")
            + "m";
    }

    // ========================================
    // 動きが終了したときのテキストの更新
    // ========================================

    private void FinishText()
    {
        Debug.Log("スコアのプレゼンテーションが終了しました。");
        scoreText.text = ScoreManager.instance
            .GetScore()
            .ToString("N2")
            + "m";
            OnCountFinished?.Invoke();
    }

    /// <summary>
    /// UIを表示する
    ///</summary>
    public void ShowUI(Canvas target)
    {
        target.enabled =true;
    }

    /// <summary>
    /// UIを非表示にする
    ///</summary>
    public void CloseUI(Canvas target)
    {
        target.enabled = false;
    }

    public void UIManagerGetComponents()
    {
        
        if(scoreController == null)
        {
            scoreController = FindAnyObjectByType<ScoreController>();
        }
        if(scoreText == null)
        {
            scoreText = scoreText = GameObject.Find("ScoreText") .GetComponent<TextMeshProUGUI>();
        }
    }
}

