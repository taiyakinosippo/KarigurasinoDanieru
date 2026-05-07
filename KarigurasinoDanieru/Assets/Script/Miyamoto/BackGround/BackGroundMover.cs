using UnityEngine;
using UnityEngine.UI;
public class BackGroundMover : MonoBehaviour
{
    [SerializeField] private RectTransform[] images;     //配置する画像
    [SerializeField] private float speed = 100;　　　　　//基礎スピード
    [SerializeField] private StageManager stageManager;  //ステージの情報を保持している
    [SerializeField] private int denominator = 10;　　　 //分母
    private float height = 540f;                         //画像の高さ
    private bool backGround = false;　　　　　　　　　　 //背景を動かすかどうか
    private int score = 0;                               //スコアを格納するための変数
    private float firstSpeed = 0;  　　　　　　　　　　　 //基礎スピードを格納するための変数
    [SerializeField] private float deceleration = 2f;　　//減速率
    private float targetDistance = 0;
    private float movedDistance = 0;

    private void Start()
    {
        foreach (var img in images)
        {
            Sprite sprite =
            stageManager.GetRandomBackground(0);
            if (sprite != null)
                img.GetComponent<Image>().sprite = sprite;
        }
    }

    public void StartMoving()
    {
        firstSpeed = speed;
        backGround = true;
        score = ScoreManager.instance.GetScore();
        targetDistance = score;
        speed += score / denominator;
    }


    private void Update()
    {
        if (!backGround) return;
        if (images == null || images.Length == 0) return;

        float moveAmount = speed * Time.deltaTime;
        movedDistance += moveAmount;

        foreach (var img in images)
        {
            img.anchoredPosition -= new Vector2(0, moveAmount);
        }

        if (movedDistance >= targetDistance)
        {
            backGround = false;

            AdjustBackgroundPosition();

            return;
        }
        RectTransform lowest = GetLowest();

        if (lowest != null && lowest.anchoredPosition.y <= -height)
        {
            RectTransform highest = GetHighest();
            if (highest == null) return;
            lowest.anchoredPosition = new Vector2(0, highest.anchoredPosition.y + height);
            var sprite = stageManager.GetRandomBackground(score);
            if (sprite != null)
                lowest.GetComponent<Image>().sprite = sprite;
        }
        speed = Mathf.Lerp(speed, firstSpeed, Time.deltaTime * deceleration);
    }

    RectTransform GetLowest()
    {
        if (images == null || images.Length == 0) return null;

        RectTransform result = images[0];
        foreach (var img in images)
        {
            if (img != null && img.anchoredPosition.y < result.anchoredPosition.y)
                result = img;
        }
        return result;
    }

    RectTransform GetHighest()
    {
        if (images == null || images.Length == 0) return null;

        RectTransform result = images[0];
        foreach (var img in images)
        {
            if (img != null && img.anchoredPosition.y > result.anchoredPosition.y)
                result = img;
        }
        return result;
    }
    void AdjustBackgroundPosition()
    {
        foreach (var img in images)
        {
            Vector2 pos = img.anchoredPosition;

            pos.y = Mathf.Round(pos.y / height) * height;

            img.anchoredPosition = pos;
            backGround = false;
        }
    }
}
