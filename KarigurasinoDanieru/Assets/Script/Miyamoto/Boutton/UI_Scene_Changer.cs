using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UI_Scene_Changer : MonoBehaviour, IPointerClickHandler ,IPointerDownHandler, IPointerUpHandler
{    public string sceneName;
    private Image _button;
    private Color _defaultColor;
    private bool isPointerDown = false;

    [SerializeField]public Color highlightColor = new Color(1.2f, 1.2f, 1.2f, 1f);
    [SerializeField] Fade fade;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioSourceManager.instance.PlaySE(SEType.clickSE);
        _button.color = _defaultColor;

        modeManager.OnGoButtonPressed();
    }
     public void OnPointerUp(PointerEventData eventData)
     {
        if(!isPointerDown)
        {
            _button.color = _defaultColor;
        }
     }

}
