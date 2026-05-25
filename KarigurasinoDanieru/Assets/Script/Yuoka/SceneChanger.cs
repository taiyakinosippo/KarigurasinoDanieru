using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Fade fade;

    public void ChangeScene(string sceneName)
    {
        if (fade == null)
        {
            Debug.LogError("[SceneChanger] Fade が設定されていません。");
            return;
        }

        // 「シーン遷移中（フェードアウトが必要）」という状態(1)をセーブする
        PlayerPrefs.SetInt("TransitionState", 1);
        PlayerPrefs.Save(); // 確実に保存

        // フェードイン（画面を暗くする）
        fade.FadeIn(1f, () =>
        {
            // 暗くなったらシーンを切り替える
            SceneManager.LoadScene(sceneName);
        });
    }
}