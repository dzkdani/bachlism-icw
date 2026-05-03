using System.Collections;

public enum VibrationType { Default, Light, Heavy, Success, Warning }

public interface IHapticService
{
    void Vibrate(VibrationType type = VibrationType.Default);
}