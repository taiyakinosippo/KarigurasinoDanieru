using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MashButton : MonoBehaviour
{
    public int baseScore = 10;//一回クリックのポイント

    bool isHardMode = false;//ハードモード切り替え
    public Image targetImage;//透明度を下げる画像
    public float fadeDuration = 5f;
    public RectTransform buttonRect;

    public Image clickImage;//クリック時に表示する画像
    public float displayTime = 1.0f;//画像を消す秒数

    private float timerw;
    private bool isGameOver = true;//

    public AudioSource audioSource;//
    public AudioClip clickSound;//

    public int clickCount = 0;//クリック数
    public int score = 0;//スコア

    // ===================
    // ランダム移動用
    // ===================

    public RectTransform moveTarget;

    //  public float moveRange = 300f;
    public float moveInterval = 2f;
    public float moveDuration = 0.5f;

    public float minY = -190f;
    public float maxY = 190f;

    private Vector2 startPos;
    
    public void SetupMashButton(GameLevel level)
    {
        isHardMode = (level == GameLevel.Hard);

        timerw = fadeDuration;

        startPos = moveTarget.anchoredPosition;

        if (isHardMode)
        {
            StartCoroutine(RandomMoveRoutine());
        }
    }

    void Update()
    {
        if (isGameOver) return;

        if (isHardMode)
        {
            timerw -= Time.deltaTime;
            timerw = Mathf.Clamp(timerw, 0, fadeDuration);

            float alpha = timerw / fadeDuration;

            //targetImage.color = new Color(1, 1, 1, alpha);

            // StartCoroutine(RandomMoveRoutine());

        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (RectTransformUtility.RectangleContainsScreenPoint(
                buttonRect,
                mousePos,
                null))
            {
                clickCount++;
                score = clickCount * baseScore;
                PlaySound();
                ShowImage();

                
            }
            ScoreManager.instance.MashButtonScore(score);
        }
    }

    void PlaySound()
    {
        audioSource.PlayOneShot(clickSound);
    }
    void ShowImage()
    {
        StartCoroutine(ShowAndHide());
    }

    IEnumerator ShowAndHide()
    {
        clickImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        clickImage.gameObject.SetActive(false);
    }

    public void StartMashButton()
    {
        isGameOver = false;
    }

    public void StopMashButton()
    {
        isGameOver = true;
    }


    IEnumerator RandomMoveRoutine()
    {
        // ゲーム開始まで待機
        yield return new WaitUntil(() => !isGameOver);

        while (!isGameOver)
        {
            yield return new WaitForSeconds(moveInterval);

            if (!isHardMode) yield break;

            float randomY = Random.Range(minY, maxY);

            Vector2 targetPos =
                new Vector2(startPos.x, randomY);

            yield return StartCoroutine(
                MoveToPosition(targetPos)
                );
        }
    }

    IEnumerator MoveToPosition(Vector2 targetPos)
    {
        Vector2 currentPos = moveTarget.anchoredPosition;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / moveDuration;

            moveTarget.anchoredPosition =
                Vector2.Lerp(currentPos, targetPos, t);

            yield return null;
        }

        moveTarget.anchoredPosition = targetPos;
    }
}