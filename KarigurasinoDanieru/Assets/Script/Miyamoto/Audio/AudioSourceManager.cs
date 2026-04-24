using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioSourceManager : MonoBehaviour
{
    public static AudioSourceManager instance;
    private AudioSource _audioSource;

    [Header("SEàÍóó")]
    [SerializeField] private List<SEData> seList;
    private Dictionary<SEType, AudioClip> seDictionary;

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

        foreach (var se in seList)
        {
            if (!seDictionary.ContainsKey(se.seType))
            {
                seDictionary.Add(se.seType, se.audioClip);
            }
            else
            {
                Debug.LogWarning($"SETypeÇ™èdï°ÇµÇƒÇÈ: {se.seType}");
            }
        }
    }
    public void PlaySE(SEType seType)
    {
        if (seDictionary.TryGetValue(seType, out AudioClip clip))
        {
            _audioSource.PlayOneShot(clip);
        }
    }

}
