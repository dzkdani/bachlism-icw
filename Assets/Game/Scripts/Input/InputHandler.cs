using UnityEngine;
using DG.Tweening;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private float returnDuration = 0.4f;
    [SerializeField] private float maxRotation = 20f;

    private RectTransform cardRect;
    private CanvasGroup cardCanvasGroup;
    private Canvas canvas;

    private Vector2 originalPosition;
    private Quaternion originalRotation;
    private bool isActiveDrag;
    private float canvasHalfWidth;

    private CardInfo cardInfo;
    private InputManager.DropZone currentZone = InputManager.DropZone.None;


    private void Awake()
    {
        cardRect = GetComponent<RectTransform>();
        cardCanvasGroup = GetComponent<CanvasGroup>();
        cardInfo = GetComponent<CardInfo>();
    }

    private void OnEnable()
    {
        KillTweens();
    }

    private void OnDisable()
    {
        KillTweens();
        isActiveDrag = false;
    }

    public void BeginDrag(Vector2 screenPos)
    {
        if (cardRect == null)
            return;

        KillTweens();

        isActiveDrag = true;
        originalPosition = cardRect.anchoredPosition;
        originalRotation = cardRect.localRotation;

        if (cardCanvasGroup != null)
        {
            cardCanvasGroup.interactable = false;
            cardCanvasGroup.blocksRaycasts = false;
        }

        UpdateDrag(screenPos, InputManager.DropZone.None);

        AudioManager.Instance.Play("swipe");
    }

    public void UpdateDrag(Vector2 currentPos, InputManager.DropZone zone)
    {
        if (!isActiveDrag || cardRect == null)
            return;

        RectTransform parentRect = cardRect.parent as RectTransform;
        if (parentRect == null)
            return;

        // Convert screen position to card-parent local position.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            currentPos,
            UICamera,
            out Vector2 localPos
        );

        cardRect.anchoredPosition = localPos;

        float distanceFromOrigin = Mathf.Abs(localPos.x - originalPosition.x);
        float maxDistance = Mathf.Max(1f, canvasHalfWidth);
        float lerpRatio = Mathf.Clamp01(distanceFromOrigin / maxDistance);

        float targetRotation = 0f;
        switch (zone)
        {
            case InputManager.DropZone.Left:
                targetRotation = Mathf.Lerp(0f, maxRotation, lerpRatio);
                break;
            case InputManager.DropZone.Right:
                targetRotation = Mathf.Lerp(0f, -maxRotation, lerpRatio);
                break;
            case InputManager.DropZone.None:
                targetRotation = 0f;
                break;
        }

        if (zone != currentZone)
        {
            currentZone = zone;

            switch (zone)
            {
                case InputManager.DropZone.Left:
                    cardInfo?.ShowLeftDecision();
                    break;
                case InputManager.DropZone.Right:
                    cardInfo?.ShowRightDecision();
                    break;
                case InputManager.DropZone.None:
                    cardInfo?.ShowDefaultText();
                    break;
            }
        }

        cardRect.localRotation = Quaternion.Euler(0f, 0f, targetRotation);
    }

    public void EndDrag(InputManager.DropZone zone)
    {
        if (!isActiveDrag || cardRect == null)
            return;

        isActiveDrag = false;

        switch (zone)
        {
            case InputManager.DropZone.Left:
            case InputManager.DropZone.Right:
                ChooseSequence().Play();
                break;
            case InputManager.DropZone.None:
                ReturnSequence().Play();
                break;
        }

        AudioManager.Instance.Play("drop");
    }

    public void CancelDrag()
    {
        if (!isActiveDrag || cardRect == null)
            return;

        isActiveDrag = false;
        ReturnSequence().Play();
        AudioManager.Instance.Play("drop");
    }

    public Sequence ChooseSequence()
    {
        Sequence sequence = DOTween.Sequence();

        if (cardCanvasGroup != null)
        {
            sequence.Append(cardCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.OutCubic));
        }
        else
        {
            sequence.AppendInterval(0.5f);
        }

        sequence.Join(cardRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic));
        sequence.OnComplete(() =>
        {
            ResetCardVisualState();
        });
        
        return sequence;
    }

    public Sequence ReturnSequence()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(cardRect.DOAnchorPos(originalPosition, returnDuration).SetEase(Ease.OutCubic));
        sequence.Join(cardRect.DOLocalRotateQuaternion(originalRotation, returnDuration).SetEase(Ease.OutCubic));
        sequence.Join(cardRect.DOScale(Vector3.one, returnDuration).SetEase(Ease.OutCubic));
        sequence.OnComplete(ResetCardVisualState);
        return sequence;
    }

    private void ResetCardVisualState()
    {
        if (cardRect != null)
        {
            cardRect.localScale = Vector3.one;
            cardRect.localRotation = originalRotation;
            cardRect.anchoredPosition = originalPosition;
        }

        ResetCG();
    }

    private void ResetCG()
    {
        if (cardCanvasGroup == null)
            return;

        cardCanvasGroup.alpha = 1f;
        cardCanvasGroup.interactable = true;
        cardCanvasGroup.blocksRaycasts = true;
    }

    private void KillTweens()
    {
        if (cardRect != null)
        {
            cardRect.DOKill();
        }

        if (cardCanvasGroup != null)
        {
            cardCanvasGroup.DOKill();
        }
    }

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
