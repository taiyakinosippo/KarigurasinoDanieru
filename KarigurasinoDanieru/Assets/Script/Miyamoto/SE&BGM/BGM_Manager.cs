using System.ComponentModel;
using UnityEngine;

public class BGM_Manager : MonoBehaviour
{
    public static BGM_Manager Instance;
    [SerializeField] private AudioClip TitleBGM;
    [SerializeField] private AudioClip GameBGM;
    [SerializeField] private AudioClip RocketFlyBGM;
    [SerializeField] private AudioClip ResultBGM;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameBGMStart()
    {
       
    }


}
