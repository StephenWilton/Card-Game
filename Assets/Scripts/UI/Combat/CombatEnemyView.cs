using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatEnemyView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Image frameImage;
    [SerializeField] private TMP_Text positionText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text blockText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text intentText;
    [SerializeField] private EnemyTargetView targetView;

    [Header("Visuals")]
    [SerializeField] private Color normalColor = new Color(0.20f, 0.15f, 0.12f, 1f);
    [SerializeField] private Color selectableColor = new Color(0.47f, 0.33f, 0.15f, 1f);
    [SerializeField] private Color affectedColor = new Color(0.62f, 0.16f, 0.12f, 1f);
    [SerializeField] private Color invalidColor = new Color(0.12f, 0.10f, 0.09f, 0.62f);
    [SerializeField] private Color emptyColor = new Color(0.08f, 0.07f, 0.06f, 0.55f);
    [SerializeField] private float hoverScale = 1.04f;

    private GridEnemy enemy;
    private bool selectable;
    private bool pointerInside;
    private Action<GridEnemy> clicked;
    private Action<GridEnemy> pointerEntered;
    private Action<GridEnemy> pointerExited;

    public GridEnemy Enemy => enemy;

    public static CombatEnemyView CreateDefault(Transform parent)
    {
        GameObject enemyObject = new GameObject("EnemyView", typeof(RectTransform), typeof(Image), typeof(LayoutElement), typeof(EnemyTargetView), typeof(CombatEnemyView));
        enemyObject.transform.SetParent(parent, false);

        RectTransform rect = enemyObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(230f, 112f);

        Image frame = enemyObject.GetComponent<Image>();
        frame.color = new Color(0.20f, 0.15f, 0.12f, 1f);

        LayoutElement layout = enemyObject.GetComponent<LayoutElement>();
        layout.preferredWidth = 230f;
        layout.preferredHeight = 112f;
        layout.flexibleWidth = 1f;

        VerticalLayoutGroup layoutGroup = enemyObject.AddComponent<VerticalLayoutGroup>();
        layoutGroup.padding = new RectOffset(10, 10, 8, 8);
        layoutGroup.spacing = 3;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = true;

        TMP_Text position = CreateText(enemyObject.transform, "Position", 14, TextAlignmentOptions.Center);
        TMP_Text name = CreateText(enemyObject.transform, "Name", 20, TextAlignmentOptions.Center);

        RectTransform statRow = CreateTransparentPanel(enemyObject.transform, "Stats");
        HorizontalLayoutGroup statLayout = statRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        statLayout.spacing = 8;
        statLayout.childControlWidth = true;
        statLayout.childForceExpandWidth = true;

        TMP_Text health = CreateText(statRow, "Health", 16, TextAlignmentOptions.Center);
        TMP_Text block = CreateText(statRow, "Block", 16, TextAlignmentOptions.Center);
        TMP_Text status = CreateText(enemyObject.transform, "Status", 14, TextAlignmentOptions.Center);
        TMP_Text intent = CreateText(enemyObject.transform, "Intent", 16, TextAlignmentOptions.Center);

        CombatEnemyView view = enemyObject.GetComponent<CombatEnemyView>();
        view.frameImage = frame;
        view.positionText = position;
        view.nameText = name;
        view.healthText = health;
        view.blockText = block;
        view.statusText = status;
        view.intentText = intent;
        view.targetView = enemyObject.GetComponent<EnemyTargetView>();
        return view;
    }

    public void Bind(
        GridEnemy newEnemy,
        bool isSelectable,
        CombatTargetPreviewState previewState,
        Action<GridEnemy> onClicked,
        Action<GridEnemy> onPointerEntered,
        Action<GridEnemy> onPointerExited)
    {
        EnsureReferences();

        enemy = newEnemy;
        selectable = isSelectable;
        clicked = onClicked;
        pointerEntered = onPointerEntered;
        pointerExited = onPointerExited;

        if (targetView != null)
        {
            targetView.Bind(enemy);
        }

        gameObject.name = enemy != null ? $"EnemyView_{enemy.PositionName}_{enemy.Unit.UnitName}" : "EnemyView_Empty";

        if (positionText != null)
        {
            positionText.text = enemy.PositionName;
        }

        if (nameText != null)
        {
            nameText.text = enemy.Unit.UnitName;
        }

        if (healthText != null)
        {
            healthText.text = $"HP {enemy.Unit.UnitCurrentHealth}/{enemy.Unit.UnitMaxHealth}";
        }

        if (blockText != null)
        {
            blockText.text = $"Block {enemy.Unit.UnitBlock}";
        }

        if (intentText != null)
        {
            intentText.text = $"Intent: {enemy.IntentSummary}";
        }

        if (statusText != null)
        {
            statusText.text = enemy.Unit.GetStatusSummary();
        }

        SetPreviewState(previewState);
    }

    public void BindEmpty(string label)
    {
        EnsureReferences();

        enemy = null;
        selectable = false;
        clicked = null;
        pointerEntered = null;
        pointerExited = null;

        if (targetView != null)
        {
            targetView.Bind(null);
        }

        gameObject.name = "EnemyView_Empty";

        if (positionText != null)
        {
            positionText.text = label;
        }

        if (nameText != null)
        {
            nameText.text = "Empty";
        }

        if (healthText != null)
        {
            healthText.text = "";
        }

        if (blockText != null)
        {
            blockText.text = "";
        }

        if (intentText != null)
        {
            intentText.text = "";
        }

        if (statusText != null)
        {
            statusText.text = "";
        }

        if (frameImage != null)
        {
            frameImage.color = emptyColor;
        }
    }

    public void SetPreviewState(CombatTargetPreviewState previewState)
    {
        if (frameImage == null)
        {
            return;
        }

        switch (previewState)
        {
            case CombatTargetPreviewState.Affected:
                frameImage.color = affectedColor;
                break;

            case CombatTargetPreviewState.Selectable:
                frameImage.color = selectableColor;
                break;

            case CombatTargetPreviewState.Invalid:
                frameImage.color = invalidColor;
                break;

            default:
                frameImage.color = normalColor;
                break;
        }

        transform.localScale = pointerInside && enemy != null ? Vector3.one * hoverScale : Vector3.one;
    }

    public void ShowDamageFeedback(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        FloatingCombatText.Spawn(transform, $"-{amount}", new Color(0.92f, 0.24f, 0.18f, 1f));
    }

    public void ShowBlockFeedback(int amount)
    {
        if (amount == 0)
        {
            return;
        }

        string prefix = amount > 0 ? "+" : "";
        FloatingCombatText.Spawn(transform, $"{prefix}{amount} Block", new Color(0.56f, 0.72f, 0.92f, 1f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectable && enemy != null && enemy.IsAlive)
        {
            clicked?.Invoke(enemy);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;

        if (enemy != null && enemy.IsAlive)
        {
            pointerEntered?.Invoke(enemy);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;

        if (enemy != null)
        {
            pointerExited?.Invoke(enemy);
        }
    }

    private void Awake()
    {
        EnsureReferences();
    }

    private void EnsureReferences()
    {
        frameImage = frameImage != null ? frameImage : GetComponent<Image>();
        targetView = targetView != null ? targetView : GetComponent<EnemyTargetView>();
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

    private static RectTransform CreateTransparentPanel(Transform parent, string objectName)
    {
        GameObject panelObject = new GameObject(objectName, typeof(RectTransform));
        panelObject.transform.SetParent(parent, false);
        return panelObject.GetComponent<RectTransform>();
    }
}
