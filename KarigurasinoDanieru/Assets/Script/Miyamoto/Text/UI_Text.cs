using UnityEngine;
using TMPro;

public class UI_Text : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI gameMode_text;
    [SerializeField]private TextMeshProUGUI gameLevel_text;

    public void View()
    {
        gameMode_text.text = "ゲームモード : " + GameManager.instance.currentMode;
        gameLevel_text.text = "ゲーム難易度 : " + GameManager.instance.currentLevel;

        if (PlayerPrefs.GetInt("TutorialState") == 1)
        {
            gameLevel_text.text = "ゲーム難易度 :チュートリアル";
        }
    }
}
