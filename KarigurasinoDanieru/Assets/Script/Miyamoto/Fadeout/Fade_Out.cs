using UnityEngine;

public class Fade_Out : MonoBehaviour
{
    [SerializeField] Fade fade;
    [SerializeField] Timer timer;
    public void Start()
    {
        if (timer != null)
        {
            timer.SetTimerRunning(false);
        }

        fade.FadeOut(1f, () =>
        {
            if (timer != null)
            {
                timer.SetTimerRunning(true);
            }
        });
    }
}
