using UnityEngine;
using TMPro;
public class Timer : MonoBehaviour
{
  [SerializeField]private float TimeLimit = 0f;
    private float currentTime = 0f;
  public TextMeshProUGUI timeText;
  [HideInInspector] public bool isTimeStop= false;
  [HideInInspector] private bool isjump = false;

    private void Awake()
    {
        currentTime = TimeLimit;
    }
    void Update()
    {
        timeText.enabled = false;
        if (isTimeStop) return;
        timeText.enabled = true;
        if (currentTime > 0)
        {
            TimeLimit -= Time.deltaTime;
            currentTime = TimeLimit;
            timeText.text = Mathf.Ceil(currentTime).ToString();
        }
        else if(!isjump)
        {
            isjump = true;
        }
    }
}

