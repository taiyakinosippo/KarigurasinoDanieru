using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// モードやレベル選択がない場合のUIのボタンにアニメーションをつけるクラス。
/// </summary>

public class UI_Button_visual_Anim : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image  button;             　　　　　　 // ボタンのImageコンポーネント
    public Canvas target;                           // ボタンを押したときに表示・非表示にするUIのCanvas
    [SerializeField] private Sprite _button;        // ボタンの通常状態のスプライト
    [SerializeField] private Sprite _downButton;    // ボタンの押下状態のスプライト
    [SerializeField] private UIAction action;       // ボタンのアクション（表示か非表示か）
    [SerializeField] private Animator animator;     // アニメーションを制御するAnimatorコンポーネント

    void Start()
    {
        button = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        button.sprite = _downButton;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        button.sprite = _button;
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
        UI_Manager.instance.ScheduleCloseUI(target, 1.0f);
        yield break;
    }

    private IEnumerator PlayShowAnimation()
    {
        UI_Manager.instance.ShowUI(target);
        animator.CrossFade("Show", 0f, 0, 0f);
        yield return new WaitForSeconds(1.0f);
    }
}
