using UnityEngine;
using UnityEngine.UI;
public class BackGroundMover : MonoBehaviour
{
    [SerializeField] private RectTransform[] images;
    [SerializeField] private float speed = 100f;
    [SerializeField] private BackGroundChanger changer;
    private float height = 540f;

    private void Start()
    {
        // 初期化（最初からランダムにしておく）
        foreach (var img in images)
        {
            var sprite = changer.GetRandomBackground();
            if (sprite != null)
                img.GetComponent<Image>().sprite = sprite;
        }
    }

    private void Update()
    {
        if (images == null || images.Length == 0) return;

        foreach (var img in images)
        {
            img.anchoredPosition -= new Vector2(0, speed * Time.deltaTime);
        }

        RectTransform lowest = GetLowest();

        if (lowest != null && lowest.anchoredPosition.y <= -height)
        {
            RectTransform highest = GetHighest();
            if (highest == null) return;

            // 上に回す
            lowest.anchoredPosition = new Vector2(0, highest.anchoredPosition.y + height);

            // ランダム背景
            var sprite = changer.GetRandomBackground();
            if (sprite != null)
                lowest.GetComponent<Image>().sprite = sprite;
        }
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
}
