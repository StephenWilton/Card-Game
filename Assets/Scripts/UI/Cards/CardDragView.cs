using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private CardInstance card;
    private Canvas canvas;
    private Action<CardInstance> onClick;
    private Action<CardInstance, GameObject> onDrop;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private LayoutElement layoutElement;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector2 originalAnchoredPosition;
    private bool isPlayable;
    private bool isDragging;
    private bool suppressNextClick;

    public void Bind(
        CardInstance card,
        Canvas canvas,
        Action<CardInstance> onClick,
        Action<CardInstance, GameObject> onDrop,
        bool isPlayable)
    {
        this.card = card;
        this.canvas = canvas;
        this.onClick = onClick;
        this.onDrop = onDrop;
        this.isPlayable = isPlayable;

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        layoutElement = GetComponent<LayoutElement>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isPlayable)
        {
            return;
        }

        if (suppressNextClick)
        {
            suppressNextClick = false;
            return;
        }

        onClick?.Invoke(card);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isPlayable || canvas == null)
        {
            return;
        }

        isDragging = true;
        suppressNextClick = true;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        originalAnchoredPosition = rectTransform.anchoredPosition;

        if (layoutElement != null)
        {
            layoutElement.ignoreLayout = true;
        }

        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isPlayable || !isDragging)
        {
            return;
        }

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isPlayable || !isDragging)
        {
            return;
        }

        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
        RestoreToHand();
        isDragging = false;
        Invoke(nameof(ClearClickSuppression), 0.05f);
        onDrop?.Invoke(card, dropTarget);
    }

    private void ClearClickSuppression()
    {
        suppressNextClick = false;
    }

    private void RestoreToHand()
    {
        canvasGroup.blocksRaycasts = true;

        if (originalParent == null)
        {
            return;
        }

        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.anchoredPosition = originalAnchoredPosition;

        if (layoutElement != null)
        {
            layoutElement.ignoreLayout = false;
        }
    }
}
