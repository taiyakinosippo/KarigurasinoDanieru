using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Option_Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private Image _button;
    public  Canvas _option;
    private Color _defaultColor;
    [SerializeField]public Color _highlightColor = new Color(1.2f, 1.2f, 1.2f, 1f);

    void Start()
    {
        _button = GetComponent<Image>();
        _defaultColor = _button.color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _button.color = _highlightColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _button.color = _defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioSourceManager.instance.PlaySE(SEType.clickSE);
        _option.enabled = true;
    }
}
