using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameMode currentMode;
    public GameLevel currentLevel;

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

}
