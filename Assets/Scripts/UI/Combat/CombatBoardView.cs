using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatBoardView : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private CombatCardView cardViewPrefab;
    [SerializeField] private CombatEnemyView enemyViewPrefab;

    [Header("Root References")]
    [SerializeField] private RectTransform handPanel;
    [SerializeField] private RectTransform frontRowPanel;
    [SerializeField] private RectTransform backRowPanel;
    [SerializeField] private RectTransform choicePanel;
    [SerializeField] private RectTransform playerPanel;
    [SerializeField] private RectTransform pilePanel;
    [SerializeField] private RectTransform logPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_Text pileText;
    [SerializeField] private TMP_Text logText;

    [Header("Visuals")]
    [SerializeField] private Color backgroundColor = new Color(0.07f, 0.055f, 0.05f, 1f);
    [SerializeField] private Color panelColor = new Color(0.13f, 0.105f, 0.085f, 0.96f);
    [SerializeField] private Color quietPanelColor = new Color(0.09f, 0.08f, 0.07f, 0.72f);
    [SerializeField] private Color commandColor = new Color(0.32f, 0.23f, 0.17f, 1f);
    [SerializeField] private Color disabledCommandColor = new Color(0.14f, 0.12f, 0.10f, 0.65f);
    [SerializeField] private Color textColor = new Color(0.90f, 0.84f, 0.74f, 1f);

    private readonly List<CombatEnemyView> enemyViews = new List<CombatEnemyView>();
    private readonly Dictionary<GridEnemy, int> lastEnemyHealth = new Dictionary<GridEnemy, int>();
    private readonly Dictionary<GridEnemy, int> lastEnemyBlock = new Dictionary<GridEnemy, int>();

    private Canvas canvas;
    private Action<CardInstance> onCardClicked;
    private Action<CardInstance, GameObject> onCardDropped;
    private Action<GridEnemy> onEnemyClicked;
    private Action onEndTurnClicked;
    private Action onTakeRewardClicked;
    private Action onSacrificeRewardClicked;
    private Action onHealClicked;
    private Action onUpgradeClicked;
    private Action onCorruptClicked;
    private Action<int> onDeckCardClicked;
    private Action onRestartClicked;

    private CardInstance selectedCard;
    private CardInstance draggedCard;
    private GridEnemy hoveredEnemy;
    private int lastPlayerHealth = -1;
    private int lastPlayerBlock = -1;

    public static CombatBoardView CreateRuntime(Canvas parentCanvas)
    {
        GameObject boardObject = new GameObject("CombatBoardView", typeof(RectTransform), typeof(Image), typeof(CombatBoardView));
        boardObject.transform.SetParent(parentCanvas.transform, false);

        CombatBoardView boardView = boardObject.GetComponent<CombatBoardView>();
        boardView.canvas = parentCanvas;
        boardView.BuildIfNeeded();
        return boardView;
    }

    public void SetPrefabReferences(CombatCardView cardPrefab, CombatEnemyView enemyPrefab)
    {
        cardViewPrefab = cardPrefab;
        enemyViewPrefab = enemyPrefab;
    }

    public void Initialize(
        Canvas parentCanvas,
        Action<CardInstance> cardClicked,
        Action<CardInstance, GameObject> cardDropped,
        Action<GridEnemy> enemyClicked,
        Action endTurnClicked,
        Action takeRewardClicked,
        Action sacrificeRewardClicked,
        Action healClicked,
        Action upgradeClicked,
        Action corruptClicked,
        Action<int> deckCardClicked,
        Action restartClicked)
    {
        canvas = parentCanvas;
        onCardClicked = cardClicked;
        onCardDropped = cardDropped;
        onEnemyClicked = enemyClicked;
        onEndTurnClicked = endTurnClicked;
        onTakeRewardClicked = takeRewardClicked;
        onSacrificeRewardClicked = sacrificeRewardClicked;
        onHealClicked = healClicked;
        onUpgradeClicked = upgradeClicked;
        onCorruptClicked = corruptClicked;
        onDeckCardClicked = deckCardClicked;
        onRestartClicked = restartClicked;
        BuildIfNeeded();
    }

    public void BuildIfNeeded()
    {
        RectTransform root = GetComponent<RectTransform>();
        Stretch(root);

        Image backgroundImage = GetComponent<Image>() ?? gameObject.AddComponent<Image>();
        backgroundImage.color = backgroundColor;

        if (titleText != null &&
            statusText != null &&
            playerPanel != null &&
            pilePanel != null &&
            frontRowPanel != null &&
            backRowPanel != null &&
            handPanel != null &&
            choicePanel != null &&
            logPanel != null)
        {
            return;
        }

        ClearChildren(transform);

        VerticalLayoutGroup rootLayout = gameObject.GetComponent<VerticalLayoutGroup>() ?? gameObject.AddComponent<VerticalLayoutGroup>();
        rootLayout.padding = new RectOffset(28, 28, 22, 22);
        rootLayout.spacing = 12;
        rootLayout.childControlWidth = true;
        rootLayout.childControlHeight = true;
        rootLayout.childForceExpandWidth = true;
        rootLayout.childForceExpandHeight = false;

        titleText = CreateText(transform, "Title", 34, TextAlignmentOptions.Center);
        AddLayout(titleText.gameObject, -1f, 46f, 1f);

        statusText = CreateText(transform, "Status", 22, TextAlignmentOptions.Center);
        AddLayout(statusText.gameObject, -1f, 38f, 1f);

        RectTransform topRow = CreateTransparentPanel(transform, "TopRow");
        AddLayout(topRow.gameObject, -1f, 126f, 1f);
        HorizontalLayoutGroup topLayout = topRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        topLayout.spacing = 12;
        topLayout.childControlWidth = true;
        topLayout.childControlHeight = true;
        topLayout.childForceExpandWidth = true;

        playerPanel = CreatePanel(topRow, "PlayerPanel", panelColor);
        AddPadding(playerPanel, 14);
        playerText = CreateText(playerPanel, "PlayerText", 22, TextAlignmentOptions.Left);
        AddLayout(playerText.gameObject, -1f, 96f, 1f);

        pilePanel = CreatePanel(topRow, "PilePanel", panelColor);
        AddPadding(pilePanel, 14);
        pileText = CreateText(pilePanel, "PileText", 18, TextAlignmentOptions.Left);
        AddLayout(pileText.gameObject, -1f, 96f, 1f);

        RectTransform middleRow = CreateTransparentPanel(transform, "MiddleRow");
        AddLayout(middleRow.gameObject, -1f, 332f, 1f);
        HorizontalLayoutGroup middleLayout = middleRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        middleLayout.spacing = 12;
        middleLayout.childControlWidth = true;
        middleLayout.childControlHeight = true;
        middleLayout.childForceExpandWidth = true;

        RectTransform enemyArea = CreatePanel(middleRow, "EnemyArea", panelColor);
        AddPadding(enemyArea, 12);
        AddLayout(enemyArea.gameObject, -1f, -1f, 2f);

        CreateText(enemyArea, "EnemyTitle", 22, TextAlignmentOptions.Center).text = "Enemy Formation";

        backRowPanel = CreateTransparentPanel(enemyArea, "BackRow");
        AddLayout(backRowPanel.gameObject, -1f, 122f, 1f);
        AddHorizontalRowLayout(backRowPanel);

        frontRowPanel = CreateTransparentPanel(enemyArea, "FrontRow");
        AddLayout(frontRowPanel.gameObject, -1f, 122f, 1f);
        AddHorizontalRowLayout(frontRowPanel);

        choicePanel = CreatePanel(middleRow, "ChoicePanel", panelColor);
        AddPadding(choicePanel, 12);
        AddLayout(choicePanel.gameObject, 420f, -1f, 0f);

        handPanel = CreatePanel(transform, "HandPanel", quietPanelColor);
        AddLayout(handPanel.gameObject, -1f, 314f, 1f);
        HorizontalLayoutGroup handLayout = handPanel.gameObject.AddComponent<HorizontalLayoutGroup>();
        handLayout.padding = new RectOffset(14, 14, 14, 14);
        handLayout.spacing = 12;
        handLayout.childControlWidth = false;
        handLayout.childControlHeight = true;
        handLayout.childForceExpandWidth = false;
        handLayout.childForceExpandHeight = true;
        handLayout.childAlignment = TextAnchor.MiddleCenter;

        logPanel = CreatePanel(transform, "LogPanel", quietPanelColor);
        AddLayout(logPanel.gameObject, -1f, 132f, 1f);
        AddPadding(logPanel, 12);
        logText = CreateText(logPanel, "LogText", 17, TextAlignmentOptions.Left);
        AddLayout(logText.gameObject, -1f, 106f, 1f);
    }

    public void ResetTransientState()
    {
        draggedCard = null;
        hoveredEnemy = null;
        lastPlayerHealth = -1;
        lastPlayerBlock = -1;
        lastEnemyHealth.Clear();
        lastEnemyBlock.Clear();
    }

    public void Refresh(
        CombatFlowState state,
        string heroName,
        string patronName,
        Unit player,
        EnemyFormation formation,
        DeckRuntime deck,
        int turnNumber,
        int currentEnergy,
        int maxEnergy,
        int patronInfluence,
        CardInstance currentSelectedCard,
        int selectedDeckIndex,
        CardData currentRewardCard,
        IReadOnlyList<string> logLines,
        int shrineHealAmount,
        int patronCorruptCost,
        bool canUpgrade,
        bool canCorrupt)
    {
        BuildIfNeeded();

        selectedCard = currentSelectedCard;
        hoveredEnemy = null;

        titleText.text = $"{heroName} / {patronName}";
        statusText.text = GetStatusText(state, turnNumber, currentEnergy, maxEnergy, patronInfluence, selectedCard);
        playerText.text = GetPlayerText(player);
        pileText.text = GetPileText(deck, selectedDeckIndex);
        logText.text = string.Join("\n", logLines);

        RenderEnemyRows(formation, selectedCard);
        RenderHand(state, deck, currentEnergy, selectedCard);
        RenderChoices(state, deck, selectedDeckIndex, currentRewardCard, shrineHealAmount, patronCorruptCost, canUpgrade, canCorrupt);
        ShowPlayerDeltaFeedback(player);
    }

    public void UpdateLog(IReadOnlyList<string> logLines)
    {
        if (logText != null)
        {
            logText.text = string.Join("\n", logLines);
        }
    }

    private string GetStatusText(CombatFlowState state, int turnNumber, int currentEnergy, int maxEnergy, int patronInfluence, CardInstance currentSelectedCard)
    {
        if (state != CombatFlowState.Combat)
        {
            return $"{state}     Patron Influence: {patronInfluence}";
        }

        string selectedText = currentSelectedCard == null ? "Ready" : $"Targeting {currentSelectedCard.CardName}";
        return $"Turn {turnNumber}     Energy {currentEnergy}/{maxEnergy}     Patron Influence: {patronInfluence}     {selectedText}";
    }

    private string GetPlayerText(Unit player)
    {
        string statuses = player.GetStatusSummary();
        string statusLine = string.IsNullOrWhiteSpace(statuses) ? "" : $"\n{statuses}";
        return $"{player.UnitName}\nHP {player.UnitCurrentHealth}/{player.UnitMaxHealth}\nBlock {player.UnitBlock}{statusLine}";
    }

    private string GetPileText(DeckRuntime deck, int selectedDeckIndex)
    {
        string text = $"Deck {deck.Deck.Count}     Draw {deck.DrawPile.Count}     Discard {deck.DiscardPile.Count}\n";

        for (int i = 0; i < deck.Deck.Count; i++)
        {
            CardInstance card = deck.Deck[i];
            string marker = i == selectedDeckIndex ? "> " : "  ";
            string corrupted = card.IsCorrupted ? " [Corrupted]" : "";
            text += $"{marker}{i + 1}. {card.CardName}{corrupted}\n";
        }

        return text;
    }

    private void RenderEnemyRows(EnemyFormation formation, CardInstance currentSelectedCard)
    {
        enemyViews.Clear();
        ClearChildren(frontRowPanel);
        ClearChildren(backRowPanel);

        for (int row = 1; row >= 0; row--)
        {
            RectTransform rowPanel = row == 0 ? frontRowPanel : backRowPanel;

            for (int column = 0; column < formation.GridColumns; column++)
            {
                GridEnemy enemy = formation.GetEnemyAt(row, column);
                CombatEnemyView enemyView = CreateEnemyView(rowPanel);

                if (enemy == null || !enemy.IsAlive)
                {
                    enemyView.BindEmpty(row == 0 ? $"Front {column + 1}" : $"Back {column + 1}");
                    continue;
                }

                CombatTargetPreviewState previewState = GetPreviewState(enemy, currentSelectedCard);
                bool selectable = TargetResolver.CanSelectEnemy(currentSelectedCard, enemy);

                enemyView.Bind(
                    enemy,
                    selectable,
                    previewState,
                    onEnemyClicked,
                    OnEnemyPointerEntered,
                    OnEnemyPointerExited);

                ShowEnemyDeltaFeedback(enemy, enemyView);
                enemyViews.Add(enemyView);
            }
        }

        ApplyTargetPreview();
    }

    private void RenderHand(CombatFlowState state, DeckRuntime deck, int currentEnergy, CardInstance currentSelectedCard)
    {
        ClearChildren(handPanel);

        if (state != CombatFlowState.Combat)
        {
            TMP_Text closedText = CreateText(handPanel, "HandClosed", 20, TextAlignmentOptions.Center);
            closedText.text = "Hand closed";
            AddLayout(closedText.gameObject, -1f, 82f, 1f);
            return;
        }

        foreach (CardInstance card in deck.Hand)
        {
            bool isPlayable = currentEnergy >= card.EnergyCost;
            CombatCardView cardView = CreateCardView(handPanel);
            cardView.Bind(
                card,
                canvas,
                isPlayable,
                currentSelectedCard == card,
                onCardClicked,
                OnCardDragStarted,
                OnCardDragEnded,
                onCardDropped);
        }
    }

    private void RenderChoices(
        CombatFlowState state,
        DeckRuntime deck,
        int selectedDeckIndex,
        CardData currentRewardCard,
        int shrineHealAmount,
        int patronCorruptCost,
        bool canUpgrade,
        bool canCorrupt)
    {
        ClearChildren(choicePanel);

        if (state == CombatFlowState.Reward)
        {
            RenderRewardChoices(currentRewardCard);
            return;
        }

        if (state == CombatFlowState.Shrine)
        {
            RenderShrineChoices(deck, selectedDeckIndex, shrineHealAmount, patronCorruptCost, canUpgrade, canCorrupt);
            return;
        }

        if (state == CombatFlowState.Complete || state == CombatFlowState.Defeat)
        {
            TMP_Text stateText = CreateText(choicePanel, "RunState", 24, TextAlignmentOptions.Center);
            stateText.text = state == CombatFlowState.Complete ? "Run complete" : "Defeat";
            AddLayout(stateText.gameObject, -1f, 92f, 1f);
            CreateCommandButton(choicePanel, "Restart Run", true, onRestartClicked);
            return;
        }

        TMP_Text turnText = CreateText(choicePanel, "TurnPanelTitle", 24, TextAlignmentOptions.Center);
        turnText.text = "Player Turn";
        AddLayout(turnText.gameObject, -1f, 72f, 1f);
        CreateCommandButton(choicePanel, "End Turn", true, onEndTurnClicked);
    }

    private void RenderRewardChoices(CardData currentRewardCard)
    {
        TMP_Text title = CreateText(choicePanel, "RewardTitle", 22, TextAlignmentOptions.Center);
        title.text = "Card Reward";
        AddLayout(title.gameObject, -1f, 42f, 1f);

        if (currentRewardCard != null)
        {
            CardInstance rewardInstance = new CardInstance(currentRewardCard);
            CombatCardView rewardView = CreateCardView(choicePanel);
            rewardView.Bind(rewardInstance, canvas, false, false, null, null, null, null);
        }
        else
        {
            TMP_Text emptyReward = CreateText(choicePanel, "NoReward", 18, TextAlignmentOptions.Center);
            emptyReward.text = "No reward available";
            AddLayout(emptyReward.gameObject, -1f, 80f, 1f);
        }

        CreateCommandButton(choicePanel, "Add Card", currentRewardCard != null, onTakeRewardClicked);
        CreateCommandButton(choicePanel, "Sacrifice", currentRewardCard != null, onSacrificeRewardClicked);
    }

    private void RenderShrineChoices(DeckRuntime deck, int selectedDeckIndex, int shrineHealAmount, int patronCorruptCost, bool canUpgrade, bool canCorrupt)
    {
        TMP_Text title = CreateText(choicePanel, "ShrineTitle", 22, TextAlignmentOptions.Center);
        title.text = "Rest Site";
        AddLayout(title.gameObject, -1f, 38f, 1f);

        RectTransform deckList = CreateTransparentPanel(choicePanel, "DeckList");
        AddLayout(deckList.gameObject, -1f, 170f, 1f);
        VerticalLayoutGroup deckLayout = deckList.gameObject.AddComponent<VerticalLayoutGroup>();
        deckLayout.spacing = 4;
        deckLayout.childControlWidth = true;
        deckLayout.childForceExpandWidth = true;

        for (int i = 0; i < deck.Deck.Count; i++)
        {
            int cardIndex = i;
            string label = i == selectedDeckIndex ? $"> {deck.Deck[i].CardName}" : deck.Deck[i].CardName;
            CreateCommandButton(deckList, label, true, () => onDeckCardClicked?.Invoke(cardIndex), 34f);
        }

        CreateCommandButton(choicePanel, $"Heal {shrineHealAmount}", true, onHealClicked);
        CreateCommandButton(choicePanel, "Upgrade Selected", canUpgrade, onUpgradeClicked);
        CreateCommandButton(choicePanel, $"Corrupt Selected ({patronCorruptCost})", canCorrupt, onCorruptClicked);
    }

    private CombatCardView CreateCardView(Transform parent)
    {
        if (cardViewPrefab != null)
        {
            return Instantiate(cardViewPrefab, parent);
        }

        return CombatCardView.CreateDefault(parent);
    }

    private CombatEnemyView CreateEnemyView(Transform parent)
    {
        if (enemyViewPrefab != null)
        {
            return Instantiate(enemyViewPrefab, parent);
        }

        return CombatEnemyView.CreateDefault(parent);
    }

    private void OnCardDragStarted(CardInstance card)
    {
        draggedCard = card;
        hoveredEnemy = null;
        ApplyTargetPreview();
    }

    private void OnCardDragEnded(CardInstance card)
    {
        draggedCard = null;
        hoveredEnemy = null;
        ApplyTargetPreview();
    }

    private void OnEnemyPointerEntered(GridEnemy enemy)
    {
        hoveredEnemy = enemy;
        ApplyTargetPreview();
    }

    private void OnEnemyPointerExited(GridEnemy enemy)
    {
        if (hoveredEnemy == enemy)
        {
            hoveredEnemy = null;
            ApplyTargetPreview();
        }
    }

    private void ApplyTargetPreview()
    {
        CardInstance activeCard = draggedCard ?? selectedCard;

        foreach (CombatEnemyView enemyView in enemyViews)
        {
            if (enemyView.Enemy == null)
            {
                continue;
            }

            enemyView.SetPreviewState(GetPreviewState(enemyView.Enemy, activeCard));
        }
    }

    private CombatTargetPreviewState GetPreviewState(GridEnemy enemy, CardInstance activeCard)
    {
        if (activeCard == null || enemy == null || !enemy.IsAlive)
        {
            return CombatTargetPreviewState.None;
        }

        if (TargetResolver.WouldAffectEnemy(activeCard, enemy, hoveredEnemy))
        {
            return CombatTargetPreviewState.Affected;
        }

        if (TargetResolver.CanSelectEnemy(activeCard, enemy))
        {
            return CombatTargetPreviewState.Selectable;
        }

        if (TargetResolver.HasEnemyEffect(activeCard))
        {
            return CombatTargetPreviewState.Invalid;
        }

        return CombatTargetPreviewState.None;
    }

    private void ShowEnemyDeltaFeedback(GridEnemy enemy, CombatEnemyView enemyView)
    {
        if (enemy == null || enemy.Unit == null)
        {
            return;
        }

        int currentHealth = enemy.Unit.UnitCurrentHealth;
        int currentBlock = enemy.Unit.UnitBlock;

        if (lastEnemyHealth.TryGetValue(enemy, out int previousHealth) && currentHealth < previousHealth)
        {
            enemyView.ShowDamageFeedback(previousHealth - currentHealth);
        }

        if (lastEnemyBlock.TryGetValue(enemy, out int previousBlock))
        {
            int blockDelta = currentBlock - previousBlock;
            enemyView.ShowBlockFeedback(blockDelta);
        }

        lastEnemyHealth[enemy] = currentHealth;
        lastEnemyBlock[enemy] = currentBlock;
    }

    private void ShowPlayerDeltaFeedback(Unit player)
    {
        if (player == null || playerPanel == null)
        {
            return;
        }

        if (lastPlayerHealth >= 0)
        {
            int healthDelta = player.UnitCurrentHealth - lastPlayerHealth;

            if (healthDelta < 0)
            {
                FloatingCombatText.Spawn(playerPanel, healthDelta.ToString(), new Color(0.92f, 0.24f, 0.18f, 1f));
            }
            else if (healthDelta > 0)
            {
                FloatingCombatText.Spawn(playerPanel, $"+{healthDelta}", new Color(0.28f, 0.78f, 0.44f, 1f));
            }
        }

        if (lastPlayerBlock >= 0)
        {
            int blockDelta = player.UnitBlock - lastPlayerBlock;

            if (blockDelta != 0)
            {
                string prefix = blockDelta > 0 ? "+" : "";
                FloatingCombatText.Spawn(playerPanel, $"{prefix}{blockDelta} Block", new Color(0.56f, 0.72f, 0.92f, 1f));
            }
        }

        lastPlayerHealth = player.UnitCurrentHealth;
        lastPlayerBlock = player.UnitBlock;
    }

    private Button CreateCommandButton(Transform parent, string label, bool interactable, Action onClick, float height = 54f)
    {
        GameObject buttonObject = new GameObject("CommandButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();
        image.color = interactable ? commandColor : disabledCommandColor;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.interactable = interactable;
        button.onClick.AddListener(() => onClick?.Invoke());

        TMP_Text labelText = CreateText(buttonObject.transform, "Label", 17, TextAlignmentOptions.Center);
        labelText.text = label;
        Stretch(labelText.rectTransform);

        AddLayout(buttonObject, -1f, height, 1f);
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

    private void AddHorizontalRowLayout(RectTransform rectTransform)
    {
        HorizontalLayoutGroup layout = rectTransform.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
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
