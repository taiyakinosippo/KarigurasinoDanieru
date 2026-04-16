using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class WebRankingManager : MonoBehaviour
{
    public string saveUrl;
    public string rankingUrl;
    public Text webRankText;

    [System.Serializable]
    public class RankData
    {
        public string name;
        public int score;
    }

    void Start()
    {
        // シーン開始時にWebランキング取得
        StartCoroutine(GetRanking());
    }

    public void SendScore(string name,int score)
    {
        StartCoroutine(SendScoreCoroutine(name, score));
    }


    private IEnumerator SendScoreCoroutine(string name, int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("score", score);

        UnityWebRequest www = UnityWebRequest.Post(saveUrl, form);
        yield return www.SendWebRequest();
    }


    public IEnumerator GetRanking()
    {
        UnityWebRequest www = UnityWebRequest.Get(rankingUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            webRankText.text = "取得失敗";
            yield break;
        }

        RankData[] ranks =
            JsonHelper.FromJson<RankData>(www.downloadHandler.text);

        webRankText.text = "";
        int rank = 1;

        foreach (var r in ranks)
        {
            webRankText.text +=
                "No. "+rank+ " " +  r.name + "  " + r.score + "\n";
            rank++;
        }
    }
}