using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreSender : MonoBehaviour
{
    public string saveUrl;

    //mode を受け取る
    public void SendScore(string name, int score, string mode)
    {
        Debug.Log($"送信: {name}, {score}, mode={mode}");
        StartCoroutine(PostScore(name, score, mode));
    }

    IEnumerator PostScore(string name, int score, string mode)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("score", score);
        form.AddField("mode", mode); 

        UnityWebRequest req = UnityWebRequest.Post(saveUrl, form);

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

