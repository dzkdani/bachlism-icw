using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Static extension class for TextMeshPro typewriter text effect.
/// Provides easy-to-use methods to animate text character-by-character with customizable speed.
/// </summary>
public static class TypewriterExtensions
{
    private const string TypewriterCoroutineKey = "TypewriterCoroutine_";
    private const string TypewriterDelayCoroutineKey = "TypewriterDelayCoroutine_";

    /// <summary>
    /// Plays a typewriter effect on the TextMeshPro component.
    /// </summary>
    /// <param name="tmpro">The TextMeshPro component to apply the effect to.</param>
    /// <param name="fullText">The complete text to type out.</param>
    /// <param name="charsPerSecond">Characters to display per second (default: 25).</param>
    public static void PlayTypewriter(this TextMeshProUGUI tmpro, string fullText, float charsPerSecond = 25f)
    {
        if (tmpro == null) return;
        
        tmpro.StopTypewriter();
        tmpro.text = "";
        
        IEnumerator coroutine = TypewriterCoroutine(tmpro, fullText, charsPerSecond);
        tmpro.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Plays a typewriter effect with a direct character delay (in seconds).
    /// </summary>
    /// <param name="tmpro">The TextMeshPro component to apply the effect to.</param>
    /// <param name="fullText">The complete text to type out.</param>
    /// <param name="charDelaySeconds">Delay in seconds between each character (default: 0.04).</param>
    public static void PlayTypewriterByDelay(this TextMeshProUGUI tmpro, string fullText, float charDelaySeconds = 0.04f)
    {
        if (tmpro == null) return;
        
        tmpro.StopTypewriter();
        tmpro.text = "";
        
        IEnumerator coroutine = TypewriterDelayCoroutine(tmpro, fullText, charDelaySeconds);
        tmpro.StartCoroutine(coroutine);
    }

    public static void PlayTypewriterWithMark(this TextMeshProUGUI tmpro, string fullText, float charsPerSecond = 25f)
    {
        if (tmpro == null) return;
        
        tmpro.StopTypewriter();
        tmpro.text = "";
        
        IEnumerator coroutine = TypewriterMarkCoroutine(tmpro, fullText, 1f / charsPerSecond);
        tmpro.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Stops any active typewriter effect on this TextMeshPro component.
    /// </summary>
    public static void StopTypewriter(this TextMeshProUGUI tmpro)
    {
        if (tmpro == null) return;
        tmpro.StopAllCoroutines();
    }

    /// <summary>
    /// Immediately shows all text and stops the typewriter effect.
    /// </summary>
    public static void SkipTypewriter(this TextMeshProUGUI tmpro, string fullText)
    {
        if (tmpro == null) return;
        
        tmpro.StopTypewriter();
        tmpro.text = fullText;
    }


    // ============ Private Coroutines ============
    
    /// Coroutine that types out text character by character with a <mark> effect.
    private static IEnumerator TypewriterMarkCoroutine(TextMeshProUGUI tmpro, string fullText, float delay)
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            string visible = fullText.Substring(0, i + 1);
            tmpro.text = $"<mark=#00000088>{visible}</mark>";

            yield return new WaitForSeconds(delay);
        }
    }

    /// <summary>
    /// Coroutine that types out text character by character based on characters per second.
    /// </summary>
    private static IEnumerator TypewriterCoroutine(TextMeshProUGUI tmpro, string fullText, float charsPerSecond)
    {
        float delayBetweenChars = 1f / charsPerSecond;
        tmpro.text = "";
        
        for (int i = 0; i < fullText.Length; i++)
        {
            tmpro.text += fullText[i];
            yield return new WaitForSeconds(delayBetweenChars);
        }
    }

    /// <summary>
    /// Coroutine that types out text character by character based on a fixed delay.
    /// </summary>
    private static IEnumerator TypewriterDelayCoroutine(TextMeshProUGUI tmpro, string fullText, float charDelaySeconds)
    {
        tmpro.text = "";
        
        for (int i = 0; i < fullText.Length; i++)
        {
            tmpro.text += fullText[i];
            yield return new WaitForSeconds(charDelaySeconds);
        }
    }
}
