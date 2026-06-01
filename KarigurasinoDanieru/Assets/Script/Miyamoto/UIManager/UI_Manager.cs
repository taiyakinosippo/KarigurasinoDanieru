using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;
    [SerializeField] private ScoreController soloScoreController;      //スコアのプレゼンテーションを管理している
    [SerializeField] private ScoreController multiScoreController;     //スコアのプレゼンテーションを管理している
    [SerializeField] private TextMeshProUGUI soloScoreText;            //スコアのテキスト
    [SerializeField] private TextMeshProUGUI multiScoreText;
    private MatchState matchState;                                     //マルチの状態を管理している
    private float progress = 0f;
    public static Action OnSoloCountFinished;
    public static Action OnMultiScoreFinished;

    private float displayMyScore = 0f;
    private float displayEnemyScore = 0f;

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
        if (GameManager.instance.currentMode == GameMode.Multi)
        {
            matchState = FindObjectOfType<MatchState>();
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

    //ソロの場合のスコアの更新
    public void StartSoloScoreEvent()
    {
        soloScoreController.OnScoreChanged +=
            UpdateSoloScoreText;

        soloScoreController.OnFinished +=
            FinishSoloText;
    }

    //マルチの場合のスコアの更新
    public void StartMultiScoreEvent()
    {
        multiScoreController.OnScoreChanged +=
            UpdateMultiScoreText;
        multiScoreController.OnFinished +=
            FinishMultiText;
    }


    // ========================================
    // ここではソロのスコアのテキストの更新を行う
    // ========================================
    private void UpdateSoloScoreText(float score)
    {
       
         displayMyScore = Mathf.Lerp(displayMyScore, score, Time.deltaTime * 5f);
         soloScoreText.text = displayMyScore.ToString("N2") + "m";
         Debug.Log($"score={score}");
    }

    // ========================================
    // ここではマルチのスコアのテキストの更新を行う
    // ========================================
    private void UpdateMultiScoreText(float score)
    {
        displayEnemyScore = Mathf.Lerp(displayEnemyScore, score, Time.deltaTime * 5f);
        multiScoreText.text = displayEnemyScore.ToString("N2") + "m";
        Debug.Log($"score={score}");
    }

    // ========================================
    // 動きが終了したときのテキストの更新
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (soloScoreController == null)
        {
            soloScoreController = GameObject.Find("SoloScoreController")?.GetComponent<ScoreController>();
        }
        if (GameManager.instance.currentMode == GameMode.Multi)
        {
            multiScoreController = GameObject.Find("MultiScoreController")?.GetComponent<ScoreController>();
        }
        soloScoreText = GameObject.Find("ScoreText")
            ?.GetComponent<TextMeshProUGUI>();

        multiScoreText = GameObject.Find("MultiScoreText")
            ?.GetComponent<TextMeshProUGUI>();
    }
  
}



