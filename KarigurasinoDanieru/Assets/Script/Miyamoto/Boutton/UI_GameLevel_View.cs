using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_GameLevel_View : MonoBehaviour,IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image button;
    [SerializeField] private Sprite _button;     // 通常のボタン画像
    [SerializeField] private Sprite  downButton; // 押されたときのボタン画像
    public Canvas target;                        // 対象のUIキャンバス
    [SerializeField] private GameLevel gameLevel;// ゲームレベルの指定
    [SerializeField] private UIAction action;
    [SerializeField] private Animator animator;
    [SerializeField] private UI_Text text;

    void Start()
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
        GameManager.instance.GameLevelSelect(gameLevel);
        text.View();
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
        animator.CrossFade("Show", 0f, 0, 0f);
    }
}


