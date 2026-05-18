using UnityEngine;

public class ResultWatcher : MonoBehaviour
{
    private bool isSent = false;

    void Update()
    {
        if (isSent) return;

        if (ScoreManager.instance == null) return;

        float score = ScoreManager.instance.GetScore();

        if (score > 0.1f)
        {
            isSent = true;

            //Debug.Log("[STORE SCORE ONLY]");

            ScoreHolder.instance.SetScore(score);
        }
    }
}
