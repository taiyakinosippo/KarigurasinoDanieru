using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// ゲームモード選択のボタンのクラス。ボタンを押したときの処理を記述。
/// </summary>
public class UI_GameModa_View : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image button;                       //ボタンのimageコンポーネント
    public Canvas target;                       //押したことで表示させるUIのCanvas
    [SerializeField] private Sprite _button;    //ボタンの通常時のスプライト
    [SerializeField] private Sprite _downButton;//ボタンを押したときのスプライト
    [SerializeField] private GameMode gameMode; //このボタンが選択するゲームモード
    [SerializeField] private UIAction action;   //このボタンが押されたときのアクション
    [SerializeField] private Animator animator; //ボタンを押したときのアニメーション

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
        animator.CrossFade("Show", 0f, 0, 0f);
    }
}
