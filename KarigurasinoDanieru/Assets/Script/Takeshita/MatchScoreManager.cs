using UnityEngine;

public class MatchScoreManager : MonoBehaviour
{
    [SerializeField] private MatchState matchState; 

    public int CurrentScore { get; private set; }


    public void AddScore(int value)
    {
        CurrentScore += value;
        matchState.SetMyScore(CurrentScore);

        Debug.Log(
            $"[ADD SCORE] instanceID={GetInstanceID()}, score={CurrentScore}, matchStateID={matchState.GetInstanceID()}"
        );
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        matchState.SetMyScore(CurrentScore);
    }

    public void SetScore(int value)
    {
        CurrentScore = value;
        matchState.SetMyScore(CurrentScore);
    }
}