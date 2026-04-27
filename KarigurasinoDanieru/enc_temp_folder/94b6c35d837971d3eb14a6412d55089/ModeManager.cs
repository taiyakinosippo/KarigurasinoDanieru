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

    [Header("Managers")]
    [SerializeField] private MultiSyncManager multiSync;
    [SerializeField] private MatchScoreManager MatchscoreManager; // 現在スコア管理

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

        HideResultTexts();

        matchStatusText.gameObject.SetActive(false);
        matchingButton.interactable = false;

        multiRoomInput.onValueChanged.AddListener(OnRoomIdChanged);

        if (multiSync != null)
            multiSync.OnScoreSent += OnMultiScoreSent;

        UpdateModeText();
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
        // 🔴 InputField → int に変換
        int myScore = 0;
        if (scoreInputField != null)
        {
            int.TryParse(scoreInputField.text, out myScore);
        }

        // 正本にも反映（推奨）
        MatchscoreManager.SetScore(myScore);

        // MultiSync にも反映（送信側）
        multiSync.currentScore = myScore;

        string myName = MultiPlayerName;
        string enemyName = multiSync.opponentName;
        int enemyScore = multiSync.opponentScore;

        ShowMultiResult(myName, myScore, enemyName, enemyScore);
    }

    private void ShowMultiResult(string myName, int myScore, string enemyName, int enemyScore)
    {
        resultPlayerText.text =
            $"{myName}\nScore : {myScore}";

        resultEnemyText.text =
            string.IsNullOrEmpty(enemyName)
                ? "Waiting...\nScore : -"
                : $"{enemyName}\nScore : {enemyScore}";

        resultPlayerText.gameObject.SetActive(true);
        resultEnemyText.gameObject.SetActive(true);
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

        if (multiSync != null)
            multiSync.StopMultiSync();

        playMode.SetActive(true);
        matching.SetActive(false);
        gamePlay_Single.SetActive(false);
        gamePlay_Multi.SetActive(false);

        HideResultTexts();
        UpdateModeText();
    }

    private void UpdateModeText()
    {
        modeText.text =
            $"MODE: {(IsMultiMode ? "MULTI" : "SINGLE")}\n" +
            $"ROOM: {(string.IsNullOrEmpty(CurrentRoomId) ? "-" : CurrentRoomId)}\n" +
            $"NAME: {(IsMultiMode ? MultiPlayerName : "-")}";
    }
}