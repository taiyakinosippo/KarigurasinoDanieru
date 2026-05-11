using UnityEngine;

public class MatchState : MonoBehaviour
{
    public string MyName { get; private set; }
    public int MyScore { get; private set; }

    public string EnemyName { get; private set; }
    public int EnemyScore { get; private set; }

    public bool IsMatched { get; private set; }

    public void SetMyPlayer(string name)
    {
        MyName = name;
    }

    public void SetMyScore(int score)
    {
        MyScore = score;
    }

    public void SetEnemy(string name, int score)
    {
        EnemyName = name;
        EnemyScore = score;
        IsMatched = true;
    }

    public void ResetState()
    {
        MyName = "";
        MyScore = 0;
        EnemyName = "";
        EnemyScore = 0;
        IsMatched = false;
    }
}
