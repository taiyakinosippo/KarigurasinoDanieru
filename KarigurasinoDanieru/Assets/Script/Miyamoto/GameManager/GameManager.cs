using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public BalanceBarController balanceBar;
    public PointAreaController pointArea;
    public Timing_Bar_Logic timingBar;
    public MashButton mashButton;

    public GameMode currentMode;
    public GameLevel currentLevel;

    private bool isGameOver = false;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// ゲームモード選択のメソッド。引数でソロかマルチかを受け取る。
    /// </summary>
    public void GameModeSelect(GameMode mode)
    {
        currentMode = mode;
    }


    /// <summary>
    /// ゲームレベル選択のメソッド。引数でノーマルかハードかを受け取る。
    /// </summary>
    public void GameLevelSelect(GameLevel level)
    {
       currentLevel = level;
    }

    public void OnTimerFinished()
    {
        if (isGameOver) return;
        FinishGame();
    }

    public void FinishGame()
    {
        isGameOver = true;

        if (mashButton != null)
        {
            mashButton.StopMashButton();
        }

        if (timingBar != null)
        {
            timingBar.StopTimingBar();
        }

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
    }

}
