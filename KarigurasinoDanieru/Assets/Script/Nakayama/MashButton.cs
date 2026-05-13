using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MashButton : MonoBehaviour
{
    public int baseScore = 10;//一回クリックのポイント

    public bool isHardMode = false;//ハードモード切り替え
    public Image targetImage;//透明度を下げる画像
    private float fadeTimer = 0f;//
    private float fadeInterval = 1f;//
    public float fadeDuration = 5f;


    public Image clickImage;//クリック時に表示する画像
    public float displayTime = 1.0f;//画像を消す秒数

    private float timerw;
    private bool isGameOver = false;//

    public AudioSource audioSource;//
    public AudioClip clickSound;//

    void Start()
    {
        timerw = fadeDuration;
    }
    void Update()
    {
        if (isGameOver) return;

        if (isHardMode)
        {
            timerw -= Time.deltaTime;
            timerw = Mathf.Clamp(timerw, 0, fadeDuration);

            float alpha = timerw / fadeDuration;

            targetImage.color = new Color(1, 1, 1, alpha);

        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //clickCount++;
            //score = clickCount * baseScore;
            PlaySound();
            ShowImage();
            ScoreManager.instance.MashButtonScore(baseScore);
        }
    }
    //void DecreaseAlpha()
    //{
    //    Color color = targetImage.color;
    //    color.a -= 0.1f;
    //    color.a = Mathf.Clamp01(color.a);
    //    targetImage.color = color;
    //}
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

    public void StopMashButton()
    {
        isGameOver = true;
    }
}