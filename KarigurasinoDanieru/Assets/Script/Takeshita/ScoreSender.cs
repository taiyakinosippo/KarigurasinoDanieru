using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScoreSender : MonoBehaviour
{
    public string saveUrl="../save_score.php";

    //mode を受け取る
    public void SendScore(string name, int score, string mode)
    {
        if (ModeManager.IsMultiMode)
        {
            return;
        }

        Debug.Log($"送信: {name}, {score}, mode={mode}");
        StartCoroutine(PostScore(name, score, mode));
    }

    IEnumerator PostScore(string name, int score, string mode)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("score", score);
        form.AddField("mode", mode);

        string url;

#if UNITY_EDITOR
    // ✅ Editorでは絶対URIを使う（HTTP可）
    url = "http://localhost/KarigurashinoDaniel_Unity/save_score.php";
#else
        // ✅ WebGLでは相対パス（SSL問題回避）
        url = "../save_score.php";
#endif

        UnityWebRequest req = UnityWebRequest.Post(url, form);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ PHP に届いた: " + req.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ 通信失敗: " + req.error);
        }
    }

} 

