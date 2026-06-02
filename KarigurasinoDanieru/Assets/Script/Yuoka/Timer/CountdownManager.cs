using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int countdownTime = 3;

    [SerializeField] private InGameTimer inGameTimer;
    [SerializeField] Timing_Bar_Logic timingBar;
    [SerializeField] BalanceBarController balanceBar;
    [SerializeField] PointAreaController pointArea;
    [SerializeField] MashButton mashButton;
 
    public void StartCountdown()
    {
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        int remainingTime = countdownTime;
        while (remainingTime > 0)
        {
            countdownText.text = remainingTime.ToString();
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }
        countdownText.text = "START!";

        if (inGameTimer != null)
        {
            inGameTimer.SetTimerRunning(true);
        }

        if (timingBar != null)
        {
            timingBar.StartTimingBar();
        }

        if (balanceBar != null)
        {
            balanceBar.StartBar();
        }

        if (pointArea != null)
        {
            pointArea.StartPointArea();
        }

        if (mashButton != null)
        {
            mashButton.StartMashButton();
        }

        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        BGM_Manager.Instance.PlayGameBGM();   
    }
}
