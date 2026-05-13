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
        lastEnemyScore = -1; // ✅ ★追加（超重要）

        SendState();
        InvokeRepeating(nameof(SendState), 0.5f, 0.5f);
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
        if (isFetching) return;
        StartCoroutine(FetchStateCoroutine());
    }

    IEnumerator FetchStateCoroutine()
    {
        isFetching = true;

        //Debug.Log("[FETCH] start");

        string url = $"{fetchUrl}?room_id={roomId}";
        

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.timeout = 10;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
               
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

    /* ======================
       UI更新（仮）
    ====================== */
    void UpdateRemoteUI(PlayerState[] states)
    {
        if (states == null) return;

        foreach (var ps in states)
        {
            if (ps.player_name == playerName)
                continue;

            if (ps.score != lastEnemyScore)
            {
                lastEnemyScore = ps.score;

                // ✅ 一時状態に保存するだけ
                matchState.SetEnemy(ps.player_name, ps.score);
            }
        }
    }

    /* ======================
       マッチ成立判定
    ====================== */
    void CheckMatchSuccess(PlayerState[] states)
    {
        if (matched || states == null) return;

        if (states.Length >= 2)
        {
            foreach (var ps in states)
            {
                if (ps.player_name != playerName)
                {
                    matched = true;

                    matchState.SetEnemy(ps.player_name, ps.score);
                    modeManager?.OnMatchSuccess(ps.player_name);
                    return;
                }
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

        using (UnityWebRequest req = UnityWebRequest.Post(joinUrl, form))
        {
            req.timeout = 10;
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[MultiSync] Joined room");
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
        StartCoroutine(SendScoreManuallyCoroutine());
    }

    IEnumerator SendScoreManuallyCoroutine()
    {
        yield return StartCoroutine(SendStateCoroutine());
        OnScoreSent?.Invoke(); // ✅ 通信後
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