using UnityEngine;

public class BarView : MonoBehaviour
{
   private RectTransform bar;

    public void Start()
    {
        bar = GetComponent<RectTransform>();
    }
    public void barPosition(float move, float _maxwindth)
    {
        float windth = _maxwindth * (move / 100f);
        bar.anchoredPosition = new Vector2(windth, bar.anchoredPosition.y);
    }
}
