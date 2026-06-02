using UnityEngine;

public class Fade_Out : MonoBehaviour
{
    [SerializeField] Fade fade;
    private float fadeTime = 1.0f;

    public void Start()
    {
        int state = PlayerPrefs.GetInt("TransitionState", 0);

        if (state == 1)
        {
            if (fade != null)
            {
                fade.ImageFill();   // ˆêu‚Å‰æ–Ê‚ğ^‚ÁˆÃ‚É‚·‚é
                fade.FadeOut(fadeTime); // 1•b‚©‚¯‚Ä–¾‚é‚­‚·‚é
            }
            PlayerPrefs.SetInt("TransitionState", 0);
            PlayerPrefs.Save();
        }
    }
}
