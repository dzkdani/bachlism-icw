using UnityEngine;
using System;

public class AccessibilityManager : MonoBehaviour
{
    public static AccessibilityManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private bool enableAccessibilityMode = false;
    public bool IsEnabled => enableAccessibilityMode;

    [Header("Input Keys")]
    [SerializeField] private KeyCode toggleAccessibilityKey = KeyCode.F1;
    [SerializeField] private KeyCode leftSwipeKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode rightSwipeKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode cancelSwipeKey = KeyCode.Escape;
    [SerializeField] private KeyCode nextCardKey = KeyCode.Tab;
    [SerializeField] private KeyCode readCardKey = KeyCode.R;

    [Header("Audio")]
    [SerializeField] private string focusSoundId = "focus";
    [SerializeField] private string swipeLeftSoundId = "swipe";
    [SerializeField] private string swipeRightSoundId = "swipe";
    [SerializeField] private string cancelSoundId = "drop";

    [Header("UI")]
    [SerializeField] private GameObject accessibilityOverlay;

    public event Action<InputHandler, InputManager.DropZone> OnAccessibilityAction;
    public event Action<string> OnAnnounce;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // AUTO-DETECTION LOGIC
        if (IsDeviceAccessibilityEnabled())
        {
            SetMode(true);
            Debug.Log("[Accessibility] System accessibility detected. Mode auto-enabled.");
        }
    }

    /// <summary>
    /// Checks if Android system accessibility services (like TalkBack) are enabled.
    /// </summary>
    private bool IsDeviceAccessibilityEnabled()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        { 
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var accessibilityManager = activity.Call<AndroidJavaObject>("getSystemService", "accessibility"))
                {
                    // "isTouchExplorationEnabled" is true when TalkBack/Screen Reader is active
                    return accessibilityManager.Call<bool>("isTouchExplorationEnabled");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to check accessibility status: {e.Message}");
            return false;
        }
#else
        // For PC or iOS, return false (or true if you are manually testing)
        return false;
#endif
    }

    /// <summary>
    /// Centralized method to trigger announcements.
    /// </summary>
    public void Announce(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            OnAnnounce?.Invoke(text);
        }
    }

    // We only keep keyboard input for PC Testing (Editor). 
    // On a real Android build, these lines are ignored.
    private void Update()
    {
    #if UNITY_EDITOR
        if (Input.GetKeyDown(toggleAccessibilityKey)) ToggleMode();
        if (!enableAccessibilityMode) return;

        InputHandler activeHandler = InputManager.Instance?.ActiveHandler;
        if (activeHandler != null)
        {
            if (Input.GetKeyDown(leftSwipeKey)) TriggerAction(InputManager.DropZone.Left);
            else if (Input.GetKeyDown(rightSwipeKey)) TriggerAction(InputManager.DropZone.Right);
            else if (Input.GetKeyDown(cancelSwipeKey)) TriggerAction(InputManager.DropZone.None);
            else if (Input.GetKeyDown(readCardKey)) activeHandler.ReadCard();
        }
        if (Input.GetKeyDown(nextCardKey)) FocusNextCard();
    #endif
    }

    private void TriggerAction(InputManager.DropZone zone)
    {
        InputHandler active = InputManager.Instance?.ActiveHandler;
        if (active == null) return;

        string soundId = zone switch
        {
            InputManager.DropZone.Left => swipeLeftSoundId,
            InputManager.DropZone.Right => swipeRightSoundId,
            _ => cancelSoundId
        };

        AudioManager.Instance?.Play(soundId);
        
        string announceText = zone switch
        {
            InputManager.DropZone.Left => "Swiped left",
            InputManager.DropZone.Right => "Swiped right",
            _ => "Cancelled"
        };
        OnAnnounce?.Invoke(announceText);

        // Add haptic feedback
        Handheld.Vibrate(); 

        OnAccessibilityAction?.Invoke(active, zone);
        active.EndDrag(zone);
    }

    private void FocusNextCard()
    {
        // Find the topmost active InputHandler (assumes top card has sibling index 0)
        InputHandler[] handlers = FindObjectsByType<InputHandler>(FindObjectsSortMode.InstanceID);
        foreach (var h in handlers)
        {
            if (h.isActiveAndEnabled && h.GetComponent<RectTransform>().GetSiblingIndex() == 0)
            {
                InputManager.Instance?.SetFocus(h);
                AudioManager.Instance?.Play(focusSoundId);
                return;
            }
        }
    }

    public void ToggleMode()
    {
        SetMode(!enableAccessibilityMode);
    }

    private void SetMode(bool enabled)
    {
        enableAccessibilityMode = enabled;
        
        // Enable/Disable the overlay UI
        if (accessibilityOverlay != null)
            accessibilityOverlay.SetActive(enabled);

        // Tactile feedback for mode change
        Handheld.Vibrate();

        OnAnnounce?.Invoke(enabled ? "Accessibility mode enabled" : "Accessibility mode disabled");
    }

    public void OnButtonSwipeLeft()
    {
        if (!enableAccessibilityMode) return;
        TriggerAction(InputManager.DropZone.Left);
    }

    public void OnButtonSwipeRight()
    {
        if (!enableAccessibilityMode) return;
        TriggerAction(InputManager.DropZone.Right);
    }

    /// <summary>
    /// Public method called by Gestures to focus the next card.
    /// </summary>
    public void OnButtonFocusNext()
    {
        if (!enableAccessibilityMode) return;
        FocusNextCard();
    }

    // Assign these to your UI Buttons' "On Click ()" events in the Inspector
    public void OnButtonToggle()
    {
        ToggleMode();
    }

    /// <summary>
    /// Public method to be called by a UI Button (On Click event)
    /// </summary>
    public void OnButtonReadCard()
    {
        if (!enableAccessibilityMode) return;
        
        InputHandler handler = InputManager.Instance?.ActiveHandler;
        if (handler != null)
        {
            handler.ReadCard(); // Reads text via TTS
            Handheld.Vibrate(); // Vibrate to confirm "I'm listening"
        }
        else
        {
            // Announce if no card is selected
            Announce("No card focused. Use the menu to select a card.");
        }
    }
}