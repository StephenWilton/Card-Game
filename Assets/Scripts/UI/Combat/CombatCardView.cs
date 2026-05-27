using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatCardView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private Image frameImage;
    [SerializeField] private Image artImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private LayoutElement layoutElement;

    [Header("Visuals")]
    [SerializeField] private Color normalColor = new Color(0.22f, 0.17f, 0.13f, 1f);
    [SerializeField] private Color playableColor = new Color(0.34f, 0.24f, 0.18f, 1f);
    [SerializeField] private Color selectedColor = new Color(0.65f, 0.44f, 0.18f, 1f);
    [SerializeField] private Color corruptedColor = new Color(0.42f, 0.08f, 0.08f, 1f);
    [SerializeField] private Color disabledColor = new Color(0.14f, 0.12f, 0.11f, 0.72f);
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float dragScale = 1.12f;

    private CardInstance card;
    private Canvas canvas;
    private RectTransform rectTransform;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector2 originalAnchoredPosition;
    private Vector3 originalScale;
    private bool playable;
    private bool selected;
    private bool pointerInside;
    private bool dragging;
    private bool suppressClick;
    private Action<CardInstance> clicked;
    private Action<CardInstance> dragStarted;
    private Action<CardInstance> dragEnded;
    private Action<CardInstance, GameObject> dropped;

    public CardInstance Card => card;

    public static CombatCardView CreateDefault(Transform parent)
    {
        GameObject cardObject = new GameObject("CardView", typeof(RectTransform), typeof(Image), typeof(CanvasGroup), typeof(LayoutElement), typeof(CombatCardView));
        cardObject.transform.SetParent(parent, false);

        RectTransform rect = cardObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(210f, 285f);

        Image frame = cardObject.GetComponent<Image>();
        frame.color = new Color(0.22f, 0.17f, 0.13f, 1f);

        LayoutElement layout = cardObject.GetComponent<LayoutElement>();
        layout.preferredWidth = 210f;
        layout.preferredHeight = 285f;
        layout.minWidth = 170f;
        layout.flexibleWidth = 0f;

        VerticalLayoutGroup layoutGroup = cardObject.AddComponent<VerticalLayoutGroup>();
        layoutGroup.padding = new RectOffset(12, 12, 10, 10);
        layoutGroup.spacing = 6;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        RectTransform header = CreatePanel(cardObject.transform, "Header");
        header.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        HorizontalLayoutGroup headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
        headerLayout.spacing = 8;
        headerLayout.childControlWidth = true;
        headerLayout.childForceExpandWidth = true;

        TMP_Text name = CreateText(header, "Name", 20, TextAlignmentOptions.Left);
        AddLayout(name.gameObject, -1f, 34f, 1f);

        TMP_Text cost = CreateText(header, "Cost", 24, TextAlignmentOptions.Center);
        AddLayout(cost.gameObject, 38f, 34f, 0f);

        Image art = CreatePanel(cardObject.transform, "Art").GetComponent<Image>();
        art.color = new Color(0.10f, 0.09f, 0.08f, 1f);
        AddLayout(art.gameObject, -1f, 82f, 1f);

        TMP_Text type = CreateText(cardObject.transform, "Type", 15, TextAlignmentOptions.Center);
        AddLayout(type.gameObject, -1f, 24f, 1f);

        TMP_Text target = CreateText(cardObject.transform, "Target", 14, TextAlignmentOptions.Center);
        AddLayout(target.gameObject, -1f, 22f, 1f);

        TMP_Text description = CreateText(cardObject.transform, "Description", 15, TextAlignmentOptions.TopLeft);
        AddLayout(description.gameObject, -1f, 96f, 1f);

        CombatCardView view = cardObject.GetComponent<CombatCardView>();
        view.frameImage = frame;
        view.artImage = art;
        view.nameText = name;
        view.costText = cost;
        view.typeText = type;
        view.targetText = target;
        view.descriptionText = description;
        view.canvasGroup = cardObject.GetComponent<CanvasGroup>();
        view.layoutElement = layout;
        return view;
    }

    public void Bind(
        CardInstance newCard,
        Canvas parentCanvas,
        bool isPlayable,
        bool isSelected,
        Action<CardInstance> onClicked,
        Action<CardInstance> onDragStarted,
        Action<CardInstance> onDragEnded,
        Action<CardInstance, GameObject> onDropped)
    {
        EnsureReferences();

        card = newCard;
        canvas = parentCanvas;
        playable = isPlayable;
        selected = isSelected;
        clicked = onClicked;
        dragStarted = onDragStarted;
        dragEnded = onDragEnded;
        dropped = onDropped;
        pointerInside = false;
        dragging = false;
        suppressClick = false;

        ApplyCardText();
        ApplyVisualState();
    }

    public void SetPlayable(bool value)
    {
        playable = value;
        ApplyVisualState();
    }

    public void SetSelected(bool value)
    {
        selected = value;
        ApplyVisualState();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!playable || card == null)
        {
            return;
        }

        if (suppressClick)
        {
            suppressClick = false;
            return;
        }

        clicked?.Invoke(card);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
        ApplyVisualState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;
        ApplyVisualState();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!playable || card == null || canvas == null)
        {
            return;
        }

        EnsureReferences();
        dragging = true;
        suppressClick = true;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalScale = transform.localScale;

        if (layoutElement != null)
        {
            layoutElement.ignoreLayout = true;
        }

        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();
        dragStarted?.Invoke(card);
        ApplyVisualState();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || canvas == null)
        {
            return;
        }

        float scaleFactor = Mathf.Max(canvas.scaleFactor, 0.01f);
        rectTransform.anchoredPosition += eventData.delta / scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragging)
        {
            return;
        }

        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
        RestoreToHand();
        dragging = false;
        dragEnded?.Invoke(card);
        dropped?.Invoke(card, dropTarget);
        Invoke(nameof(ClearClickSuppression), 0.05f);
        ApplyVisualState();
    }

    private void Awake()
    {
        EnsureReferences();
    }

    private void EnsureReferences()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = canvasGroup != null ? canvasGroup : GetComponent<CanvasGroup>();
        layoutElement = layoutElement != null ? layoutElement : GetComponent<LayoutElement>();
        frameImage = frameImage != null ? frameImage : GetComponent<Image>();
    }

    private void ApplyCardText()
    {
        CardData cardData = card != null ? card.CardData : null;

        if (nameText != null)
        {
            nameText.text = cardData != null ? cardData.cardName : "Missing Card";
        }

        if (costText != null)
        {
            costText.text = cardData != null ? cardData.energyCost.ToString() : "-";
        }

        if (typeText != null)
        {
            string corrupted = cardData != null && cardData.isCorrupted ? " / Corrupted" : "";
            typeText.text = cardData != null ? $"{cardData.cardType}{corrupted}" : "Unknown";
        }

        if (targetText != null)
        {
            targetText.text = cardData != null ? GetTargetText(cardData) : "No target";
        }

        if (descriptionText != null)
        {
            descriptionText.text = cardData != null ? cardData.cardDescription : "";
        }
    }

    private void ApplyVisualState()
    {
        if (frameImage != null)
        {
            if (!playable)
            {
                frameImage.color = disabledColor;
            }
            else if (selected)
            {
                frameImage.color = selectedColor;
            }
            else if (card != null && card.IsCorrupted)
            {
                frameImage.color = corruptedColor;
            }
            else
            {
                frameImage.color = playable ? playableColor : normalColor;
            }
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = playable ? 1f : 0.62f;
        }

        if (dragging)
        {
            transform.localScale = Vector3.one * dragScale;
        }
        else if (pointerInside && playable)
        {
            transform.localScale = Vector3.one * hoverScale;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    private void RestoreToHand()
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }

        if (originalParent == null)
        {
            return;
        }

        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.anchoredPosition = originalAnchoredPosition;
        transform.localScale = originalScale == Vector3.zero ? Vector3.one : originalScale;

        if (layoutElement != null)
        {
            layoutElement.ignoreLayout = false;
        }
    }

    private void ClearClickSuppression()
    {
        suppressClick = false;
    }

    private string GetTargetText(CardData cardData)
    {
        foreach (CardActionData action in cardData.actions)
        {
            switch (action.target)
            {
                case CardTarget.Enemy:
                    return "Target: enemy";
                case CardTarget.PierceColumn:
                    return "Target: column";
                case CardTarget.AllEnemies:
                    return "Target: all enemies";
                case CardTarget.FirstRow:
                    return "Target: front row";
                case CardTarget.BackRow:
                    return "Target: back row";
            }
        }

        foreach (CardActionData action in cardData.actions)
        {
            if (action.target == CardTarget.Player)
            {
                return "Target: self";
            }
        }

        return "Target: none";
    }

    private static RectTransform CreatePanel(Transform parent, string objectName)
    {
        GameObject panelObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(parent, false);
        return panelObject.GetComponent<RectTransform>();
    }

    private static TMP_Text CreateText(Transform parent, string objectName, int fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = new Color(0.90f, 0.84f, 0.74f, 1f);
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
        return text;
    }

    private static void AddLayout(GameObject gameObject, float preferredWidth, float preferredHeight, float flexibleWidth)
    {
        LayoutElement element = gameObject.GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
        element.preferredWidth = preferredWidth;
        element.preferredHeight = preferredHeight;
        element.flexibleWidth = flexibleWidth;
    }
}
