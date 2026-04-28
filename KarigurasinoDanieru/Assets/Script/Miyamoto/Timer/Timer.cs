using UnityEngine;
using TMPro;
public class Timer : MonoBehaviour
{
    [SerializeField] private BarBalanceController balanceBar;
    [SerializeField] private PointAreaController pointArea;
    [SerializeField] private Timeing_Bar_Logic timeingBar;

    [SerializeField]private float TimeLimit = 0f;
    [SerializeField]private PlayerJump playerJump;
    private float currentTime = 0f;
    public TextMeshProUGUI timeText;
    [HideInInspector] public bool isTimeStop= false;
    [HideInInspector] private bool isjump = false;

    private void Awake()
    {
        currentTime = TimeLimit;
    }
    void Update()
    {
        if (isTimeStop)
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
            FinishGame();
        }
    }

    void FinishGame()
    {
        isTimeStop = true;

        if (balanceBar != null)
        {
            balanceBar.StopBar();

            ScoreManager.instance.BalanceBarScore(
                balanceBar.meter,
                balanceBar.baseScore, 
                balanceBar.multiplier
                );
        }

        if (pointArea != null)
        {
            pointArea.StopPointArea();
        }

        if (timeingBar != null)
        {
            timeingBar.StopTimeingBar();
        }

        if (playerJump != null &&!isjump)
        {
            isjump = true;
            playerJump.Jump();
        }
    }
}

