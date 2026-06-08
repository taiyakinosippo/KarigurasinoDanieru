using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGM_Manager : MonoBehaviour
{
    public static BGM_Manager Instance;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip waitingtimeBGM;
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
                PlayBGM(titleBGM, true);
                break;

            case "Main":
                PlayWaitingtimeBGM();
                break;

            default:
                StopBGM();
                break;
        }
    }

    public void PlayGameBGM()
    {
        PlayBGM(gameBGM, false);
    }

    public void PlayWaitingtimeBGM()
    {
        PlayBGM(waitingtimeBGM, true);
    }

    public void PlayRocketBGM()
    {
        PlayBGM(rocketFlyBGM, true);
    }

    public void PlayResultBGM()
    {
        PlayBGM(resultBGM, true);
    }

    private void StopBGM()
    {
        bgmSource.Stop();

        bgmSource.clip = null;
    }

    private void PlayBGM(AudioClip clip, bool loop)
    {
        if (bgmSource.clip == clip && bgmSource.loop == loop)
            return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

}
