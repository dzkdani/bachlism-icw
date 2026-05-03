using UnityEngine;

public class HapticProvider : MonoBehaviour, IHapticService
{
    public void Vibrate(VibrationType type = VibrationType.Default)
    {
        // Basic cross-platform vibration
        Handheld.Vibrate();

        // TODO: For advanced iOS Haptics (CoreHaptics), 
        // you would check the type and call specific iOS bridges here.
    }
}