using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_Display_Switching : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject image;

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.SetActive(false);
    }

}
