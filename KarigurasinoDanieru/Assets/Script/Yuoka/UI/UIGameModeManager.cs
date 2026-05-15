using UnityEngine;
using System.Collections;

public class UIGameModeManager : MonoBehaviour
{
    [SerializeField] CountdownManager countdownManager; //演出終了後に開始するカウントダウン

    [Header("画面分割用オブジェクト")]
    public RectTransform playerView; //自分のゲーム画面を表示する親要素
    public RectTransform enemyView;  //敵のゲーム画面を表示する親要素

    [Header("登場・退場させるUIパネル")]
    public RectTransform balanceBarPanel; //バランスバーUI
    public RectTransform mashButtonPanel; //連打ボタンUI
    public RectTransform timingBarPanel;  //タイミングバーUI

    [Header("演出設定")]
    public float transitionDuration = 1.0f;//アニメーションにかかる時間
    public bool isMulti = true;

    private void Start()
    {
        SetupScreen();
    }

    /// <summary>
    /// 各オブジェクトの初期配置を行い、演出を開始する
    /// </summary>
    public void SetupScreen(/*GameMode mode*/)
    {
        //マルチかどうかの判定
        //bool isMulti = (mode == GameMode.Multi);

        // 画面分割の初期状態を設定
        enemyView.gameObject.SetActive(isMulti);
        if (isMulti)
        {
            //マルチ：自分を左半分、敵を右半分に配置
            playerView.anchorMax = new Vector2(0.5f, 1);
            enemyView.anchorMin = new Vector2(0.5f, 0);
            enemyView.anchorMax = new Vector2(1, 1);
        }
        else
        {
            //ソロ：自分を全画面に配置
            playerView.anchorMax = new Vector2(1, 1);
        }
        //アンカー変更時に発生するずれをリセット
        ResetOffsets(playerView);
        ResetOffsets(enemyView);

        // UIを画面外(初期位置)へ動かす
        balanceBarPanel.anchoredPosition = new Vector2(-500f, 0);
        mashButtonPanel.anchoredPosition = new Vector2(500f, 0);
        timingBarPanel.anchoredPosition = new Vector2(0, -400f);


        // 演出開始（共通のコルーチンを1つだけ呼ぶ。引数でマルチかどうかを伝える）
        StartCoroutine(StartGameTransition(isMulti));
    }

    /// <summary>
    /// ゲーム開始時の登場演出（UIが中に入り、マルチなら自分の画面にズームする）
    /// </summary>
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

            // 共通UIの移動（ソロ・マルチ問わず実行）
            balanceBarPanel.anchoredPosition = new Vector2(Mathf.Lerp(-500f, 0, t), 0);
            mashButtonPanel.anchoredPosition = new Vector2(Mathf.Lerp(500f, 0, t), 0);
            timingBarPanel.anchoredPosition = new Vector2(0, Mathf.Lerp(-400f, 0, t));

            // 画面ズーム（マルチの時だけ実行）
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

        //演出終了後の処理
        if (isMulti) enemyView.gameObject.SetActive(false);
        countdownManager.StartCountdown();
        Debug.Log("Game Start!");
    }

    /// <summary>
    /// ゲーム終了時に呼び出し、UIを退場させて画面分割を戻す
    /// </summary>
    public void ShowResultLayout()
    {
        StopAllCoroutines();
        StartCoroutine(EndGameTransition());
    }

    /// <summary>
    /// ゲーム終了時のUI演出
    /// </summary>
    private IEnumerator EndGameTransition()
    {
        if (isMulti)
        {
            //敵の画面を再度表示する
            enemyView.gameObject.SetActive(true);
        }

        float elapsed = 0;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / transitionDuration);

            //UIを画面外へ動かす
            balanceBarPanel.anchoredPosition = new Vector2(Mathf.Lerp(0,-500f, t), 0);
            mashButtonPanel.anchoredPosition = new Vector2(Mathf.Lerp(0,500f, t), 0);
            timingBarPanel.anchoredPosition = new Vector2(0, Mathf.Lerp(0, -400f, t));

            //画面を分割する
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

    /// <summary>
    /// RectTransformのOffset（インスペクター上のLeft, Rightなど）を強制的に0にする
    /// アンカーを変更した際の意図しないズレを防ぐために使用
    /// </summary>
    private void ResetOffsets(RectTransform rect)
    {
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }
}