using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ModeManager : MonoBehaviour
{
    public GameObject GamePlay;
    public GameObject PlayMode;
    public GameObject Matching;

    public InputField MultiRoomInput;
    public Button MatchingButton;
    public GameObject MultiRoomInputField;
    public GameObject MatchingButton_obj;

    public Text MatchStatusText; 

    [HideInInspector]
    public static string CurrentRoomId;

    public static bool IsMultiMode = false;
    public bool matchHandled = false;

    void Start()
    {
        GamePlay.SetActive(false);
        Matching.SetActive(false);
        PlayMode.SetActive(true);

        MatchStatusText.gameObject.SetActive(false);
        MatchingButton.interactable = false;

        MultiRoomInput.onValueChanged.AddListener(OnNameChanged);
        MultiRoomInput.onEndEdit.AddListener(OnNameChanged);
    }

    void OnNameChanged(string input)
    {
        MatchingButton.interactable = !string.IsNullOrWhiteSpace(input);
    }

    public void InputSingle()
    {
        IsMultiMode = false;
        PlayMode.SetActive(false);
        Matching.SetActive(false);
        GamePlay.SetActive(true);
    }

    public void InputMulti()
    {
        IsMultiMode = true;

        PlayMode.SetActive(false);
        Matching.SetActive(true);
        GamePlay.SetActive(false);

        MultiRoomInputField.SetActive(true);
        MatchingButton_obj.SetActive(true);
        // 待機中表示
        MatchStatusText.gameObject.SetActive(true);
        MatchStatusText.text = "Waiting...";
    }

    public void InputMatching()
    {
        MultiRoomInputField.SetActive(false);
        MatchingButton_obj.SetActive(false);

        CurrentRoomId = MultiRoomInput.text.Trim();
    }

    /// <summary>
    /// マッチ成功を通知される
    /// </summary>
    public void OnMatchSuccess(string opponentName)
    {
        if (matchHandled) return;
        matchHandled = true;

        StartCoroutine(MatchSuccessSequence(opponentName));
    }


    IEnumerator MatchSuccessSequence(string opponentName)
    {
        // マッチ成功表示
        MatchStatusText.text =opponentName+" Matching!";

        // 2秒待機
        yield return new WaitForSeconds(2f);

        MatchStatusText.gameObject.SetActive(false);
        Matching.SetActive(false);
        GamePlay.SetActive(true);
    }

    public void InputBack()
    {
        PlayMode.SetActive(true);
        Matching.SetActive(false);
        GamePlay.SetActive(false);
    }
}