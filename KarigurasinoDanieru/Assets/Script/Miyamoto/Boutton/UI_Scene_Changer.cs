using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UI_Scene_Changer : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image _button;
    private Color _defaultColor;
  

    [SerializeField] public Color highlightColor = new Color(1.2f, 1.2f, 1.2f, 1f);
    [SerializeField] private MainModeManager modeManager;

    public void Start()
    {
        _button = GetComponent<Image>();
        _defaultColor = _button.color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _button.color = highlightColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      
         _button.color = _defaultColor;
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SESourceManager.instance.PlaySE(SEType.clickSE);
        modeManager.OnGoButtonPressed();
    }
}