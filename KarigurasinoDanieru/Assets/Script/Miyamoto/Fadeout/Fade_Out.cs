using UnityEngine;

public class Fade_Out : MonoBehaviour
{
    [SerializeField] Fade fade;
    [SerializeField] Timer timer;
    public void Start()
    {
        timer.isTimeStop = true;

        fade.FadeOut(1f, () =>
        {
            timer.isTimeStop = false;
        });
    }
}
