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

    // 内部状態
    private string roomId;
    private bool matched;
    private bool joined;
    private bool isFetching;
    private bool isSending;

    private ModeManager modeManager;

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
        if (enabled)
        {
            Debug.LogWarning("[MultiSync] Already running");
            return;
        }

        roomId = ModeManager.CurrentRoomId;
        playerName = ModeManager.MultiPlayerName;

        if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("[MultiSync] roomId or playerName empty");
            return;
        }

        matched = false;
        joined = false;
        isFetching = false;
        isSending = false;

        enabled = true;

        SendJoinOnce();
        InvokeRepeating(nameof(FetchState), 0.3f, 0.6f);

        Debug.Log("[MultiSync] Started");
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

    /* ======================
       状態送信
    ====================== */
    private void SendStateOnce()
    {
        if (isSending) return;
        StartCoroutine(SendStateCoroutine());
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

        string url = $"{fetchUrl}?room_id={roomId}";
        Debug.Log("[MultiSync] Fetch: " + url);

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.timeout = 10;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[MultiSync] Fetch error: " + req.error);
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

            Debug.Log($"[ROOM:{roomId}] {ps.player_name} SCORE:{ps.score}");
        }
    }

    /* ======================
       マッチ成立判定
    ====================== */
    void CheckMatchSuccess(PlayerState[] states)
    {
        if (matched || states == null) return;

        foreach (var ps in states)
        {
            if (ps.player_name != playerName)
            {
                matched = true;

                SendStateOnce();
                StopAllSync();

                if (modeManager != null)
                {
                    modeManager.OnMatchSuccess(ps.player_name);
                }

                Debug.Log("[MultiSync] Match Success!");
                break;
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