using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ModeManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject gamePlay_Single;
    [SerializeField] private GameObject gamePlay_Multi;
    [SerializeField] private GameObject playMode;
    [SerializeField] private GameObject matching;

    [Header("Multi UI")]
    [SerializeField] private InputField multiRoomInput;
    [SerializeField] private InputField multiPlayerNameInput;
    [SerializeField] private InputField scoreInputField;
    [SerializeField] private Button matchingButton;
    [SerializeField] private GameObject matchingButtonObj;
    [SerializeField] private GameObject multiRoomInputField;
    [SerializeField] private GameObject multiNameInputField;

    [Header("Text")]
    [SerializeField] private Text matchStatusText;
    [SerializeField] private Text modeText;
    [SerializeField] private Text resultPlayerText;
    [SerializeField] private Text resultEnemyText;
    [SerializeField] private Text resultStatsText;

    [Header("Managers")]
    [SerializeField] private MultiSyncManager multiSync;
    [SerializeField] private MatchState matchState;

    public static string CurrentRoomId;
    public static string MultiPlayerName;
    public static bool IsMultiMode;

    private bool matchHandled = false;

    void Start()
    {
        IsMultiMode = false;
        CurrentRoomId = "";
        MultiPlayerName = "";
        matchHandled = false;

        gamePlay_Single.SetActive(false);
        gamePlay_Multi.SetActive(false);
        matching.SetActive(false);
        playMode.SetActive(true);

        multiRoomInput.text = "";
        multiPlayerNameInput.text = "";
        scoreInputField.text = "";

        HideResultTexts();

        matchStatusText.gameObject.SetActive(false);
        matchingButton.interactable = false;

        multiRoomInput.onValueChanged.AddListener(OnRoomIdChanged);

        if (multiSync != null)
            multiSync.OnScoreSent += OnMultiScoreSent;

        UpdateModeText();
    }

    void Update()
    {
        if (IsMultiMode)
        {
            UpdateResultUI();
        }
    }

    void OnDestroy()
    {
        if (multiSync != null)
            multiSync.OnScoreSent -= OnMultiScoreSent;
    }

    void OnRoomIdChanged(string input)
    {
        matchingButton.interactable = !string.IsNullOrWhiteSpace(input);
    }

    // =====================
    // SINGLE MODE
    // =====================
    public void InputSingle()
    {
        IsMultiMode = false;
        matchHandled = false;

        playMode.SetActive(false);
        matching.SetActive(false);
        gamePlay_Multi.SetActive(false);
        gamePlay_Single.SetActive(true);

        multiRoomInput.text = "";
        multiPlayerNameInput.text = "";
        scoreInputField.text = "";

        UpdateModeText();
    }

    // =====================
    // MULTI MODE
    // =====================
    public void InputMulti()
    {
        IsMultiMode = true;
        matchHandled = false;

        playMode.SetActive(false);
        matching.SetActive(true);
        gamePlay_Single.SetActive(false);
        gamePlay_Multi.SetActive(false);

        matchingButtonObj.SetActive(true);
        multiRoomInputField.SetActive(true);
        multiNameInputField.SetActive(true);

        multiRoomInput.text = "";
        multiPlayerNameInput.text = "";
        scoreInputField.text = "";

        HideResultTexts();

        matchStatusText.gameObject.SetActive(true);
        matchStatusText.text = "Input Name & Room ID";

        UpdateModeText();
    }

    public void InputMatching()
    {
        CurrentRoomId = multiRoomInput.text.Trim();
        MultiPlayerName = multiPlayerNameInput.text.Trim();

        if (string.IsNullOrEmpty(CurrentRoomId) || string.IsNullOrEmpty(MultiPlayerName))
            return;

      
        UpdateModeText();

        multiRoomInputField.SetActive(false);
        multiNameInputField.SetActive(false);
        matchingButtonObj.SetActive(false);

        matchStatusText.text = "Waiting...";

        multiSync.BeginMultiSync();
    }


    // =====================
    // MATCH SUCCESS
    // =====================
    public void OnMatchSuccess(string opponentName)
    {
        if (matchHandled) return;
        matchHandled = true;

        StartCoroutine(MatchSuccessSequence(opponentName));
    }

    private IEnumerator MatchSuccessSequence(string opponentName)
    {
        matchStatusText.text = $"{opponentName} Matching!";
        yield return new WaitForSeconds(2f);

        matchStatusText.gameObject.SetActive(false);
        matching.SetActive(false);
        gamePlay_Multi.SetActive(true);

        UpdateModeText();
    }

    // =====================
    // MULTI RESULT (送信時表示)
    // =====================
    private void OnMultiScoreSent()
    {
        int myScore = 0;
        if (scoreInputField != null)
            int.TryParse(scoreInputField.text, out myScore);

        matchState.SetMyPlayer(MultiPlayerName);
        matchState.SetMyScore(myScore);

        // 通信用スコア
        multiSync.currentScore = myScore;

        UpdateResultUI();
    }


    private void HideResultTexts()
    {
        resultPlayerText.gameObject.SetActive(false);
        resultEnemyText.gameObject.SetActive(false);
    }

    // =====================
    // BACK
    // =====================
    public void InputBack()
    {
        IsMultiMode = false;
        CurrentRoomId = "";
        MultiPlayerName = "";
        matchHandled = false;

        playMode.SetActive(true);
        matching.SetActive(false);
        gamePlay_Single.SetActive(false);
        gamePlay_Multi.SetActive(false);

        HideResultTexts();
        UpdateModeText();

        if (multiSync != null)
            multiSync.StopMultiSync();

    }


    private void UpdateModeText()
    {
        modeText.text =
            $"MODE: {(IsMultiMode ? "MULTI" : "SINGLE")}\n" +
            $"ROOM: {(string.IsNullOrEmpty(CurrentRoomId) ? "-" : CurrentRoomId)}\n" +
            $"NAME: {(IsMultiMode ? MultiPlayerName : "-")}";
    }

    public void OnEnemyScoreUpdated(string enemyName, int enemyScore)
    {
        if (!IsMultiMode) return;

        // MatchState を更新
        matchState.SetEnemy(enemyName, enemyScore);

        UpdateResultUI();
    }

    // =====================
    // UI UPDATE
    // =====================
    private void UpdateResultUI()
    {
        resultPlayerText.text =
            $"{matchState.MyName}\nScore : {matchState.MyScore}";

        resultEnemyText.text =
            string.IsNullOrEmpty(matchState.EnemyName)
                ? "Waiting...\nScore : -"
                : $"{matchState.EnemyName}\nScore : {matchState.EnemyScore}";

        UpdateLeadStatusText();

        resultPlayerText.gameObject.SetActive(true);
        resultEnemyText.gameObject.SetActive(true);
    }

    public void OnEnemyLeft()
    {
        if (!IsMultiMode) return;

        matchStatusText.gameObject.SetActive(true);
        matchStatusText.text = "相手が退出しました";

        resultEnemyText.text = "Enemy Left";
    }

    private void UpdateLeadStatusText()
    {
        if (string.IsNullOrEmpty(matchState.EnemyName))
        {
            resultStatsText.text = "Waiting for opponent...";
            return;
        }

        if (matchState.MyScore > matchState.EnemyScore)
        {
            resultStatsText.text = "YOU ARE WINNING!";
        }
        else if (matchState.MyScore < matchState.EnemyScore)
        {
            resultStatsText.text = "ENEMY IS WINNING!";
        }
        else
        {
            resultStatsText.text = "TIE";
        }
    }
}