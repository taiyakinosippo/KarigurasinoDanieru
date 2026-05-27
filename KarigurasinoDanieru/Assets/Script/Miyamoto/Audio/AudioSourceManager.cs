using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioSourceManager : MonoBehaviour
{
    public static AudioSourceManager instance;          //インスタンス化
    private AudioSource _audioSource;　　　　　　　    
    [Header("SE一覧")]
    [SerializeField] private List<SEData> seList;　    //SEのリストをインスペクターで設定するための変数
    private Dictionary<SEType, AudioClip> seDictionary;// SETypeから高速にAudioClipを取得するための辞書

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        seDictionary = new Dictionary<SEType, AudioClip>();
        // SEリストから辞書を作成
        foreach (var se in seList)
        {
            if (!seDictionary.ContainsKey(se.seType))
            {
                seDictionary.Add(se.seType, se.audioClip);
            }
            else
            {
                Debug.LogWarning($"SETypeが重複してる: {se.seType}");
            }
        }
    }
    /// <summary>
    /// 音を再生するための関数。引数でSETypeを受け取り、対応するAudioClipを再生する。
    /// </summary>

    public void PlaySE(SEType seType)
    {
        if (seDictionary.TryGetValue(seType, out AudioClip clip))
        {
            _audioSource.PlayOneShot(clip);
        }
    }

}
