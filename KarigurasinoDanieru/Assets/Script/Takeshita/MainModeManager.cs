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
    [SerializeField] private TMP_InputField playerNameField;
    [SerializeField] public GameObject NameInputFieldObj;
    [SerializeField] private TextMeshProUGUI matchingText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private string nextSceneName;
    [SerializeField] private string playerNameInput;
    [SerializeField] private Fade fade;

    [Header("Managers")]
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

        playerNameField.characterLimit = 10;

    }

    private void Awake()
    {
        roomIdInputField.onValueChanged.AddListener(_ => UpdateGoButtonState());


        if (playerNameField != null)
        {
            playerNameField.characterLimit = 10;
            playerNameField.contentType = TMP_InputField.ContentType.Standard;
            playerNameField.lineType = TMP_InputField.LineType.SingleLine;

            playerNameField.onEndEdit.AddListener(OnNameChanged);
        }

    }

    // =====================
    // モード選択（ボタン）
    // =====================
    public void OnClickSingle()
    {
        goButton.SetActive(true);
        roomIdInputFieldObj.SetActive(false);
        NameInputFieldObj.SetActive(true);
        GM.GameModeSelect(GameMode.Solo);
        UpdateGoButtonState();
        UpdateRoomIdUI();
    }

    public void OnClickMulti()
    {
        roomIdInputFieldObj.SetActive(true);
        NameInputFieldObj.SetActive(true);
        goButton.SetActive(false);
        GM.GameModeSelect(GameMode.Multi);
        UpdateGoButtonState();
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

    // =====================
    // マルチ開始
    // =====================
    public void StartMulti(string roomId, string playerName)
    {
        CurrentRoomId = roomId;
        MultiPlayerName = playerName;

        MatchState.Instance.SetMyPlayer(playerName);
        MultiSyncManager.Instance.BeginMultiSync();
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
            MultiSyncManager.Instance.StopMultiSync();
        }

        var sender = FindObjectOfType<ScoreSender>();
        if (sender != null)
        {
            sender.StopFetching();
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
            MultiSyncManager.Instance.SendScoreIfHigher(matchScoreManager.CurrentScore);
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
           MatchState.Instance.MyName,
            finalScore,
            GM.currentLevel.ToString().ToLower()
        );
    }


    void PrintCurrentGameState()
{
    //Debug.Log(
    //    $"[DEBUG] Mode: {GameManager.instance.currentMode}, " +
    //    $"Difficulty: {GameManager.instance.currentLevel}"
    //);
}

    private string FilterPlayerName(string input)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (char c in input)
        {
            // ✅ 空白・改行は除外
            if (char.IsWhiteSpace(c)) continue;

            // ✅ 全角英数字は除外
            if (
                (c >= 'Ａ' && c <= 'Ｚ') ||
                (c >= 'ａ' && c <= 'ｚ') ||
                (c >= '０' && c <= '９')
            )
            {
                continue;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }

    private void OnNameChanged(string input)
    {
        string filtered = FilterPlayerName(input);

        //filtered = filtered.ToUpper();

        if (playerNameField.text != filtered)
        {
            playerNameField.SetTextWithoutNotify(filtered);
        }
    }


    public void OnGoButtonPressed()
    {
        string name = playerNameField.text;

        if (string.IsNullOrEmpty(name))
        {

            int random = UnityEngine.Random.Range(1000, 9999);
            name = $"PLAYER_{random}";

            Debug.Log($"⚠ 名前未入力 → 自動生成: {name}");

        }

        MultiPlayerName = name;

        MatchState.Instance.SetMyPlayer(name);

        Debug.Log($"🔥 最終名前: {MatchState.Instance.MyName}");

        if (GM.currentMode == GameMode.Multi)
        {
            CurrentRoomId = roomIdInputField.text;

            UpdatePlayerInfoUI();

            StartCoroutine(StartMatchingRoutine());
        }
        else
        {
            StartCoroutine(LoadSingleRoutine());
        }
    }

    void UpdateGoButtonState()
    {
        bool hasName = playerNameField != null && playerNameField.text.Length > 0;

        if (GM.currentMode == GameMode.Multi)
        {
            bool hasRoomId = roomIdInputField != null && roomIdInputField.text.Length > 0;

            goButton.SetActive(hasRoomId);
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
        while (!MatchState.Instance.IsMatched)
        {
            yield return null;
        }

       // Debug.Log("[MATCH FOUND]");

        matchingText.text = $"VS {MatchState.Instance.EnemyName}";

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
      //  Debug.Log("[LOAD SCENE CALLED]");
        SceneManager.LoadScene(nextSceneName);
    }
}
