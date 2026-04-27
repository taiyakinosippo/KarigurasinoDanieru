using UnityEngine;

public class UI_Barview : MonoBehaviour
{
    private RectTransform bar_transpos;
    public void Awake()
    {
        bar_transpos = GetComponent<RectTransform>();
    }
    public void barPosition(float move, float _maxwindth)
    {
        float windth = _maxwindth * move;
        bar_transpos.anchoredPosition = new Vector2(windth, bar_transpos.anchoredPosition.y);
    }

    public void HideBar()
    {
        bar_transpos.gameObject.SetActive(false);
    }

    public void ShowBar()
    {
        bar_transpos.gameObject.SetActive(true);
        bar_transpos.anchoredPosition = new Vector2(0, bar_transpos.anchoredPosition.y);
    }
}
