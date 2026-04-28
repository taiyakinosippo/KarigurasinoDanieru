using UnityEngine;
using TMPro;
public class Timer : MonoBehaviour
{
    [SerializeField]private float TimeLimit = 0f;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI score_text;
    private float currentTime = 0f;
    private bool isRunning = false;

    private void Awake()
    {
        currentTime = TimeLimit;
        isRunning = true;
    }
    void Update()
    {
        if (!isRunning)
        {
            return;
        }

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timeText.text = Mathf.Ceil(currentTime).ToString();
        }

        if (currentTime <= 0)
        {
            currentTime = 0;
            isRunning = false;

            GameManager.instance.OnTimerFinished();
            score_text.text = "Score: " + ScoreManager.instance.GetScore();
        }
    }

    //外部からタイマーを動かしたり止めたりするメソッド
    public void SetTimerRunning(bool state)
    {
        isRunning = state;
    }
}

