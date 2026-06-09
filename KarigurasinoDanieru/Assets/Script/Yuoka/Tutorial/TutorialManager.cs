using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialImages;

    private int currentIndex = 0;
    private bool canClick = false;

    IEnumerator Start()
    {
        if (PlayerPrefs.GetInt("TutorialState") != 1)
        {
            PlayerPrefs.SetInt("TransitionState", 0);
            PlayerPrefs.Save();

            Destroy(gameObject);
            yield break;
        }

        // 1•b‘Ò‚Â
        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 0f;

        for (int i = 0; i < tutorialImages.Length; i++)
        {
            tutorialImages[i].SetActive(i == 0);
        }

        canClick = true;
    }

    void Update()
    {
        if (!canClick) return;

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            NextTutorial();
        }
    }

    void NextTutorial()
    {
        tutorialImages[currentIndex].SetActive(false);

        currentIndex++;

        if (currentIndex >= tutorialImages.Length)
        {
            EndTutorial();
            return;
        }

        tutorialImages[currentIndex].SetActive(true);
    }

    void EndTutorial()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}