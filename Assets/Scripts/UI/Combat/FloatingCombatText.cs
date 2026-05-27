using TMPro;
using UnityEngine;

public class FloatingCombatText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float lifetime = 0.85f;
    [SerializeField] private Vector2 travel = new Vector2(0f, 42f);

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Color startColor;
    private float elapsed;

    public static FloatingCombatText Spawn(Transform parent, string value, Color color)
    {
        GameObject textObject = new GameObject("FloatingCombatText", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(FloatingCombatText));
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(160f, 42f);
        rect.anchoredPosition = Vector2.zero;

        TMP_Text tmp = textObject.GetComponent<TMP_Text>();
        tmp.text = value;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;
        tmp.raycastTarget = false;

        FloatingCombatText floatingText = textObject.GetComponent<FloatingCombatText>();
        floatingText.text = tmp;
        floatingText.Begin();
        return floatingText;
    }

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<TMP_Text>();
        }

        rectTransform = GetComponent<RectTransform>();
    }

    private void Begin()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        startPosition = rectTransform.anchoredPosition;
        startColor = text != null ? text.color : Color.white;
        elapsed = 0f;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float progress = lifetime > 0f ? Mathf.Clamp01(elapsed / lifetime) : 1f;

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startPosition + travel * EaseOut(progress);
        }

        if (text != null)
        {
            Color color = startColor;
            color.a = 1f - progress;
            text.color = color;
        }

        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }

    private float EaseOut(float value)
    {
        return 1f - Mathf.Pow(1f - value, 3f);
    }
}
