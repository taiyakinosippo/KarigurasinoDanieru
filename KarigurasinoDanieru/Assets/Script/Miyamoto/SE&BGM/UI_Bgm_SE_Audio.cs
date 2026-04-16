using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class UI_Bgm_SE_Audio : MonoBehaviour
{
   public AudioMixer _audioMixer;
   public Slider _bgmSlider;
   public Slider _seSlider;
   const string BGM_KEY = "BGMVolume";
   const string SE_KEY = "SEVolume";

    void Start()
   {
        float _bgm = PlayerPrefs.GetFloat(BGM_KEY, 0.75f);
        float _se = PlayerPrefs.GetFloat(SE_KEY, 0.75f);

        //BGM&SEÇÃèâä˙ê›íË
        _bgmSlider.value = _bgm;
        _seSlider.value = _se;
  
        _bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        _seSlider.onValueChanged.AddListener(SetSEVolume);
        SetBGMVolume(_bgm);
        SetSEVolume(_se);
    }
    public void SetBGMVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        _audioMixer.SetFloat(BGM_KEY, Mathf.Log10(value) * 20);
         PlayerPrefs.SetFloat(BGM_KEY, value);
    }

    public void SetSEVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        _audioMixer.SetFloat(SE_KEY, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(SE_KEY, value);
    }
}
