using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGM_Manager : MonoBehaviour
{
    public static BGM_Manager Instance;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip gameBGM;
    [SerializeField] private AudioClip rocketFlyBGM;
    public AudioClip RocketFlyBGM => rocketFlyBGM;
    [SerializeField] private AudioClip resultBGM;
    public AudioClip ResultBGM => resultBGM;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Title":
                PlayBGM(titleBGM);
                break;

            default:
                StopBGM();
                break;
        }
    }

    public void PlayGameBGM()
    {
        PlayBGM(gameBGM);
    }

    public void PlayRocketBGM()
    {
        PlayBGM(rocketFlyBGM);
    }

    public void PlayResultBGM()
    {
        PlayBGM(resultBGM);
    }

    private void StopBGM()
    {
        bgmSource.Stop();

        bgmSource.clip = null;
    }

    private void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip)
            return;

        bgmSource.clip = clip;
        bgmSource.Play();
    }

}
