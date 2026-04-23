using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class MultiSyncManager : MonoBehaviour
{

    [Header("Server URLs")]
    public string updateUrl = "../mp_update.php";
    public string fetchUrl = "../mp_fetch.php";

    [Header("Player Info")]
    public string playerName;
    public int currentScore;

    // 内部用
    private string roomId;

    private bool matched = false;

    void Start()
    {
        // ソロなら何もしない
        if (!ModeManager.IsMultiMode)
        {
            enabled = false;
            return;
        }

        if (string.IsNullOrEmpty(ModeManager.CurrentRoomId))
        {
            Debug.LogError("RoomID が未設定！");
            enabled = false;
            return;
        }

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = System.Guid.NewGuid().ToString();
        }

        roomId = ModeManager.CurrentRoomId;

        // 0.5秒ごとに同期
        InvokeRepeating(nameof(SendState), 0f, 0.5f);
        InvokeRepeating(nameof(FetchState), 0.25f, 0.5f);
    }

    /* ======================
       状態送信
    ====================== */
    void SendState()
    {
        StartCoroutine(SendStateCoroutine());
    }

    IEnumerator SendStateCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("room_id", roomId);
        form.AddField("player_name", playerName);
        form.AddField("score", currentScore);

        UnityWebRequest req = UnityWebRequest.Post(updateUrl, form);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("SendState error: " + req.error);
        }
    }

    /* ======================
       状態取得
    ====================== */
    void FetchState()
    {
        StartCoroutine(FetchStateCoroutine());
    }

    IEnumerator FetchStateCoroutine()
    {
        UnityWebRequest req =
            UnityWebRequest.Get(fetchUrl + "?room_id=" + roomId);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(req.error);
            yield break;   // ← これが無いと無限ループ
        }

        if (string.IsNullOrEmpty(req.downloadHandler.text))
            yield break;

        PlayerState[] states =
            JsonHelper.FromJson<PlayerState>(req.downloadHandler.text);

        if (states == null || states.Length == 0)
            yield break;

        UpdateRemoteUI(states);
        CheckMatchSuccess(states);
    }


    /* ======================
       UI更新
    ====================== */
    void UpdateRemoteUI(PlayerState[] states)
    {
        foreach (PlayerState ps in states)
        {
            // 自分は無視（必要なら表示も可）
            if (ps.player_name == playerName)
                continue;

            Debug.Log($"[ROOM:{roomId}] {ps.player_name} SCORE:{ps.score}");

            // ここで
            // ・相手スコア表示
            // ・順位表示
            // ・ゲージ更新
            // などを行う
        }
    }

    void CheckMatchSuccess(PlayerState[] states)
    {
        if (matched) return;

        foreach (PlayerState ps in states)
        {
            if (ps.player_name != playerName)
            {
                matched = true;

                CancelInvoke();
                enabled = false;

                FindObjectOfType<ModeManager>()
                    .OnMatchSuccess(ps.player_name);

                break;
            }
        }
    }

}

/* ======================
   送受信用データ構造
====================== */
[System.Serializable]
public class PlayerState
{
    public string player_name;
    public int score;
}