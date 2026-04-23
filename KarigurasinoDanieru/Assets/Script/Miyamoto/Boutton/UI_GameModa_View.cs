using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_GameModa_View : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image _button;
    public Canvas target;
    private Color _defaultColor;
    [SerializeField] private GameMode gameMode;
    [SerializeField] private UIAction action;
    [SerializeField] private Animator animator;
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
        GameManager.instance.GameModeSelect(gameMode);
        AudioSourceManager.instance.PlaySE(SEType.SelectbuttonSE);
        switch (action)
        {
            case UIAction.Show:
                UI_Manager.instance.ShowUI(target);
                break;

            case UIAction.Close:
                UI_Manager.instance.CloseUI(target);
                break;
        }
        animator.CrossFade("Difficulty_Select_Window_Show ", 0f);
    }
}
