using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClickCounter : MonoBehaviour
{
    public int clickCount = 0;//連打した回数
    public int score = 0;//スコア
    public int point = 1;//一回クリックのポイント

    public bool isHardMode = false;//ハードモード切り替え
    public Image targetImage;//透明度を下げる画像
    private float fadeTimer = 0f;//
    private float fadeInterval = 1f;//
    public float fadeDuration = 5f;

     public Text countText;//連打した回数のテキスト
    //public Text scoreText;
    public Text timerText;//残り時間



    public Image clickImage;//クリック時に表示する画像
    public float displayTime = 1.0f;//画像を消す秒数

    public float timeLimit = 10f;//
    private float timer;//
    private float timerw;
    private bool isPlaying = true;//

    public AudioSource audioSource;//
    public AudioClip clickSound;//

    void Start()
    {
        timerw = fadeDuration;
        timer = timeLimit;
        UpdateUI();
    }
    void Update()
    {
        if (!isPlaying) return;

        timer -= Time.deltaTime;
        timerText.text = "残り時間: " + Mathf.Ceil(timer).ToString();
        if(isHardMode)
        {
            if(isHardMode)
            {
                timerw -= Time.deltaTime;
                timerw = Mathf.Clamp(timerw, 0, fadeDuration);

                float alpha = timerw / fadeDuration;

                targetImage.color = new Color(1, 1, 1, alpha);

            }
        }
    
        if (timer <= 0)
        {
            timer = 0;
            isPlaying = false;
            timerText.text = "終了！";
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            clickCount++;
            score = clickCount * point;
            PlaySound();
            ShowImage();
            UpdateUI();
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
    void UpdateUI()
    {
        countText.text = "回数：" + clickCount;
     //   countText.text = "連打回数: " + clickCount;
      // scoreText.text = "スコア: " + score;
    }
}