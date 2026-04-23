using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_Button_visual_Anim : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image _button;
    public Canvas target;
    private Color _defaultColor;
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
        AudioSourceManager.instance.PlaySE(SEType.SelectbuttonSE);
        if (action == UIAction.Close)
        {
            StartCoroutine(PlayCloseAnimation());
        }
        else
        {
            StartCoroutine(PlayShowAnimation());
        }
    }

    private IEnumerator PlayCloseAnimation()
    {
        animator.CrossFade("Close", 0f, 0, 0f);
        yield return new WaitForSeconds(1.0f);
        UI_Manager.instance.CloseUI(target);
    }

    private IEnumerator PlayShowAnimation()
    {
        UI_Manager.instance.ShowUI(target);
        animator.CrossFade("Show", 0f, 0, 0f);
        yield return new WaitForSeconds(1.0f);
    }
}
