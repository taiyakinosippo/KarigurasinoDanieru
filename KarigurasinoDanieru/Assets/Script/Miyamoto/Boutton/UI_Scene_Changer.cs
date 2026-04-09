using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Scene_Changer : MonoBehaviour, IPointerClickHandler
{
    public string sceneName;
    private Image image;
    [SerializeField]public Color highlightColor = new Color(1.2f, 1.2f, 1.2f, 1f);


    public void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        image.color = highlightColor;
        SceneManager.LoadScene(sceneName);
    }


}
