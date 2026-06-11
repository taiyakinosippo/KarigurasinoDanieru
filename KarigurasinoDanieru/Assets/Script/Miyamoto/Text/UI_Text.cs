using UnityEngine;
using TMPro;

public class UI_Text : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI gameMode_text;
    [SerializeField]private TextMeshProUGUI gameLevel_text;

    public void View()
    {
        gameMode_text.text = "ゲームモード : " + GetGameModeLabel(GameManager.instance.currentMode);
        gameLevel_text.text = "ゲーム難易度 : " + GetGameLevelLabel(GameManager.instance.currentLevel);

        if (PlayerPrefs.GetInt("TutorialState") == 1)
        {
            gameLevel_text.text = "ゲーム難易度 : チュートリアル";
        }
    }

    private string GetGameModeLabel(GameMode mode)
    {
        return mode == GameMode.Multi ? "マルチ" : "ソロ";
    }

    private string GetGameLevelLabel(GameLevel level)
    {
        return level == GameLevel.Hard ? "ハード" : "ノーマル";
    }
}
