using UnityEngine;
using UnityEngine.UI;

public class MainModeManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public GameObject roomIdInputFieldObj;
    [SerializeField] public Button goButton;
    [SerializeField] public InputField roomIdInputField;

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

        goButton.interactable = false;

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
        GM.GameModeSelect(GameMode.Solo);
        PrintCurrentGameState();
        UpdateRoomIdUI();
    }

    public void OnClickMulti()
    {
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
        goButton.interactable = input.Length > 0;
    }

    // =====================
    // マルチ開始
    // =====================
    public void StartMulti(string roomId, string playerName)
    {
        CurrentRoomId = roomId;
        MultiPlayerName = playerName;

        multiSync.BeginMultiSync();
        matchState.SetMyPlayer(playerName);
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

}
