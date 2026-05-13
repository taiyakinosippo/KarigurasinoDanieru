using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MultiSyncManager : MonoBehaviour
{
    [Header("Server URLs")]
    private string updateUrl;
    private string fetchUrl;
    private string joinUrl;
   
    [Header("Player Info")]
    public string playerName;
    public int currentScore;

    public System.Action OnScoreSent;

    // 内部状態
    private string roomId;
    private bool matched;
    private bool joined;
    private bool isFetching;
    private bool isSending;

    [SerializeField] private MatchState matchState;
    [SerializeField] private ModeManager modeManager;

    public string opponentName = "";
    public int opponentScore = 0;
    private int lastEnemyScore = -1;
    private int lastSentScore = 0;

    private bool enemyPreviouslyPresent = false;

    void Awake()
    {
        updateUrl = ServerConfig.BaseUrl + "mp_update.php";
        fetchUrl = ServerConfig.BaseUrl + "mp_fetch.php";
        joinUrl = ServerConfig.BaseUrl + "mp_join.php";
       
        modeManager = FindObjectOfType<ModeManager>();
    }

    void Start()
    {
        // 明示的に停止状態で開始
        StopAllSync();
    }

    /* ======================
       マルチ開始
    ====================== */
    public void BeginMultiSync()
    {
       
        enabled = true;

        roomId = ModeManager.CurrentRoomId;
        playerName = ModeManager.MultiPlayerName;

        matched = false;
        lastEnemyScore = -1;

        SendJoinOnce();
        currentScore = matchState.MyScore;
        SendState();
        InvokeRepeating(nameof(FetchState), 0.5f, 0.5f);
    }

    /* ======================
       完全停止
    ====================== */
    private void StopAllSync()
    {
        CancelInvoke();
        StopAllCoroutines();
        enabled = false;
    }

   
    IEnumerator SendStateCoroutine()
    {
       
        isSending = true;

        WWWForm form = new WWWForm();
        form.AddField("room_id", roomId);
        form.AddField("player_name", playerName);
        form.AddField("score", currentScore);
        form.AddField("difficulty", ModeManager.CurrentDifficulty.ToString());

        using (UnityWebRequest req = UnityWebRequest.Post(updateUrl, form))
        {
            req.timeout = 10;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[MultiSync] Send error: " + req.error);
            }
        }
       // Debug.Log($"[SEND] room={roomId}, name={playerName}, score={currentScore}");

        isSending = false;
    }

    /* ======================
       状態取得
    ====================== */
    private void FetchState()
    {
        //Debug.Log("[FetchState] called");
        if (isFetching) return;
        StartCoroutine(FetchStateCoroutine());
    }

    IEnumerator FetchStateCoroutine()
    {
        isFetching = true;

        //Debug.Log("[FETCH] start");

        string url =
      $"{fetchUrl}?room_id={roomId}" +
      $"&difficulty={ModeManager.CurrentDifficulty}" +
      $"&player_name={playerName}";


        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.timeout = 10;
            yield return req.SendWebRequest();
            //Debug.Log("[FETCH RAW RESPONSE] " + req.downloadHandler.text);

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "[FETCH FAILED]\n" +
                    "url: " + url + "\n" +
                    "result: " + req.result + "\n" +
                    "error: " + req.error
                );
                isFetching = false;
                yield break;
            }

            if (string.IsNullOrEmpty(req.downloadHandler.text))
            {
                isFetching = false;
                yield break;
            }

            PlayerState[] states =
                JsonHelper.FromJson<PlayerState>(req.downloadHandler.text);

            UpdateRemoteUI(states);
            CheckMatchSuccess(states);
        }

        isFetching = false;
    }

    void UpdateRemoteUI(PlayerState[] states)
    {
        //Debug.Log($"[UpdateRemoteUI] called. states.Count={states.Length}");
        bool enemyFound = false;

        foreach (var ps in states)
        {
            if (ps.player_name == playerName)
                continue;

            enemyFound = true;
            enemyPreviouslyPresent = true;

            // ✅ MultiSyncManager 自身にもセット
            opponentName = ps.player_name;
            opponentScore = ps.score;

            // ✅ MatchState 更新（正本）
            matchState.SetEnemy(ps.player_name, ps.score);

            // ✅ UIへ通知
            modeManager?.OnEnemyScoreUpdated(ps.player_name, ps.score);
        }

        if (!enemyFound && enemyPreviouslyPresent)
        {
            enemyPreviouslyPresent = false;

            opponentName = "";
            opponentScore = 0;

            modeManager?.OnEnemyLeft();
        }

        //Debug.Log($"[ENEMY] name={opponentName}, score={opponentScore}");
    }

    /* ======================
       マッチ成立判定
    ====================== */
    void CheckMatchSuccess(PlayerState[] states)
    {
        //Debug.Log($"[CHECK] myName={playerName}, count={(states == null ? -1 : states.Length)}");
        //if (states != null)
        //{
        //    foreach (var ps in states)
        //    {
        //        Debug.Log($"[STATE] {ps.player_name}");
        //    }
        //}

        if (matched || states == null) return;

        foreach (var ps in states)
        {
            if (ps.player_name != playerName)
            {
                matched = true;
                enemyPreviouslyPresent = true;
                modeManager?.OnMatchSuccess(ps.player_name);
                return;
            }
        }
    }


    /* ======================
       Join
    ====================== */
    public void SendJoinOnce()
    {
        if (joined) return;
        joined = true;
        StartCoroutine(SendJoinCoroutine());
    }

    IEnumerator SendJoinCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("room_id", roomId);
        form.AddField("player_name", playerName);
        form.AddField("difficulty", ModeManager.CurrentDifficulty.ToString());

        using (UnityWebRequest req = UnityWebRequest.Post(joinUrl, form))
        {
            req.timeout = 10;
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log("[MultiSync] Joined room");
            }
            else
            {
                Debug.LogError("[MultiSync] Join failed: " + req.error);
            }
        }
    }


    void SendState()
    {
        if (isSending) return;
        StartCoroutine(SendStateCoroutine());
    }

    public void StopMultiSync()
    {
        CancelInvoke();
        StopAllCoroutines();
        enabled = false;

       // Debug.Log("[MULTI] Sync stopped");
    }

    // =====================
    // 手動スコア送信（ボタン用）
    // =====================
    public void SendScoreManually()
    {
        if (!ModeManager.IsMultiMode) return;
        if (isSending) return;

        // ✅ MatchState を読むだけ
        currentScore = matchState.MyScore;

        Debug.Log(
            $"[MULTI SEND] name={matchState.MyName}, score={currentScore}"
        );

        StartCoroutine(SendScoreManuallyCoroutine());
    }

    IEnumerator SendScoreManuallyCoroutine()
    {
        
        yield return StartCoroutine(SendStateCoroutine());
        OnScoreSent?.Invoke(); // ✅ 通信後
    }

    public void ResetJoinState()
    {
        joined = false;
        matched = false;
        enemyPreviouslyPresent = false;
        lastEnemyScore = -1;
    }

   

    public void SendScoreIfHigher(int currentTotalScore)
    {
        // 前回送信より大きければ送信
        if (currentTotalScore <= lastSentScore)
        {
            Debug.Log($"[MULTI SKIP] current={currentTotalScore}, lastSent={lastSentScore}");
            return;
        }

        lastSentScore = currentTotalScore;
        currentScore = currentTotalScore;

        Debug.Log($"[MULTI SEND] new high score = {currentScore}");
        StartCoroutine(SendStateCoroutine());
    }
}

/* ======================
   通信用データ
====================== */
[System.Serializable]
public class PlayerState
{
    public string player_name;
    public int score;
}