using UnityEngine;

public class MatchState : MonoBehaviour
{
    public string MyName { get; private set; }
    public int MyScore { get; private set; }

    public string EnemyName { get; private set; }
    public int EnemyScore { get; private set; }

    public bool IsMatched { get; private set; }

    void Awake()
    {
        if (FindObjectsOfType<MatchState>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }

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

        Debug.Log($"[MATCH SET] Enemy={name}, Score={score}");
    }


    public void ResetState()
    {
        MyName = "";
        MyScore = 0;
        EnemyName = "";
        EnemyScore = 0;
        IsMatched = false;
    }

    public void EnablePersistence()
    {
        DontDestroyOnLoad(gameObject);
    }
}
