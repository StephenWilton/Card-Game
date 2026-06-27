using UnityEngine;

[ExecuteAlways]
public class HandLayout : MonoBehaviour
{
    [SerializeField] private float normalSpacing = 20f;
    [SerializeField] private float minSpacing = -80f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        LayoutCards();
    }

    private void LayoutCards()
    {
        int count = transform.childCount;
        if (count == 0) return;

        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        RectTransform firstCard = transform.GetChild(0).GetComponent<RectTransform>();
        float cardWidth = firstCard.rect.width;
        float availableWidth = rectTransform.rect.width;

        float spacing = normalSpacing;

        if (count > 1)
        {
            float spacingToFit = (availableWidth - cardWidth * count) / (count - 1);
            spacing = Mathf.Clamp(spacingToFit, minSpacing, normalSpacing);
        }

        float totalWidth = cardWidth * count + spacing * (count - 1);
        float startX = -totalWidth / 2f + cardWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            RectTransform card = transform.GetChild(i).GetComponent<RectTransform>();
            card.anchorMin = new Vector2(0.5f, 0.5f);
            card.anchorMax = new Vector2(0.5f, 0.5f);
            card.pivot = new Vector2(0.5f, 0.5f);
            card.anchoredPosition = new Vector2(startX + i * (cardWidth + spacing), 0f);
        }
    }
}