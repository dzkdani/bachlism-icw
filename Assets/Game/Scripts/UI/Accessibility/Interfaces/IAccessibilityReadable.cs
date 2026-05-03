using System.Collections;

/// <summary>
/// Implement this interface on any GameObject (like Cards) that should be read aloud.
/// </summary>
public interface IAccessibilityReadable
{
    /// <summary>
    /// Returns the text content to be spoken by the TTS engine.
    /// </summary>
    string GetAccessibilityText();
}