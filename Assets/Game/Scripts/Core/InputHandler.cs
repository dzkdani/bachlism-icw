using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private float returnDuration = 0.4f;
    [SerializeField] private float maxRotation = 20f;
    
    private RectTransform draggedCardRect;
    private Vector2 originalPosition;
    private Quaternion originalRotation;
    private Canvas canvas;
    private float canvasMiddleX = 0f;
    private float canvasMiddleSides;

    private void OnEnable()
    {
        // Subscribe to drag and drop events
        InputManager.Instance.OnDragStarted += HandleDragStarted;
        InputManager.Instance.OnDragging += HandleDragging;
        InputManager.Instance.OnDropped += HandleDropped;
        InputManager.Instance.OnDragCancelled += HandleDragCancelled;
    }

    private void OnDisable()
    {
        // Unsubscribe from events to prevent memory leaks
        InputManager.Instance.OnDragStarted -= HandleDragStarted;
        InputManager.Instance.OnDragging -= HandleDragging;
        InputManager.Instance.OnDropped -= HandleDropped;
        InputManager.Instance.OnDragCancelled -= HandleDragCancelled;
    }

    private void Start()
    {
        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
        }
        canvasMiddleX = 0f; // Middle point in canvas local coordinates
        canvasMiddleSides = canvas.GetComponent<RectTransform>().rect.width / 4f; // Distance from middle to card starting position
    }

    private void HandleDragStarted(Vector2 startPos)
    {
        // Use GraphicRaycaster to find the topmost UI element under the cursor
        GraphicRaycaster raycaster = FindAnyObjectByType<GraphicRaycaster>();
        if (raycaster == null) return;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = startPos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        // Find the first Image component in the raycast results (topmost)
        foreach (RaycastResult result in results)
        {
            Image cardImage = result.gameObject.GetComponent<Image>();
            if (cardImage != null)
            {
                draggedCardRect = cardImage.GetComponent<RectTransform>();
                originalPosition = draggedCardRect.anchoredPosition;
                originalRotation = draggedCardRect.localRotation;
                Debug.Log($"Drag started on: {result.gameObject.name}");
                return;
            }
        }
    }

    private void HandleDragging(Vector2 currentPos, InputManager.DropZone zone)
    {
        if (draggedCardRect == null)
            return;

        // Convert screen position to canvas-relative position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            draggedCardRect.parent as RectTransform,
            currentPos,
            null,
            out Vector2 localPos
        );

        // Update card position to follow drag
        draggedCardRect.anchoredPosition = localPos;

        // Calculate distance from original position to canvas middle
        float distanceToMiddle = Mathf.Abs(localPos.x - originalPosition.x);
        
        // Calculate max distance (from original pos to middle of canvas)
        float maxDistance = Mathf.Abs(canvasMiddleX - canvasMiddleSides); // Adjusted for card starting position
        
        // Clamp the ratio between 0 and 1
        float lerpRatio = Mathf.Clamp01(distanceToMiddle / maxDistance);

        // Calculate rotation based on zone and lerp ratio
        float targetRotation = 0f;
        switch (zone)
        {
            case InputManager.DropZone.Left:
                targetRotation = Mathf.Lerp(0f, maxRotation, lerpRatio); // 0 to +30
                break;
            case InputManager.DropZone.Right:
                targetRotation = Mathf.Lerp(0f, -maxRotation, lerpRatio); // 0 to -30
                break;
            case InputManager.DropZone.None:
                targetRotation = 0f;
                draggedCardRect.localScale = Vector3.one;
                break;
        }

        // Apply rotation around Z axis
        draggedCardRect.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, targetRotation), 0.1f).SetEase(Ease.OutCubic);
        // draggedCardRect.localRotation = Quaternion.Euler(0, 0, targetRotation);

        Debug.Log($"Dragging - Zone: {zone}, Position: {localPos}, Rotation: {targetRotation}, LerpRatio: {distanceToMiddle / maxDistance}");
    }

    private void HandleDropped(InputManager.DropZone zone)
    {
        if (draggedCardRect == null)
            return;

        Debug.Log($"Card dropped in zone: {zone}");

        // Animate back to original position and rotation
        Sequence returnSequence = DOTween.Sequence();
        returnSequence.Append(draggedCardRect.DOAnchorPos(originalPosition, returnDuration).SetEase(Ease.OutCubic));
        returnSequence.Join(draggedCardRect.DOLocalRotateQuaternion(originalRotation, returnDuration).SetEase(Ease.OutCubic));
        returnSequence.Join(draggedCardRect.DOScale(Vector3.one, returnDuration).SetEase(Ease.OutCubic));
        returnSequence.OnComplete(() =>
        {
            draggedCardRect = null;
        });

        // Handle zone-specific logic
        switch (zone)
        {
            case InputManager.DropZone.Left:
                Debug.Log("Trigger LEFT action!");
                break;

            case InputManager.DropZone.Right:
                Debug.Log("Trigger RIGHT action!");
                break;

            case InputManager.DropZone.None:
                Debug.Log("Released outside zones");
                break;
        }
    }

    private void HandleDragCancelled()
    {
        if (draggedCardRect == null)
            return;

        Debug.Log("Drag cancelled");

        // Animate back to original position and rotation
        Sequence returnSequence = DOTween.Sequence();
        returnSequence.Append(draggedCardRect.DOAnchorPos(originalPosition, returnDuration).SetEase(Ease.OutCubic));
        returnSequence.Join(draggedCardRect.DOLocalRotateQuaternion(originalRotation, returnDuration).SetEase(Ease.OutCubic));
        returnSequence.Join(draggedCardRect.DOScale(Vector3.one, returnDuration).SetEase(Ease.OutCubic));
        returnSequence.OnComplete(() =>
        {
            draggedCardRect = null;
        });
    }
}
