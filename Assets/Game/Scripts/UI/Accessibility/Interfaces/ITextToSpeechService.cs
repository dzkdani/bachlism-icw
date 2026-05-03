using System.Collections;

public interface ITextToSpeechService
{
    void Speak(string text);
    void Stop();
    bool IsSpeaking { get; }
}