using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// シーン遷移のためのクラス。ボタンを押したときに、MainSceneに遷移する。
/// </summary>
public class UI_Scene_Changer : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image button;
    [SerializeField]private Sprite _button;
    [SerializeField] private Sprite downButton;
    [SerializeField] private MainModeManager modeManager;

    public void Start()
    {
        button = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        button.sprite = downButton;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        button.sprite = _button;  
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioSourceManager.instance.PlaySE(SEType.clickSE);
        modeManager.OnGoButtonPressed();
    }
}