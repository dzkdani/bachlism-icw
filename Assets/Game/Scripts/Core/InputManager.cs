using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public enum DropZone { Left, Right, None }

    [Header("Drag Settings")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private float canvasMiddleXOffset = 40f; // Offset from middle to create a dead zone 
    [SerializeField] private float dropOffset = 20f; // Minimum distance from middle to count as a drop
    
    private Vector2 dragStartPos;
    private Vector2 currentDragPos;
    private bool isDragging;
    private DropZone currentZone = DropZone.None;

    // Canvas zones
    private float canvasWidth;
    private float canvasMiddleX;
    private float leftZoneStart;
    private float leftZoneEnd;
    private float rightZoneStart;
    private float rightZoneEnd;

    // Events for drag and drop
    public event Action<Vector2> OnDragStarted;
    public event Action<Vector2, DropZone> OnDragging;
    public event Action<DropZone> OnDropped;
    public event Action OnDragCancelled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Find canvas if not assigned
        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
        }
        CalculateCanvasZones();
    }

    private void CalculateCanvasZones()
    {
        if (canvas == null)
            return;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasWidth = canvasRect.rect.width;
        canvasMiddleX = canvasWidth / 2f;

        // Left half: from left edge to middle
        leftZoneStart = -canvasWidth / 2f;
        leftZoneEnd = 0f - canvasMiddleXOffset;

        // Right half: from middle to right edge
        rightZoneStart = 0f + canvasMiddleXOffset;
        rightZoneEnd = canvasWidth / 2f;

        Debug.Log($"Canvas Width: {canvasWidth}, Middle: {canvasMiddleX}");
        Debug.Log($"Left Zone: {leftZoneStart} to {leftZoneEnd}");
        Debug.Log($"Right Zone: {rightZoneStart} to {rightZoneEnd}");
    }

    private void Update()
    {
        HandleTouchInput();
        HandleMouseInput(); // For testing in the editor with mouse input
    }

    #region Touch Input
    private void HandleTouchInput()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                StartDrag(touch.position);
                break;

            case TouchPhase.Moved:
                UpdateDrag(touch.position);
                break;

            case TouchPhase.Ended:
                EndDrag();
                break;

            case TouchPhase.Canceled:
                CancelDrag();
                break;
        }
    }
    #endregion

    #region  Mouse Input (for testing)
    private void HandleMouseInput()
    {
        if (Input.touchCount > 0)
            return; // Prioritize touch input

        if (Input.GetMouseButtonDown(0))
        {
            StartDrag(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            UpdateDrag(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDrag();
        }
    }
    #endregion

    private void StartDrag(Vector2 screenPos)
    {
        dragStartPos = screenPos;
        currentDragPos = screenPos;
        isDragging = true;
        currentZone = DropZone.None;

        OnDragStarted?.Invoke(dragStartPos);
        Debug.Log($"Drag started at: {dragStartPos}");
    }

    private void UpdateDrag(Vector2 screenPos)
    {
        if (!isDragging)
            return;

        currentDragPos = screenPos;

        // Convert screen position to canvas-relative position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            null,
            out Vector2 localPos
        );

        // Determine which zone we're in
        DropZone newZone = DetermineZone(localPos.x);
        if (newZone != currentZone)
        {
            currentZone = newZone;
            Debug.Log($"Zone changed to: {currentZone}");
        }

        OnDragging?.Invoke(currentDragPos, currentZone);
    }

    private void EndDrag()
    {
        if (!isDragging)
            return;

        isDragging = false;
        Debug.Log($"Dropped in zone: {currentZone}");
        
        OnDropped?.Invoke(currentZone);
        currentZone = DropZone.None;
    }

    private void CancelDrag()
    {
        if (!isDragging)
            return;

        isDragging = false;
        currentZone = DropZone.None;
        OnDragCancelled?.Invoke();
    }

    private DropZone DetermineZone(float xPosition)
    {
        if (xPosition < leftZoneEnd - dropOffset) //(xPosition >= leftZoneStart && xPosition < leftZoneEnd)
        {
            return DropZone.Left;
        }
        else if (xPosition > rightZoneStart + dropOffset) //(xPosition >= rightZoneStart && xPosition <= rightZoneEnd)
        {
            return DropZone.Right;
        }

        return DropZone.None;
    }

    public bool IsDragging => isDragging;
    public DropZone CurrentZone => currentZone;
}