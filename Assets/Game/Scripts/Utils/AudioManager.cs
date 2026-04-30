using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [System.Serializable]
    public class AudioData
    {
        public string id;
        public AudioClip clip;
        public float volume = 1f;
        public bool loop = false;
    }

    [SerializeField] private List<AudioData> audioList = new List<AudioData>();
    private Dictionary<string, AudioData> audioDict = new Dictionary<string, AudioData>();
    private Dictionary<string, AudioSource> activeSources = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeAudioDict();
    }

    private void InitializeAudioDict()
    {
        audioDict.Clear();
        foreach (var audio in audioList)
        {
            if (!audioDict.ContainsKey(audio.id.ToLower()))
                audioDict[audio.id.ToLower()] = audio;
        }
    }

    public void Play(string id, bool loop = false)
    {
        if (!audioDict.TryGetValue(id.ToLower(), out var audioData))
        {
            Debug.LogWarning($"Audio '{id}' not found in AudioManager");
            return;
        }

        string sourceKey = $"{id}_{Time.frameCount}";
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = audioData.clip;
        source.volume = audioData.volume;
        source.loop = loop || audioData.loop;
        source.Play();

        activeSources[sourceKey] = source;
        
        if (!source.loop)
            Destroy(source, audioData.clip.length);
    }

    public void Stop(string id)
    {
        var keysToRemove = new List<string>();
        foreach (var kvp in activeSources)
        {
            if (kvp.Key.StartsWith(id.ToLower() + "_") && kvp.Value != null)
            {
                kvp.Value.Stop();
                Destroy(kvp.Value);
                keysToRemove.Add(kvp.Key);
            }
        }
        foreach (var key in keysToRemove)
            activeSources.Remove(key);
    }

    public void StopAll()
    {
        foreach (var source in activeSources.Values)
            if (source != null) Destroy(source);
        activeSources.Clear();
    }

    public void SetVolume(string id, float volume)
    {
        if (audioDict.TryGetValue(id.ToLower(), out var audioData))
            audioData.volume = Mathf.Clamp01(volume);
    }

    public bool IsPlaying(string id) => activeSources.Values.Count > 0;
}
