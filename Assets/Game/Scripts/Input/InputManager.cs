using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public enum DropZone { Left, Right, None }

    [Header("Drag Settings")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private float canvasMiddleXOffset = 40f; // Offset from middle to create a dead zone
    [SerializeField] private float dropOffset = 20f; // Minimum distance from middle to count as a drop

    private Vector2 currentDragPos;
    private bool isDragging;
    private InputHandler activeHandler;
    private DropZone currentZone = DropZone.None;

    // Canvas zones
    private float leftZoneEnd;
    private float rightZoneStart;

    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>(16);


    public event Action<InputHandler> OnActiveDragStarted;
    public event Action<InputHandler, Vector2, DropZone> OnActiveDragging;
    public event Action<InputHandler, DropZone> OnActiveDropped;
    public event Action<InputHandler> OnActiveDragCancelled;

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
            canvas = FindAnyObjectByType<Canvas>();

        if (graphicRaycaster == null && canvas != null)
            graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();

        CalculateCanvasZones();
    }

    private void CalculateCanvasZones()
    {
        if (canvas == null)
            return;

        leftZoneEnd = -canvasMiddleXOffset;
        rightZoneStart = canvasMiddleXOffset;
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

    #region Mouse Input (for testing)
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
        if (isDragging || canvas == null || graphicRaycaster == null || EventSystem.current == null)
            return;

        InputHandler handler = FindHandlerUnderPointer(screenPos);
        if (handler == null)
            return;

        activeHandler = handler;
        currentDragPos = screenPos;
        isDragging = true;
        currentZone = DropZone.None;

        activeHandler.BeginDrag(screenPos);
        OnActiveDragStarted?.Invoke(activeHandler);
    }

    private void UpdateDrag(Vector2 screenPos)
    {
        if (!isDragging || activeHandler == null || canvas == null)
            return;

        currentDragPos = screenPos;

        // Convert screen position to canvas-relative position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            UICamera,
            out Vector2 localPos
        );

        currentZone = DetermineZone(localPos.x);

        activeHandler.UpdateDrag(currentDragPos, currentZone);
        OnActiveDragging?.Invoke(activeHandler, currentDragPos, currentZone);
    }

    private void EndDrag()
    {
        if (!isDragging)
            return;

        InputHandler droppedHandler = activeHandler;
        DropZone droppedZone = currentZone;

        isDragging = false;
        activeHandler = null;
        currentZone = DropZone.None;

        if (droppedHandler == null)
            return;

        droppedHandler.EndDrag(droppedZone);
        OnActiveDropped?.Invoke(droppedHandler, droppedZone);
    }

    private void CancelDrag()
    {
        if (!isDragging)
            return;

        InputHandler cancelledHandler = activeHandler;

        isDragging = false;
        activeHandler = null;
        currentZone = DropZone.None;

        if (cancelledHandler == null)
            return;

        cancelledHandler.CancelDrag();
        OnActiveDragCancelled?.Invoke(cancelledHandler);
    }

    private InputHandler FindHandlerUnderPointer(Vector2 screenPos)
    {
        raycastResults.Clear();

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };

        graphicRaycaster.Raycast(pointerEventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            InputHandler handler = result.gameObject.GetComponentInParent<InputHandler>();
            if (handler != null && handler.isActiveAndEnabled)
            {
                return handler;
            }
        }

        return null;
    }

    private DropZone DetermineZone(float xPosition)
    {
        if (xPosition < leftZoneEnd - dropOffset)
        {
            return DropZone.Left;
        }

        if (xPosition > rightZoneStart + dropOffset)
        {
            return DropZone.Right;
        }

        return DropZone.None;
    }

    public bool IsDragging => isDragging;
    public InputHandler ActiveHandler => activeHandler;
    public DropZone CurrentZone => currentZone;
    public Canvas ActiveCanvas => canvas;

    private Camera UICamera
    {
        get
        {
            if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;
            return canvas.worldCamera;
        }
    }
}
