using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThreatBoardView : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text townStateText;
    [SerializeField] private TMP_Text patronText;
    [SerializeField] private TMP_Text logText;
    [SerializeField] private RectTransform optionPanel;
    [SerializeField] private RectTransform commandPanel;

    private readonly Color backgroundColor = new Color(0.065f, 0.058f, 0.05f, 1f);
    private readonly Color panelColor = new Color(0.14f, 0.12f, 0.10f, 0.96f);
    private readonly Color optionColor = new Color(0.25f, 0.19f, 0.14f, 1f);
    private readonly Color patronOptionColor = new Color(0.35f, 0.10f, 0.12f, 1f);
    private readonly Color textColor = new Color(0.90f, 0.84f, 0.74f, 1f);

    public static ThreatBoardView CreateRuntime()
    {
        GameObject canvasObject = new GameObject("ThreatBoardCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas runtimeCanvas = canvasObject.GetComponent<Canvas>();
        runtimeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        GameObject viewObject = new GameObject("ThreatBoardView", typeof(RectTransform), typeof(Image), typeof(ThreatBoardView));
        viewObject.transform.SetParent(canvasObject.transform, false);

        ThreatBoardView view = viewObject.GetComponent<ThreatBoardView>();
        view.canvas = runtimeCanvas;
        view.BuildIfNeeded();
        return view;
    }

    public void BuildIfNeeded()
    {
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        RectTransform root = GetComponent<RectTransform>();
        Stretch(root);

        Image background = GetComponent<Image>() ?? gameObject.AddComponent<Image>();
        background.color = backgroundColor;

        if (titleText != null && townStateText != null && patronText != null && optionPanel != null && commandPanel != null && logText != null)
        {
            return;
        }

        ClearChildren(transform);

        VerticalLayoutGroup rootLayout = gameObject.GetComponent<VerticalLayoutGroup>() ?? gameObject.AddComponent<VerticalLayoutGroup>();
        rootLayout.padding = new RectOffset(32, 32, 24, 24);
        rootLayout.spacing = 14;
        rootLayout.childControlWidth = true;
        rootLayout.childControlHeight = true;
        rootLayout.childForceExpandWidth = true;
        rootLayout.childForceExpandHeight = false;

        titleText = CreateText(transform, "Title", 36, TextAlignmentOptions.Center);
        titleText.text = "Threat Board";
        AddLayout(titleText.gameObject, -1f, 54f, 1f);

        RectTransform statusRow = CreateTransparentPanel(transform, "StatusRow");
        AddLayout(statusRow.gameObject, -1f, 150f, 1f);
        HorizontalLayoutGroup statusLayout = statusRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        statusLayout.spacing = 14;
        statusLayout.childControlWidth = true;
        statusLayout.childControlHeight = true;
        statusLayout.childForceExpandWidth = true;

        RectTransform townPanel = CreatePanel(statusRow, "TownPanel", panelColor);
        AddPadding(townPanel, 14);
        townStateText = CreateText(townPanel, "TownState", 22, TextAlignmentOptions.Left);
        AddLayout(townStateText.gameObject, -1f, 120f, 1f);

        RectTransform patronPanel = CreatePanel(statusRow, "PatronPanel", panelColor);
        AddPadding(patronPanel, 14);
        patronText = CreateText(patronPanel, "PatronState", 22, TextAlignmentOptions.Left);
        AddLayout(patronText.gameObject, -1f, 120f, 1f);

        optionPanel = CreatePanel(transform, "Options", panelColor);
        AddLayout(optionPanel.gameObject, -1f, 520f, 1f);
        GridLayoutGroup optionGrid = optionPanel.gameObject.AddComponent<GridLayoutGroup>();
        optionGrid.padding = new RectOffset(16, 16, 16, 16);
        optionGrid.spacing = new Vector2(14, 14);
        optionGrid.cellSize = new Vector2(430, 220);
        optionGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        optionGrid.constraintCount = 2;

        RectTransform bottomRow = CreateTransparentPanel(transform, "BottomRow");
        AddLayout(bottomRow.gameObject, -1f, 190f, 1f);
        HorizontalLayoutGroup bottomLayout = bottomRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        bottomLayout.spacing = 14;
        bottomLayout.childControlWidth = true;
        bottomLayout.childControlHeight = true;
        bottomLayout.childForceExpandWidth = true;

        RectTransform logPanel = CreatePanel(bottomRow, "LogPanel", panelColor);
        AddPadding(logPanel, 12);
        logText = CreateText(logPanel, "Log", 18, TextAlignmentOptions.Left);
        AddLayout(logText.gameObject, -1f, 160f, 1f);

        commandPanel = CreatePanel(bottomRow, "Commands", panelColor);
        AddPadding(commandPanel, 12);
        AddLayout(commandPanel.gameObject, 320f, -1f, 0f);
    }

    public void Refresh(
        RunState runState,
        ThreatBoardState boardState,
        IReadOnlyList<string> logLines,
        Action<ThreatBoardOption> onOptionSelected,
        Action onRestartRun)
    {
        BuildIfNeeded();

        titleText.text = boardState.FinalCrisisReady ? "Final Crisis" : "Threat Board";
        townStateText.text =
            $"Safe Haven\nIntegrity: {boardState.SafeHavenIntegrity}\nCountdown: {boardState.CountdownRemaining}\nThreat Level: {boardState.ThreatLevel}\nAnswered: {boardState.ChoicesMade}";
        patronText.text =
            $"{runState.PatronDisplayName}\nInfluence: {runState.PatronInfluence}\nThe Patron may mark one option.";
        logText.text = string.Join("\n", logLines);

        RenderOptions(boardState.CurrentOptions, onOptionSelected);
        RenderCommands(onRestartRun);
    }

    private void RenderOptions(IReadOnlyList<ThreatBoardOption> options, Action<ThreatBoardOption> onOptionSelected)
    {
        ClearChildren(optionPanel);

        if (options == null || options.Count == 0)
        {
            TMP_Text emptyText = CreateText(optionPanel, "NoOptions", 22, TextAlignmentOptions.Center);
            emptyText.text = "No threats available.";
            return;
        }

        foreach (ThreatBoardOption option in options)
        {
            CreateOptionButton(optionPanel, option, onOptionSelected);
        }
    }

    private void RenderCommands(Action onRestartRun)
    {
        ClearChildren(commandPanel);

        TMP_Text commandTitle = CreateText(commandPanel, "CommandTitle", 22, TextAlignmentOptions.Center);
        commandTitle.text = "Board";
        AddLayout(commandTitle.gameObject, -1f, 42f, 1f);

        CreateCommandButton(commandPanel, "Restart Board Run", onRestartRun);
    }

    private Button CreateOptionButton(Transform parent, ThreatBoardOption option, Action<ThreatBoardOption> onOptionSelected)
    {
        GameObject buttonObject = new GameObject("ThreatOption", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();
        image.color = option.IsPatronSuggestion ? patronOptionColor : optionColor;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => onOptionSelected?.Invoke(option));

        VerticalLayoutGroup layout = buttonObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(14, 14, 12, 12);
        layout.spacing = 8;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;

        TMP_Text title = CreateText(buttonObject.transform, "Title", 23, TextAlignmentOptions.Left);
        title.text = option.IsPatronSuggestion
            ? $"{option.Data.displayName}  [Patron]"
            : option.Data.displayName;
        AddLayout(title.gameObject, -1f, 34f, 1f);

        TMP_Text type = CreateText(buttonObject.transform, "Type", 16, TextAlignmentOptions.Left);
        type.text = $"{option.Data.optionType} / {option.Data.outcomeType}";
        AddLayout(type.gameObject, -1f, 26f, 1f);

        TMP_Text description = CreateText(buttonObject.transform, "Description", 17, TextAlignmentOptions.TopLeft);
        description.text = option.Data.description;
        AddLayout(description.gameObject, -1f, 84f, 1f);

        TMP_Text forecast = CreateText(buttonObject.transform, "Forecast", 15, TextAlignmentOptions.Left);
        forecast.text = $"After: Countdown {option.ProjectedCountdown}  Threat {option.ProjectedThreatLevel}  Haven {option.ProjectedSafeHavenIntegrity}";
        AddLayout(forecast.gameObject, -1f, 28f, 1f);

        return button;
    }

    private Button CreateCommandButton(Transform parent, string label, Action onClick)
    {
        GameObject buttonObject = new GameObject("CommandButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();
        image.color = optionColor;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => onClick?.Invoke());

        TMP_Text text = CreateText(buttonObject.transform, "Label", 18, TextAlignmentOptions.Center);
        text.text = label;
        Stretch(text.rectTransform);

        AddLayout(buttonObject, -1f, 58f, 1f);
        return button;
    }

    private RectTransform CreatePanel(Transform parent, string objectName, Color color)
    {
        GameObject panelObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        Image image = panelObject.GetComponent<Image>();
        image.color = color;
        return panelObject.GetComponent<RectTransform>();
    }

    private RectTransform CreateTransparentPanel(Transform parent, string objectName)
    {
        GameObject panelObject = new GameObject(objectName, typeof(RectTransform));
        panelObject.transform.SetParent(parent, false);
        return panelObject.GetComponent<RectTransform>();
    }

    private TMP_Text CreateText(Transform parent, string objectName, int fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = textColor;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
        return text;
    }

    private void AddPadding(RectTransform rectTransform, int padding)
    {
        VerticalLayoutGroup layout = rectTransform.gameObject.GetComponent<VerticalLayoutGroup>() ?? rectTransform.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(padding, padding, padding, padding);
        layout.spacing = 8;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
    }

    private void AddLayout(GameObject gameObject, float preferredWidth, float preferredHeight, float flexibleWidth)
    {
        LayoutElement layout = gameObject.GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
        layout.preferredWidth = preferredWidth;
        layout.preferredHeight = preferredHeight;
        layout.flexibleWidth = flexibleWidth;
    }

    private void Stretch(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
