using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;

public class MainModeManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public GameObject roomIdInputFieldObj;
    [SerializeField] public GameObject goButton;
    [SerializeField] public InputField roomIdInputField;
    [SerializeField] private TextMeshProUGUI matchingText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private string nextSceneName;
    [SerializeField] private string playerNameInput;
    [SerializeField] private Fade fade;

    [Header("Managers")]
    [SerializeField] private MultiSyncManager multiSync;
    [SerializeField] private MatchState matchState;
    [SerializeField] private RankingInputManager rankingInputManager;
    [SerializeField] private MatchScoreManager matchScoreManager;

    public static string CurrentRoomId;
    public static string MultiPlayerName;

    public GameManager GM => GameManager.instance;

    private bool matchHandled = false;

    void Start()
    {
        matchHandled = false;

        roomIdInputField.characterLimit = 5;
        roomIdInputField.contentType = InputField.ContentType.IntegerNumber;

        goButton.SetActive(true);
        matchingText.gameObject.SetActive(false);
    }

    void Awake()
    {
        roomIdInputField.onValueChanged.AddListener(OnRoomIdChanged);
    }

    // =====================
    // モード選択（ボタン）
    // =====================
    public void OnClickSingle()
    {
        goButton.SetActive(true);
        roomIdInputFieldObj.SetActive(false);
        GM.GameModeSelect(GameMode.Solo);
        PrintCurrentGameState();
        UpdateRoomIdUI();
    }

    public void OnClickMulti()
    {
        roomIdInputFieldObj.SetActive(true);
        goButton.SetActive(false);
        GM.GameModeSelect(GameMode.Multi);
        PrintCurrentGameState();
        UpdateRoomIdUI();
    }

    public void OnClickNormal()
    {
        GM.GameLevelSelect(GameLevel.Normal);
        PrintCurrentGameState();
    }

    public void OnClickHard()
    {
        GM.GameLevelSelect(GameLevel.Hard);
        PrintCurrentGameState();
    }

    void UpdateRoomIdUI()
    {
        if (roomIdInputField == null) return;

        bool isMulti = GameManager.instance.currentMode == GameMode.Multi;

        roomIdInputFieldObj.SetActive(isMulti);
    }

    void OnRoomIdChanged(string input)
    {
        if (GM.currentMode == GameMode.Multi)
        {
            // ✅ 入力あればボタン表示
            goButton.SetActive(input.Length > 0);
        }
    }

    // =====================
    // マルチ開始
    // =====================
    public void StartMulti(string roomId, string playerName)
    {
        CurrentRoomId = roomId;
        MultiPlayerName = playerName;

        matchState.SetMyPlayer(playerName);
        matchState.EnablePersistence();
        multiSync.BeginMultiSync();
    }

    // =====================
    // マッチ成功
    // =====================
    public void OnMatchSuccess()
    {
        if (matchHandled) return;
        matchHandled = true;

        Debug.Log("[MATCH] Success");
    }

    // =====================
    // ゲーム終了
    // =====================
    public void EndGame()
    {
        Debug.Log("[GAME END]");

        if (ScoreHolder.instance != null && ScoreHolder.instance.FinalScore > 0)
        {
            SaveUnifiedScore();
        }

        if (GM.currentMode == GameMode.Multi)
        {
            multiSync.StopMultiSync();
        }

        matchHandled = false;
    }

    // =====================
    // スコア更新
    // =====================
    public void UpdateScore(int value)
    {
        if (GM.currentMode == GameMode.Multi)
        {
            matchScoreManager.SetScore(value);
            multiSync.SendScoreIfHigher(matchScoreManager.CurrentScore);
        }
        else
        {
            matchScoreManager.AddScore(value);
        }
    }

    // =====================
    // ランキング保存
    // =====================
    void SaveUnifiedScore()
    {
        int finalScore = ScoreHolder.instance.FinalScore;

        rankingInputManager.SendScore(
            matchState.MyName,
            finalScore,
            GM.currentLevel.ToString().ToLower()
        );
    }


    void PrintCurrentGameState()
{
    Debug.Log(
        $"[DEBUG] Mode: {GameManager.instance.currentMode}, " +
        $"Difficulty: {GameManager.instance.currentLevel}"
    );
}

    public void OnGoButtonPressed()
    {
        if (GM.currentMode == GameMode.Multi)
        {
            CurrentRoomId = roomIdInputField.text;

            if (string.IsNullOrEmpty(CurrentRoomId))
            {
                Debug.LogError("[ERROR] roomId empty");
                return;
            }

            MultiPlayerName = string.IsNullOrEmpty(playerNameInput)
                ? "PLAYER"
                : playerNameInput;

            MultiPlayerName += "_" + Random.Range(1000, 9999);
            UpdatePlayerInfoUI();

            // ✅ コールバック無しに変更
            StartCoroutine(StartMatchingRoutine());
        }
        else
        {
            StartCoroutine(LoadSingleRoutine());
        }
    }

    IEnumerator LoadSingleRoutine()
    {
        fade.FadeIn(1f);

        yield return new WaitForSeconds(1f);

        PlayerPrefs.SetInt("TransitionState", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(nextSceneName);
    }
    void StartMatchingAndWait()
    {
        // ✅ UI表示
        matchingText.gameObject.SetActive(true);
        matchingText.text = "Waiting Matching...";

        // ✅ マルチ開始
        StartMulti(CurrentRoomId, MultiPlayerName);

        // ✅ マッチ待ち
        StartCoroutine(WaitForMatchAndLoad(nextSceneName));
    }

    private IEnumerator WaitForMatchAndLoad(string sceneName)
    {
        while (!matchState.IsMatched)
        {
            yield return null;
        }

        Debug.Log("[MATCH FOUND]");

        matchingText.text = $"VS {matchState.EnemyName}";

        yield return new WaitForSeconds(0.5f);

        fade.FadeIn(1f);

        // ❌ ここをやめる
        // yield return new WaitForSeconds(1.2f);

        PlayerPrefs.SetInt("TransitionState", 1);
        PlayerPrefs.Save();

        // ✅ Invokeに変更（重要）
        Invoke(nameof(LoadNextScene), 1.2f);
    
}

    IEnumerator StartMatchingRoutine()
    {
        fade.FadeIn(1f); // ✅ 演出だけ

        yield return new WaitForSeconds(1f);

        StartMatchingAndWait();
    }


    void UpdatePlayerInfoUI()
    {
        if (infoText == null) return;

        infoText.text =
            $"RoomID: {CurrentRoomId}\n" +
            $"Name: {MultiPlayerName}";
    }

    void LoadNextScene()
    {
        Debug.Log("[LOAD SCENE CALLED]");
        SceneManager.LoadScene(nextSceneName);
    }
}
