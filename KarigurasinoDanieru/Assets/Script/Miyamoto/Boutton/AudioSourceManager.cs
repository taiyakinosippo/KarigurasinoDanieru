using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
   public static AudioSourceManager instance;
   private AudioSource _audioSource;

   [Header("SEˆê——")]
    public AudioClip _indSE;
    public AudioClip _backSE;
    public AudioClip _clickSE;

    void Awake()
    {
        instance = this;
        _audioSource = GetComponent<AudioSource>();
    }
    public void PlaySE(SEType seType)
    {
        switch (seType)
        {
            case SEType.indicatorSE:
                _audioSource.PlayOneShot(_indSE);
                break;
            case SEType.backbuttonSE:
                _audioSource.PlayOneShot(_backSE);
                break;
            case SEType.clickSE:
                _audioSource.PlayOneShot(_clickSE);
                break;
        }
    }
}