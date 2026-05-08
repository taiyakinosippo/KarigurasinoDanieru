using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class BackGroundMover : MonoBehaviour
{
    [SerializeField] private RectTransform[] images;     //配置する画像
    [SerializeField] private StageManager stageManager;  //ステージの情報を保持している
    [SerializeField] private ScoreManager scoreManger;   //スコアの情報を保持している
    [SerializeField] private float deceleration = 2f;　　//減速率
    private float speed = 0;　　　　　　　　　　　　　　 //飛ぶスピードを格納する変数
    private float height = 540f;                         //画像の高さ
    private bool backGround = false;　　　　　　　　　　 //背景を動かすかどうか

    //初期化
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
    //初期化
    public void StartMoving(float score)
    {
        backGround = true;
        speed = scoreManger.GetAddScore(score);
        Debug.Log($"スピード{speed}");
    }


    private void Update()
    {
       
        if (!backGround) return;
        if (images == null || images.Length == 0) return;
       
        float moveAmount = speed * Time.deltaTime;
        foreach (var img in images)
        {
            img.anchoredPosition -= new Vector2(0, moveAmount);
        }
        RectTransform lowest = GetLowest();

        if (lowest != null && lowest.anchoredPosition.y <= -height)
        {
            RectTransform highest = GetHighest();
            if (highest == null) return;
            lowest.anchoredPosition = new Vector2(0, highest.anchoredPosition.y + height);
            var sprite = stageManager.GetRandomBackground(scoreManger.GetCurrentScore());
            if (sprite != null)
                lowest.GetComponent<Image>().sprite = sprite;
        }
        speed = Mathf.Lerp(speed, 0, Time.deltaTime * deceleration);
    }
    //画像がスクロールしきったか
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
    //一番高い画像がどこにあるのか
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
    //スクロールを止める
    public void ScrollEnd()
    {
        foreach (var img in images)
        {
            Vector2 pos = img.anchoredPosition;
            pos.y = Mathf.Round(pos.y / height) * height;
            img.anchoredPosition = pos;
            backGround = false;
            speed = 0;
        }
    }
}
