
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AudioItem
{
    public AudioType name;

    public AudioClip clip;
    [Range(0f, 128f)]
    public float volume;
}


public sealed class AudioController : EventTarget
{
    [SerializeField] private List<AudioItem> audioItems = new();

    private Dictionary<AudioType, AudioSource> _audioItemsDict = new();

    private void Awake()
    {
        foreach (var item in audioItems)
        {
            var audioSource = Instantiate(new GameObject("sfx"), transform).AddComponent<AudioSource>();
            audioSource.clip = item.clip;
            audioSource.volume = item.volume;
            _audioItemsDict[item.name] = audioSource;
        }
    }

    public void Play(AudioType name, bool loop = false)
    {
        if (_audioItemsDict.TryGetValue(name, out AudioSource audioSource))
        {
            audioSource.loop = loop;
            audioSource.Play();
        }
    }

    public void Stop(AudioType name)
    {
        if (_audioItemsDict.TryGetValue(name, out AudioSource audioSource))
        {
            audioSource.Stop();
        }
    }
}