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
    }

    void Start()
    {
        if (GameManager.instance.currentMode == GameMode.Solo)
        {
            StartSoloScoreEvent();
        }
        else if (GameManager.instance.currentMode == GameMode.Multi)
        {
            StartMultiScoreEvent();
        }
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
        displayMyScore = Mathf.Lerp(displayMyScore, score, Time.deltaTime * 5f);
        soloScoreText.text = displayMyScore.ToString("N2") + "m";
        Debug.Log($"score={score}");
    }

    // ========================================
    // マルチのスコアのテキストの更新
    // ========================================
    private void UpdateMultiScoreText(float score)
    {
        displayEnemyScore = Mathf.Lerp(displayEnemyScore, score, Time.deltaTime * 5f);
        multiScoreText.text = displayEnemyScore.ToString("N2") + "m";
        Debug.Log($"score={score}");
    }

    // ========================================
    // 汎用スコアのテキストの更新（main版）
    // ========================================
    private void UpdateScoreText(float score)
    {
        scoreText.text = score.ToString("N2") + "m";
    }

    // ========================================
    // 終了時のテキストの更新
    // ========================================

    private void FinishSoloText()
    {
        soloScoreText.text = ScoreManager.instance
            .SoloResultScore()
            .ToString("N2") + "m";
        OnSoloCountFinished?.Invoke();
    }

    private void FinishMultiText()
    {
        multiScoreText.text = ScoreManager.instance
            .MultiResultScore()
            .ToString("N2") + "m";
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

        multiScoreText = GameObject.Find("MultiScoreText")
            ?.GetComponent<TextMeshProUGUI>();

        // 汎用テキスト（main版）
        if (scoreText == null)
        {
            scoreText = GameObject.Find("ScoreText")
                ?.GetComponent<TextMeshProUGUI>();
        }
    }
}
