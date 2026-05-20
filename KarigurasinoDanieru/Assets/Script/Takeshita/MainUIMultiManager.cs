using UnityEngine;
using TMPro;

public class MainUIMultiManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI ScoreText;

    private MatchState matchState;

    void Update()
    {
       if (matchState == null)
        {
            matchState = FindObjectOfType<MatchState>();
            return;
        }

        ScoreText.text =
            $"YOU: {matchState.MyScore}\n" +
            $"ENEMY: {matchState.EnemyScore}";
    }
}