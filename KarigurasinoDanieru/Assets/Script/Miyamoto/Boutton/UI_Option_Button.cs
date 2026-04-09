using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UI_Option_Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Image _button;
    public GameObject _option;
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
        StartCoroutine(CloseOption());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _button.color = _defaultColor;
    }

    IEnumerator CloseOption()
    {
        yield return new WaitForSeconds(0.1f); 
        _option.SetActive(true);
    }
}
