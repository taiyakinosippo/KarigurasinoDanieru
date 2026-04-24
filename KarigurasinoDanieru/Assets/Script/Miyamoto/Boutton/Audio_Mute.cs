using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Audio_Mute : MonoBehaviour ,IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public AudioSource audioSource;
    private Image _button;
    private Color _defaultColor;
    [SerializeField] Color _selectedColor = new Color(1.2f, 1.2f, 1.2f, 1f);

    void Start()
    {
        _button = GetComponent<Image>();
        _defaultColor = _button.color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _button.color = _selectedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _button.color = _defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        audioSource.mute = !audioSource.mute;
        if (audioSource.mute == true) return;
        AudioSourceManager.instance.PlaySE(SEType.SelectbuttonSE);
        
    }


}
