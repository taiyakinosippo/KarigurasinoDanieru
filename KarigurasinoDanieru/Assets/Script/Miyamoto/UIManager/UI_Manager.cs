using UnityEngine;

public class UI_Manager : MonoBehaviour
{
   public static UI_Manager instance;
   private void Awake()
   {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowUI(Canvas ui)
    {
        ui.enabled = true;
    }

    public void CloseUI(Canvas ui)
    {
        ui.enabled = false;
    }
}

