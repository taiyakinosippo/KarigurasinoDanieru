using TMPro;
using UnityEngine;
using System;

public class UI_Manager : MonoBehaviour
{
   public static UI_Manager instance;
   [SerializeField] private ScoreController scoreController; //スコアのプレゼンテーションを管理している
   [SerializeField] private TextMeshProUGUI scoreText;                 //スコアのテキスト
    [SerializeField] private TextMeshProUGUI enemyScoreText;
    private float progress = 0f;
    public static Action OnCountFinished;

    private float displayMyScore = 0f;
    private float displayEnemyScore = 0f;

     private MatchState matchState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    void Start()
    {
        if (GameManager.instance.currentMode == GameMode.Multi)
        {
            matchState = FindObjectOfType<MatchState>();
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
        //シングル
        if (GameManager.instance.currentMode != GameMode.Multi)
        {
            displayMyScore = Mathf.Lerp(displayMyScore, score, Time.deltaTime * 5f);
            scoreText.text = displayMyScore.ToString("N2") + "m";
            Debug.Log($"score={score}");
            return;
        }

        // マルチ
        if (matchState == null)
        {
            matchState = FindObjectOfType<MatchState>();
            return;
        }

        // ✅ 自分はscoreベース
        displayMyScore = Mathf.Lerp(displayMyScore, score, Time.deltaTime * 5f);

        // ✅ 敵はMatchState
        displayEnemyScore = Mathf.Lerp(displayEnemyScore, matchState.EnemyScore, Time.deltaTime * 5f);

        if (scoreText != null)
            scoreText.text = displayMyScore.ToString("N2") + "m";

        if (enemyScoreText != null)
            enemyScoreText.text = displayEnemyScore.ToString("N2") + "m";

        //Debug.Log($"[SELF SCORE] score={score}");
        Debug.Log($"[MATCH STATE] My={matchState?.MyScore}, Enemy={matchState?.EnemyScore}");

    }

    // ========================================
    // 動きが終了したときのテキストの更新
    // ========================================


    private void FinishText()
    {
        progress = 1f;
        if (GameManager.instance.currentMode != GameMode.Multi)
        {
            scoreText.text = ScoreManager.instance
                .GetScore()
                .ToString("N2") + "m";

            OnCountFinished?.Invoke();
            return;
        }

        if (matchState == null)
        {
            matchState = FindObjectOfType<MatchState>();
            return;
        }

        if (matchState.MyScore <= 0 || matchState.EnemyScore <= 0)
        {
            Debug.Log("[WAIT] スコア未確定");
            return;
        }


        displayMyScore = ScoreManager.instance.GetScore();
        displayEnemyScore = matchState.EnemyScore;


        if (scoreText != null)
            scoreText.text = displayMyScore.ToString("N2") + "m";

        if (enemyScoreText != null)
            enemyScoreText.text = displayEnemyScore.ToString("N2") + "m";

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
        if (scoreController == null)
        {
            scoreController = FindAnyObjectByType<ScoreController>();
        }

        scoreText = GameObject.Find("ScoreText")
            ?.GetComponent<TextMeshProUGUI>();

        enemyScoreText = GameObject.Find("EnemyScoreText")
            ?.GetComponent<TextMeshProUGUI>();
    }
}



