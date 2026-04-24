using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ModeManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject gamePlay;
    [SerializeField] private GameObject playMode;
    [SerializeField] private GameObject matching;

    [Header("Multi UI")]
    [SerializeField] private InputField multiRoomInput;
    [SerializeField] private InputField multiPlayerNameInput;
    [SerializeField] private Button matchingButton;
    [SerializeField] private GameObject matchingButtonObj;
    [SerializeField] private GameObject multiRoomInputField;
    [SerializeField] private GameObject gamePlayNameInput;

    [Header("Text")]
    [SerializeField] private Text matchStatusText;
    [SerializeField] private Text modeText;

    [Header("Managers")]
    [SerializeField] private MultiSyncManager multiSync;
    [SerializeField] private ScoreSender scoreSender;

    [Header("Runtime State")]
    public static string CurrentRoomId;
    public static string MultiPlayerName = "";
    public static bool IsMultiMode = false;

    private bool matchHandled = false;
    private int lastScore;

    void Start()
    {
        // 初期状態
        IsMultiMode = false;
        CurrentRoomId = "";
        MultiPlayerName = "";
        matchHandled = false;

        gamePlay.SetActive(false);
        matching.SetActive(false);
        playMode.SetActive(true);

        if (matchStatusText != null)
            matchStatusText.gameObject.SetActive(false);

        if (matchingButton != null)
            matchingButton.interactable = false;

        if (multiRoomInput != null)
        {
            multiRoomInput.onValueChanged.AddListener(OnRoomIdChanged);
            multiRoomInput.onEndEdit.AddListener(OnRoomIdChanged);
        }

        UpdateModeText();
    }

    void OnRoomIdChanged(string input)
    {
        if (matchingButton != null)
            matchingButton.interactable = !string.IsNullOrWhiteSpace(input);
    }

    // =====================
    // SINGLE MODE
    // =====================
    public void InputSingle()
    {
        Debug.Log("[MODE] SINGLE START");

        IsMultiMode = false;
        CurrentRoomId = "";
        MultiPlayerName = "";
        matchHandled = false;

        if (multiSync != null)
            multiSync.enabled = false;

        playMode.SetActive(false);
        matching.SetActive(false);
        gamePlay.SetActive(true);

        if (gamePlayNameInput != null)
            gamePlayNameInput.SetActive(true);

        UpdateModeText();
    }

    // =====================
    // MULTI MODE
    // =====================
    public void InputMulti()
    {
        Debug.Log("[MODE] MULTI SELECT");
        IsMultiMode = true;

        playMode.SetActive(false);
        matching.SetActive(true);
        gamePlay.SetActive(false);

        multiRoomInputField.SetActive(true);
        multiPlayerNameInput.gameObject.SetActive(true);
        matchingButtonObj.SetActive(true);

        if (matchStatusText != null)
        {
            matchStatusText.gameObject.SetActive(true);
            matchStatusText.text = "Input Name & Room ID";
        }

        if (gamePlayNameInput != null)
            gamePlayNameInput.SetActive(false);

        UpdateModeText();
    }

    public void InputMatching()
    {
        if (multiRoomInput == null || multiPlayerNameInput == null || multiSync == null)
        {
            Debug.LogError("[MATCH] Missing reference");
            return;
        }

        string roomId = multiRoomInput.text.Trim();
        string playerName = multiPlayerNameInput.text.Trim();

        if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("[MATCH] Name or RoomID empty");
            return;
        }

        CurrentRoomId = roomId;
        MultiPlayerName = playerName;

        Debug.Log($"[MATCH] name={MultiPlayerName}, room={CurrentRoomId}");

        multiRoomInputField.SetActive(false);
        multiPlayerNameInput.gameObject.SetActive(false);
        matchingButtonObj.SetActive(false);

        if (matchStatusText != null)
            matchStatusText.text = "Waiting...";

        UpdateModeText();

        // ✅ マッチング開始
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
        if (matchStatusText != null)
            matchStatusText.text = opponentName + " Matching!";

        yield return new WaitForSeconds(2f);

        if (matchStatusText != null)
            matchStatusText.gameObject.SetActive(false);

        matching.SetActive(false);
        gamePlay.SetActive(true);

        if (gamePlayNameInput != null)
            gamePlayNameInput.SetActive(false);
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
        gamePlay.SetActive(false);

        UpdateModeText();
    }

    // =====================
    // UI
    // =====================
    private void UpdateModeText()
    {
        if (modeText == null) return;

        modeText.text =
            $"MODE:{(IsMultiMode ? "MULTI" : "SINGLE")}\n" +
            $"ROOM:{(string.IsNullOrEmpty(CurrentRoomId) ? "-" : CurrentRoomId)}\n" +
            $"NAME:{(IsMultiMode ? MultiPlayerName : "-")}";
    }

    // =====================
    // GAME END
    // =====================
    public void OnGameEnd(int score)
    {
        lastScore = score;

        if (scoreSender == null)
        {
            Debug.LogError("[MODE] ScoreSender not found");
            return;
        }

        string mode = IsMultiMode ? "multi" : "normal";
        string name = IsMultiMode ? MultiPlayerName : "";

        Debug.Log($"[MODE] GameEnd score={score}, mode={mode}");

        scoreSender.SendScore(name, lastScore, mode);
    }
}
