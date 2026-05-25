using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UIGameModeManager uiModeManager;
    public BalanceBarController balanceBar;
    public PointAreaController pointArea;
    public Timing_Bar_Logic timingBar;
    public MashButton mashButton;

    public GameMode currentMode;
    public GameLevel currentLevel;

    private bool isGameOver = false;

    [Header("演出設定")]
    [SerializeField] private float presentationDelay = 2.5f; // ゲーム終了からロケット発射までの待機時間

    public void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            InitializeInGame();
        }
    }

    /// <summary>
    /// ゲームモード選択のメソッド。引数でソロかマルチかを受け取る。
    /// </summary>
    public void GameModeSelect(GameMode mode)
    {
        currentMode = mode;
    }


    /// <summary>
    /// ゲームレベル選択のメソッド。引数でノーマルかハードかを受け取る。
    /// </summary>
    public void GameLevelSelect(GameLevel level)
    {
        currentLevel = level;
    }

    private void InitializeInGame()
    {
        isGameOver = false;

        //インゲームシーンの各コンポーネントを取得
        uiModeManager = FindFirstObjectByType<UIGameModeManager>();
        balanceBar = FindFirstObjectByType<BalanceBarController>();
        pointArea = FindFirstObjectByType<PointAreaController>();
        timingBar = FindFirstObjectByType<Timing_Bar_Logic>();
        mashButton = FindFirstObjectByType<MashButton>();

        //ヒエラルキーからUIGameModeManagerを探す
        if (uiModeManager == null)
        {
            uiModeManager = FindFirstObjectByType<UIGameModeManager>();
        }

        if (uiModeManager != null)
        {
            //GameManagerが保持している現在のモードを渡して初期化
            uiModeManager.SetupScreen(currentMode);
        }

        if (timingBar != null)
        {
            timingBar.SetupTimingBar(currentLevel);
        }

        if (mashButton != null)
        {
            mashButton.SetupMashButton(currentLevel);
        }

        if (pointArea != null)
        {
            pointArea.SetupPointArea(currentLevel);
        }
    }

    public void OnTimerFinished()
    {
        if (isGameOver) return;
        FinishGame();
    }

    public void FinishGame()
    {
        isGameOver = true;

        if (uiModeManager != null)
        {
            uiModeManager.ShowResultLayout();
        }

        if (mashButton != null)
        {
            mashButton.StopMashButton();
        }

        if (timingBar != null)
        {
            timingBar.StopTimingBar();
        }

        if (balanceBar != null)
        {
            balanceBar.StopBar();

            ScoreManager.instance.BalanceBarScore(
                balanceBar.meter,
                balanceBar.baseScore,
                balanceBar.multiplier
                );
        }

        if (pointArea != null)
        {
            pointArea.StopPointArea();
        }

        // 待機処理を開始
        StartCoroutine(DelayScorePresentationRoutine());
    }

    // 待機してからロケットを発射するコルーチン
    private IEnumerator DelayScorePresentationRoutine()
    {
        // インスペクターで設定した秒数待機する
        yield return new WaitForSeconds(presentationDelay);

        // 待機完了後、ロケットを発射
        if (ScoreManager.instance != null)
        {
            float finalScore = ScoreManager.instance.GetScore();
            ScoreManager.instance.StartFinalScorePresentation();
        }
    }
}