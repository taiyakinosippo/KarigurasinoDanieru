using UnityEngine;
using TMPro;

public class MainUIMultiManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI ScoreText;

    private MatchState matchState;

    void Start()
    {
        matchState = FindObjectOfType<MatchState>();
    }

    void Update()
    {
        if (matchState == null) return;

        ScoreText.text =
            $"YOU: {matchState.MyScore}\n" +
            $"ENEMY: {matchState.EnemyScore}";
    }
}
