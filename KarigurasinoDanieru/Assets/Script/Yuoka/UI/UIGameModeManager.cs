using UnityEngine;
using System.Collections;

public class UIGameModeManager : MonoBehaviour
{
    [SerializeField] CountdownManager countdownManager;

    public RectTransform playerView;
    public RectTransform enemyView;
    public RectTransform balanceBarPanel; //
    public RectTransform mashButtonPanel; //
    public RectTransform timingBarPanel;  // 
    public float transitionDuration = 1.0f;
    public bool isMulti = true;

    private void Start()
    {
        SetupScreen();
    }

    // ゲーム開始時に呼ばれるメイン関数
    public void SetupScreen(/*GameMode mode*/)
    {
        //マルチかどうかの判定
        //bool isMulti = (mode == GameMode.Multi);

        // 敵画面の初期位置とアクティブ設定
        enemyView.gameObject.SetActive(isMulti);
        if (isMulti)
        {
            playerView.anchorMax = new Vector2(0.5f, 1);
            enemyView.anchorMin = new Vector2(0.5f, 0);
            enemyView.anchorMax = new Vector2(1, 1);
        }
        else
        {
            playerView.anchorMax = new Vector2(1, 1);
        }
        ResetOffsets(playerView);
        ResetOffsets(enemyView);

        // UIを画面外へ
        balanceBarPanel.anchoredPosition = new Vector2(-500f, 0);
        mashButtonPanel.anchoredPosition = new Vector2(500f, 0);
        timingBarPanel.anchoredPosition = new Vector2(0, -400f);


        // 演出開始（共通のコルーチンを1つだけ呼ぶ。引数でマルチかどうかを伝える）
        StartCoroutine(StartGameTransition(isMulti));
    }

    private IEnumerator StartGameTransition(bool isMulti)
    {
        // マルチの時だけ、相手を確認させるために少し待機
        if (isMulti) yield return new WaitForSeconds(2.0f);
        //else yield return new WaitForSeconds(0.5f);

        float elapsed = 0;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / transitionDuration);

            // 共通UIの移動（ソロ・マルチ問わず実行） ---
            balanceBarPanel.anchoredPosition = new Vector2(Mathf.Lerp(-500f, 0, t), 0);
            mashButtonPanel.anchoredPosition = new Vector2(Mathf.Lerp(500f, 0, t), 0);
            timingBarPanel.anchoredPosition = new Vector2(0, Mathf.Lerp(-400f, 0, t));

            // 画面ズーム（マルチの時だけ実行） ---
            if (isMulti)
            {
                float edge = Mathf.Lerp(0.5f, 1.0f, t);
                playerView.anchorMax = new Vector2(edge, 1);
                enemyView.anchorMin = new Vector2(edge, 0);
                enemyView.anchorMax = new Vector2(edge + 0.5f, 1);
                ResetOffsets(playerView);
                ResetOffsets(enemyView);
            }

            yield return null;
        }

        if (isMulti) enemyView.gameObject.SetActive(false);
        countdownManager.StartCountdown();
        Debug.Log("Game Start!");
    }

    private void ResetOffsets(RectTransform rect)
    {
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    public void ShowResultLayout()
    {
        StopAllCoroutines();
        StartCoroutine(EndGameTransition());
    }

    private IEnumerator EndGameTransition()
    {
        if (isMulti)
        {
            enemyView.gameObject.SetActive(true);
        }

        float elapsed = 0;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / transitionDuration);

            balanceBarPanel.anchoredPosition = new Vector2(Mathf.Lerp(0,-500f, t), 0);
            mashButtonPanel.anchoredPosition = new Vector2(Mathf.Lerp(0,500f, t), 0);
            timingBarPanel.anchoredPosition = new Vector2(0, Mathf.Lerp(0, -400f, t));

            if (isMulti)
            {
                float edge = Mathf.Lerp(1.0f,0.5f, t);
                playerView.anchorMax = new Vector2(edge, 1);
                enemyView.anchorMin = new Vector2(edge, 0);
                enemyView.anchorMax = new Vector2(edge + 0.5f, 1);
                ResetOffsets(playerView);
                ResetOffsets(enemyView);
            }
            yield return null;
        }
    }
}