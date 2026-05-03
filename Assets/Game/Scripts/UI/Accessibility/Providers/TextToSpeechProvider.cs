using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextToSpeechProvider : MonoBehaviour, ITextToSpeechService
{
    [Header("Settings")]
    [SerializeField] private float wordsPerSecond = 2.5f;
    
    private Queue<string> _speechQueue = new Queue<string>();
    private bool _isSpeaking = false;

    public bool IsSpeaking => _isSpeaking;

    public void Speak(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        _speechQueue.Enqueue(text);
        if (!_isSpeaking) StartCoroutine(ProcessQueue());
    }

    public void Stop() 
    { 
        _speechQueue.Clear(); 
        _isSpeaking = false; 
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_ttsObject != null) _ttsObject.Call("stop");
#endif
    }

    private IEnumerator ProcessQueue()
    {
        _isSpeaking = true;
        while (_speechQueue.Count > 0)
        {
            string text = _speechQueue.Dequeue();
            ExecutePlatformSpeak(text);
            
            // Approximate wait time
            float duration = Mathf.Max(0.5f, text.Split(' ', '\n', '\t').Length / wordsPerSecond);
            yield return new WaitForSecondsRealtime(duration);
        }
        _isSpeaking = false;
    }

    private void ExecutePlatformSpeak(string text)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        SpeakAndroid(text);
#elif UNITY_IOS && !UNITY_EDITOR
        SpeakIOS(text);
#else
        Debug.Log($"[Editor TTS]: {text}");
#endif
    }

    // --- Android Implementation ---
#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject _ttsObject;
    private bool _isTtsReady = false;

    private void Awake()
    {
        InitializeTTS();
    }

    private void SpeakAndroid(string text)
    {
        if (!_isTtsReady) return;
        
        try
        {
            // QUEUE_ADD = 1
            _ttsObject.Call("speak", text, 1, null, null);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Android TTS Speak Error: {e.Message}");
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
                _isTtsReady = true; 
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Android TTS Initialization Failed: {e.Message}");
            _isTtsReady = false;
        }
    }

    private class TTSInitCallback : AndroidJavaProxy
    {
        public TTSInitCallback() : base("android.speech.tts.TextToSpeech$OnInitListener") { }
        public void onInit(int status) { }
    }
#endif

    // --- iOS Placeholder ---
#if UNITY_IOS && !UNITY_EDITOR
    private void SpeakIOS(string text)
    {
        // TODO: Implement iOS AVSpeechSynthesizer via Native Plugin or Objective-C bridge.
        // Example: iOSBridge.SpeakText(text);
        Debug.Log($"[iOS TTS Placeholder]: {text}");
    }
#endif
}