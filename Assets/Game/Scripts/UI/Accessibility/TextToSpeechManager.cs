using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Simple cross-platform Text-to-Speech manager.
/// Supports Android natively and logs to console in Editor/other platforms.
/// </summary>
public class TextToSpeechManager : MonoBehaviour
{
    public static TextToSpeechManager Instance { get; private set; }
    
    private Queue<string> _speechQueue = new Queue<string>();
    private bool _isSpeaking = false;
    private float _wordsPerSecond = 2.5f; // Average speaking rate

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Automatically subscribe to accessibility announcements
        AccessibilityManager ac = FindFirstObjectByType<AccessibilityManager>();
        if (ac != null)
            ac.OnAnnounce += Speak;
    }

    public void Speak(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        _speechQueue.Enqueue(text);
        if (!_isSpeaking) StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        _isSpeaking = true;
        while (_speechQueue.Count > 0)
        {
            string text = _speechQueue.Dequeue();
            
#if UNITY_ANDROID && !UNITY_EDITOR
            SpeakAndroid(text);
#elif UNITY_IOS && !UNITY_EDITOR
            // iOS TTS would require a native plugin or Unity's iOS SDK wrapper.
            // Placeholder for now.
            Debug.Log($"[iOS TTS]: {text}");
#else
            Debug.Log($"[TTS]: {text}");
#endif
            yield return new WaitForSecondsRealtime(CalculateDuration(text));
        }
        _isSpeaking = false;
    }

    private float CalculateDuration(string text)
    {
        int wordCount = text.Split(' ', '\n', '\t', '.', ',').Length;
        return Mathf.Max(0.5f, wordCount / _wordsPerSecond);
    }


#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject _ttsObject;
    private bool _isTtsReady = false;

    private void SpeakAndroid(string text)
    {
        if (!_isTtsReady) InitializeTTS();
        
        if (_isTtsReady && _ttsObject != null)
        {
            try
            {
                // QUEUE_ADD = 1
                _ttsObject.Call("speak", text, 1, null, null);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"TTS Speak Error: {e.Message}");
            }
        }
    }

    private void InitializeTTS()
    {
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                _ttsObject = new AndroidJavaObject("android.speech.tts.TextToSpeech", context, new TTSInitCallback());
                // Give it a moment to initialize
                _isTtsReady = true; 
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"TTS Initialization Failed: {e.Message}");
            _isTtsReady = false;
        }
    }

    private class TTSInitCallback : AndroidJavaProxy
    {
        public TTSInitCallback() : base("android.speech.tts.TextToSpeech$OnInitListener") { }
        public void onInit(int status)
        {
            // status 0 = SUCCESS. Could hook back to main thread if needed.
        }
    }
#endif
}