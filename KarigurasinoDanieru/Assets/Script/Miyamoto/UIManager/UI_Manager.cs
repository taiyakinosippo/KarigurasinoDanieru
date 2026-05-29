using TMPro;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UI_Manager : MonoBehaviour
{
   public static UI_Manager instance;
   [SerializeField] private ScoreController scoreController; //�X�R�A�̃v���[���e�[�V�������Ǘ����Ă���
   [SerializeField] private TextMeshProUGUI scoreText;                 //�X�R�A�̃e�L�X�g
    public static Action OnCountFinished;
    private readonly Dictionary<Canvas, int> _pendingCloseRequests = new Dictionary<Canvas, int>();
    private void Awake()
   {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //�X�R�A�̍X�V
    public void StartScoreEvent()
    {
        scoreController.OnScoreChanged +=
            UpdateScoreText;

        scoreController.OnFinished +=
            FinishText;
    }

    // ========================================
    // �����ł̓X�R�A�̃e�L�X�g�̍X�V���s��
    // ========================================

    private void UpdateScoreText(float score)
    {
        scoreText.text =
            score.ToString("N2")
            + "m";
    }

    // ========================================
    // �������I�������Ƃ��̃e�L�X�g�̍X�V
    // ========================================

    private void FinishText()
    {
        Debug.Log("�X�R�A�̃v���[���e�[�V�������I�����܂����B");
        scoreText.text = ScoreManager.instance
            .GetScore()
            .ToString("N2")
            + "m";
            OnCountFinished?.Invoke();
    }

    /// <summary>
    /// UI��\������
    ///</summary>
    public void ShowUI(Canvas target)
    {
        _pendingCloseRequests.Remove(target);
        target.enabled = true;
    }

    /// <summary>
    /// UI���\���ɂ���
    ///</summary>
    public void CloseUI(Canvas target)
    {
        _pendingCloseRequests.Remove(target);
        target.enabled = false;
    }

    public void ScheduleCloseUI(Canvas target, float delay)
    {
        int requestId = 1;
        if (_pendingCloseRequests.TryGetValue(target, out int existingId))
        {
            requestId = existingId + 1;
        }

        _pendingCloseRequests[target] = requestId;
        StartCoroutine(CloseUIAfterDelay(target, delay, requestId));
    }

    private IEnumerator CloseUIAfterDelay(Canvas target, float delay, int requestId)
    {
        yield return new WaitForSeconds(delay);

        if (_pendingCloseRequests.TryGetValue(target, out int currentId) && currentId == requestId)
        {
            _pendingCloseRequests.Remove(target);
            target.enabled = false;
        }
    }

    public void UIManagerGetComponents()
    {
        
        if(scoreController == null)
        {
            scoreController = FindAnyObjectByType<ScoreController>();
        }
        if(scoreText == null)
        {
            scoreText = scoreText = GameObject.Find("ScoreText") .GetComponent<TextMeshProUGUI>();
        }
    }
}

