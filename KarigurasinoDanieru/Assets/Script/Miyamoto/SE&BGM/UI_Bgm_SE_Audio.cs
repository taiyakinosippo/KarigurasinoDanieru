using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class UI_Bgm_SE_Audio : MonoBehaviour
{
   public AudioMixer _audioMixer;
   public Slider _bgmSlider;
   public Slider _seSlider;
   public UnityEngine.UI.Image _bgmVolumeImage;
   public UnityEngine.UI.Image _seVolumeImage;
   public Audio_Mute _bgmButtonMute;
   public Audio_Mute _seButtonMute;
   public Sprite _volumeHighSprite;   // 100～70
   public Sprite _volumeMiddleSprite; // 69～40
   public Sprite _volumeLowSprite;    // 39～1
   public Sprite _volumeMuteSprite;   // 0
   const string BGM_KEY = "BGMVolume";
   const string SE_KEY = "SEVolume";
   private float _currentBgmValue;
   private float _currentSeValue;
   private bool _isBgmMuteImageActive;
   private bool _isSeMuteImageActive;
   private bool _isInitialized;
   private float _lastSEPlayTime;
   [SerializeField] private SEType _seSampleType = SEType.SelectbuttonSE;
   [SerializeField] private float _seSampleCooldown = 0.08f;

    void Start()
   {
        float _bgm = PlayerPrefs.GetFloat(BGM_KEY, 0.75f);
        float _se = PlayerPrefs.GetFloat(SE_KEY, 0.75f);

        //BGM&SE設定
        _bgmSlider.value = _bgm;
        _seSlider.value = _se;
  
        _bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        _seSlider.onValueChanged.AddListener(SetSEVolume);
        SetBGMVolume(_bgm);
        SetSEVolume(_se);
        _isInitialized = true;
    }
    public void SetBGMVolume(float value)
    {
        _currentBgmValue = value;
        bool isZero = value <= 0f;
        if (_isBgmMuteImageActive && !isZero)
        {
            _isBgmMuteImageActive = false;
        }

        if (!isZero && _bgmButtonMute != null && _bgmButtonMute.audioSource != null && _bgmButtonMute.audioSource.mute)
        {
            _bgmButtonMute.audioSource.mute = false;
        }

        float mixerValue = Mathf.Clamp(value, 0.0001f, 1f);
        _audioMixer.SetFloat(BGM_KEY, Mathf.Log10(mixerValue) * 20);
        PlayerPrefs.SetFloat(BGM_KEY, value);

        UpdateVolumeImage(_bgmVolumeImage, value, isZero);
    }

    public void SetSEVolume(float value)
    {
        _currentSeValue = value;
        bool isZero = value <= 0f;
        if (_isSeMuteImageActive && !isZero)
        {
            _isSeMuteImageActive = false;
        }

        if (!isZero && _seButtonMute != null && _seButtonMute.audioSource != null && _seButtonMute.audioSource.mute)
        {
            _seButtonMute.audioSource.mute = false;
        }

        float mixerValue = Mathf.Clamp(value, 0.0001f, 1f);
        _audioMixer.SetFloat(SE_KEY, Mathf.Log10(mixerValue) * 20);
        PlayerPrefs.SetFloat(SE_KEY, value);

        UpdateVolumeImage(_seVolumeImage, value, isZero);

        if (_isInitialized)
        {
            PlaySESample();
        }
    }

    private void PlaySESample()
    {
        if (AudioSourceManager.instance == null)
            return;

        if (Time.time - _lastSEPlayTime < _seSampleCooldown)
            return;

        _lastSEPlayTime = Time.time;
        AudioSourceManager.instance.PlaySE(_seSampleType);
    }

    public void ToggleBgmMuteImageState()
    {
        _isBgmMuteImageActive = !_isBgmMuteImageActive;
        if (_bgmVolumeImage == null)
            return;

        if (_isBgmMuteImageActive)
        {
            _bgmVolumeImage.sprite = _volumeMuteSprite;
        }
        else
        {
            UpdateVolumeImage(_bgmVolumeImage, _currentBgmValue, _currentBgmValue <= 0f);
        }
    }

    public void ToggleSeMuteImageState()
    {
        _isSeMuteImageActive = !_isSeMuteImageActive;
        if (_seVolumeImage == null)
            return;

        if (_isSeMuteImageActive)
        {
            _seVolumeImage.sprite = _volumeMuteSprite;
        }
        else
        {
            UpdateVolumeImage(_seVolumeImage, _currentSeValue, _currentSeValue <= 0f);
        }
    }

    private void UpdateVolumeImage(UnityEngine.UI.Image image, float value, bool isZero)
    {
        if (image == null)
            return;

        if (isZero)
        {
            image.sprite = _volumeMuteSprite;
            return;
        }

        int percent = Mathf.RoundToInt(Mathf.Clamp01(value) * 100f);
        if (percent >= 70)
        {
            image.sprite = _volumeHighSprite;
        }
        else if (percent >= 40)
        {
            image.sprite = _volumeMiddleSprite;
        }
        else
        {
            image.sprite = _volumeLowSprite;
        }
    }
}
