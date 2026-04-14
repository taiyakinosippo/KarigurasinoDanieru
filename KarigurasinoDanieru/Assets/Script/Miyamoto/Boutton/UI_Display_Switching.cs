using UnityEngine;
using UnityEngine.EventSystems;



public class UI_Display_Switching : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject _image;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _image.SetActive(true);
       AudioSourceManager.instance.PlaySE(SEType.indicatorSE);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _image.SetActive(false);
    }

}
