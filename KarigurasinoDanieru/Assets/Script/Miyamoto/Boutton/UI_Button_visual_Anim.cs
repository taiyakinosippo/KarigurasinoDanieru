using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

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
            UI_Manager.instance.ShowUI(target);
        }
    }

    private IEnumerator PlayCloseAnimation()
    {
        animator.Play("Difficulty_Select_Window_Close", 0, 0f);

        float length = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(length);

        UI_Manager.instance.CloseUI(target);
    }

}
