using UnityEngine;

/// <summary>
/// Detects touch gestures to trigger Accessibility actions.
/// This allows blind users to play without needing to see on-screen buttons.
/// </summary>
[RequireComponent(typeof(Collider2D))] // Or attach to Canvas/Image with RaycastTarget
public class AccessibilityGestures : MonoBehaviour
{
    [Header("Timing Thresholds (seconds)")]
    [SerializeField] private float doubleTapThreshold = 0.3f;
    [SerializeField] private float longPressThreshold = 0.5f;
    [SerializeField] private float swipeThreshold = 50f; // pixels

    private float _lastTapTime;
    private Vector2 _touchStartPos;
    private float _touchStartTime;

    private void Update()
    {
        if (!AccessibilityManager.Instance.IsEnabled) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            HandleTouch(touch);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            // Fallback for Editor mouse testing
            _touchStartPos = Input.mousePosition;
            _touchStartTime = Time.time;
            
            // Check Double Tap
            if (Time.time - _lastTapTime < doubleTapThreshold)
            {
                AccessibilityManager.Instance.OnButtonReadCard();
                _lastTapTime = 0;
            }
            else
            {
                _lastTapTime = Time.time;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Long Press Check
            if (Time.time - _touchStartTime > longPressThreshold)
            {
                AccessibilityManager.Instance.OnButtonFocusNext();
            }
            else
            {
                // Swipe Check
                Vector2 diff = (Vector2)Input.mousePosition - _touchStartPos;
                if (Mathf.Abs(diff.x) > swipeThreshold)
                {
                    if (diff.x > 0) AccessibilityManager.Instance.OnButtonSwipeRight();
                    else AccessibilityManager.Instance.OnButtonSwipeLeft();
                }
            }
        }
    }

    private void HandleTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                _touchStartPos = touch.position;
                _touchStartTime = Time.time;

                if (Time.time - _lastTapTime < doubleTapThreshold)
                {
                    AccessibilityManager.Instance.OnButtonReadCard();
                    _lastTapTime = 0;
                }
                else
                {
                    _lastTapTime = Time.time;
                }
                break;

            case TouchPhase.Ended:
                if (Time.time - _touchStartTime > longPressThreshold)
                {
                    AccessibilityManager.Instance.OnButtonFocusNext();
                }
                else
                {
                    Vector2 diff = touch.position - _touchStartPos;
                    if (Mathf.Abs(diff.x) > swipeThreshold)
                    {
                        if (diff.x > 0) AccessibilityManager.Instance.OnButtonSwipeRight();
                        else AccessibilityManager.Instance.OnButtonSwipeLeft();
                    }
                }
                break;
        }
    }
}