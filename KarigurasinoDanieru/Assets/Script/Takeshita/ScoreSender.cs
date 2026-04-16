using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreSender : MonoBehaviour
{
    public string saveUrl;

    public void SendScore(string name, int score)
    {
        Debug.Log($"送信しようとしている: {name}, {score}");
        StartCoroutine(PostScore(name, score));
    }

    IEnumerator PostScore(string name, int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("score", score);

        UnityWebRequest req =
            UnityWebRequest.Post(saveUrl, form);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ PHP に届いた");
            Debug.Log("レスポンス: " + req.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ 通信失敗: " + req.error);
        }
    }
}