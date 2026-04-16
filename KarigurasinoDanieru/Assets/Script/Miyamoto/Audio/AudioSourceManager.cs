using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
   public static AudioSourceManager instance;
   private AudioSource _audioSource;

   [Header("SEˆê——")]
   [SerializeField]public AudioClip _indSE;
   [SerializeField]public AudioClip _backSE;
   [SerializeField]public AudioClip _clickSE;
   [SerializeField]public AudioClip _missSE;
   [SerializeField]public AudioClip _goodSE;
   [SerializeField]public AudioClip _greatSE;
   [SerializeField]public AudioClip _perfectSE;

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
            case SEType.missSE:
                _audioSource.PlayOneShot(_missSE);
                break;
            case SEType.goodSE: 
                _audioSource.PlayOneShot(_goodSE);
                break;
            case SEType.greatSE: 
                _audioSource.PlayOneShot(_greatSE);
                break;
            case SEType.perfectSE:
                _audioSource.PlayOneShot(_perfectSE);
                break;
        }
    }
}