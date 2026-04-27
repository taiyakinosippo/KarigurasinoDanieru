using UnityEngine;
using TMPro;

public class UI_Text : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI gameMode_text;
    [SerializeField]private TextMeshProUGUI gameLevel_text;

    public void View()
    {
        gameMode_text.text = "Mode : " + GameManager.instance.currentMode;
        gameLevel_text.text = "Level : " + GameManager.instance.currentLevel;
    }
}
