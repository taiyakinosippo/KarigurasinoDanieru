using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;
    
    // ソロ用
    [SerializeField] private ScoreController soloScoreController;
    [SerializeField] private TextMeshProUGUI soloScoreText;
    
    // マルチ用
    [SerializeField] private ScoreController multiScoreController;
    [SerializeField] private TextMeshProUGUI multiScoreText;
    [SerializeField] private ScoreSender scoreSender;
   
    // 汎用（main版から）
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private MatchState matchState;
    private float progress = 0f;
    
    // イベント
    public static Action OnSoloCountFinished;
    public static Action OnMultiScoreFinished;
    public static Action OnCountFinished;
    
    // 表示用変数
    private float displayMyScore = 0f;
    private float displayEnemyScore = 0f;

    private float targetEnemyScore = 0f;
    private bool isMyFinished = false;
    private bool isEnemyFinished = false;

    public static bool IsScoreReady = false;

    private bool isStartText = false;

    private float soloFinalScore = 0f;
    private float multiFinalScore = 0f;
    private string soloText;
    private string multiText;

    // UI遅延閉鎖管理
    private readonly Dictionary<Canvas, int> _pendingCloseRequests = new Dictionary<Canvas, int>();


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


        if (multiScoreText != null)
        {
            multiScoreText.text = "0.00m";
        }


    }

    IEnumerator Start()
    {
        IsScoreReady = false;

        yield return new WaitForSeconds(0.2f); 

        scoreSender = FindObjectOfType<ScoreSender>();

        scoreSender.OnEnemyScoreChanged += ShowEnemyScore;
    }

    private void Update()
    {
        //if (GameManager.instance.currentMode != GameMode.Multi) return;

        //if (multiScoreText == null || multiScoreText.Equals(null))
        //    return;

        //CheckStart(displayMyScore, targetEnemyScore);

        //if (!isStartText) return;

        //displayEnemyScore = Mathf.Lerp(displayEnemyScore, targetEnemyScore, Time.deltaTime * 1f);

        //multiScoreText.text = displayEnemyScore.ToString("N2") + "m";
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ソロの場合のスコアの更新
    public void StartSoloScoreEvent()
    {
        soloScoreController.OnScoreChanged += UpdateSoloScoreText;
        soloScoreController.OnFinished += FinishSoloText;
    }

    // マルチの場合のスコアの更新
    public void StartMultiScoreEvent()
    {
        multiScoreController.OnScoreChanged += UpdateMultiScoreText;
        multiScoreController.OnFinished += FinishMultiText;
    }

    // ========================================
    // ソロのスコアのテキストの更新
    // ========================================
    private void UpdateSoloScoreText(float score)
    {
       //マルチ処理
        if (GameManager.instance.currentMode == GameMode.Multi)
        {
         
            float enemy = targetEnemyScore;

            //同時スタートチェック
            if (!CheckStart(score, targetEnemyScore)) return;

            //自分のスコア更新
            displayMyScore = score;

            if (soloScoreText != null)
            {
                soloScoreText.text = displayMyScore.ToString("N2") + "m";
            }

            isMyFinished = true;
        }
        else
        {
            //ソロ処理
            displayMyScore = score;
            soloScoreText.text = displayMyScore.ToString("N2") + "m";
            Debug.Log($"score={score}");
        }

    }

    // ========================================
    // マルチのスコアのテキストの更新
    // ========================================
    private void UpdateMultiScoreText(float score)
    {
        if (GameManager.instance.currentMode != GameMode.Multi) return;
        if (scoreSender == null) return;
        //同時スタートチェック
        if (!CheckStart(score, targetEnemyScore)) return;

        targetEnemyScore = score;
    
        multiScoreText.text = displayEnemyScore.ToString("N2") + "m";
        Debug.Log($"aaa:{targetEnemyScore},{score}");
    }

    // ========================================
    // マルチのスコアのテキストの更新(敵）
    // ========================================
    private void UpdateEnemyMultiScoreText(float enemyScore)
    {
        if (GameManager.instance.currentMode != GameMode.Multi) return;

        float myScore = displayMyScore;

        //同時スタートチェック
        if (!CheckStart(myScore, enemyScore)) return;

        
            targetEnemyScore = enemyScore;

            if (displayEnemyScore < targetEnemyScore)
            {
                float diff = targetEnemyScore - displayEnemyScore;
                float speed = diff * 0.5f;

                displayEnemyScore += speed * Time.deltaTime;

                if (displayEnemyScore >= targetEnemyScore)
                {
                    displayEnemyScore = targetEnemyScore;
                    isEnemyFinished = true;
                    Debug.Log("敵表示更新: " + displayEnemyScore);
                }
            }

        

        if (multiScoreText != null)
        {
            Debug.Log("❌ multiScoreText NULL");

        }

        else
        {
            multiScoreText.text = displayEnemyScore.ToString("N2") + "m";
            Debug.Log("✅ multiScoreText OK");
        }

    }

    // ========================================
    // 汎用スコアのテキストの更新（main版）
    // ========================================
    private void UpdateScoreText(float score)
    {
        scoreText.text = score.ToString("N2") + "m";
    }

    private bool CheckStart(float myScore, float enemyScore)
    {
        if (isStartText) return true;

        if (myScore > 0 && enemyScore > 0)
        {
            isStartText = true;
            IsScoreReady = true;

            displayMyScore = 0f;
            displayEnemyScore = 0f;

            Debug.Log("🚀 同時スタート！");
            return true;
        }

        return false;
    }

    private void ShowEnemyScore(float score)
    {
        if (score <= 0) return;

        targetEnemyScore = score;

        Debug.Log($"targetEnemyScore{targetEnemyScore}");
      
    }
  

    // ========================================
    // 終了時のテキストの更新
    // ========================================

    private void FinishSoloText()
    {
        float finalScore = ScoreManager.instance.SoloResultScore();
       
        Debug.Log(finalScore + "が表示されました");
       
        soloText = finalScore.ToString("N2") + "m";
        soloScoreText.text = soloText;
        Debug.Log("ソロスコアのプレゼンテーションが終了しました。");
        OnSoloCountFinished?.Invoke();
    }

    private void FinishMultiText()
    {
        float finalScore = ScoreManager.instance.MultiResultScore();
        multiText = finalScore.ToString("N2") + "m";
        multiScoreText.text = multiText;
        Debug.Log("マルチテキスト表示");
        OnMultiScoreFinished?.Invoke();
    }

    private void FinishText()
    {
        Debug.Log("スコアのプレゼンテーションが終了しました。");
        scoreText.text = ScoreManager.instance
            .SoloResultScore()
            .ToString("N2") + "m";
        OnCountFinished?.Invoke();
    }

    /// <summary>
    /// UIを表示する
    /// </summary>
    public void ShowUI(Canvas target)
    {
        _pendingCloseRequests.Remove(target);
        target.enabled = true;
    }

    /// <summary>
    /// UIを非表示にする
    /// </summary>
    public void CloseUI(Canvas target)
    {
        _pendingCloseRequests.Remove(target);
        target.enabled = false;
    }

    public void ScheduleCloseUI(Canvas target, float delay)
    {
        int requestId = 1;
        if (_pendingCloseRequests.TryGetValue(target, out int existingId))
        {
            requestId = existingId + 1;
        }

        _pendingCloseRequests[target] = requestId;
        StartCoroutine(CloseUIAfterDelay(target, delay, requestId));
    }

    private IEnumerator CloseUIAfterDelay(Canvas target, float delay, int requestId)
    {
        yield return new WaitForSeconds(delay);

        if (_pendingCloseRequests.TryGetValue(target, out int currentId) && currentId == requestId)
        {
            _pendingCloseRequests.Remove(target);
            target.enabled = false;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UIManagerGetComponents();
    }

    public void UIManagerGetComponents()
    {
        // ソロ用
        if (soloScoreController == null)
        {
            soloScoreController = GameObject.Find("SoloScoreController")
                ?.GetComponent<ScoreController>();
        }

        // マルチ用
        if (GameManager.instance.currentMode == GameMode.Multi)
        {
            multiScoreController = GameObject.Find("MultiScoreController")
                ?.GetComponent<ScoreController>();
        }

        // テキスト取得
        soloScoreText = GameObject.Find("ScoreText")
            ?.GetComponent<TextMeshProUGUI>();

        multiScoreText = GameObject.Find("EnemyScoreText")
            ?.GetComponent<TextMeshProUGUI>();

       

        // 汎用テキスト（main版）
        if (scoreText == null)
        {
            scoreText = GameObject.Find("ScoreText")
                ?.GetComponent<TextMeshProUGUI>();
        }

        if (multiScoreText == null)
        {
            multiScoreText = GameObject.Find("EnemyScoreText")
                ?.GetComponent<TextMeshProUGUI>();
        }
    }
}
