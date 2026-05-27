using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatView
{
    private readonly Action<CardInstance> onCardClicked;
    private readonly Action<CardInstance, GameObject> onCardDropped;
    private readonly Action<GridEnemy> onEnemyClicked;
    private readonly Action onEndTurnClicked;
    private readonly Action onTakeRewardClicked;
    private readonly Action onSacrificeRewardClicked;
    private readonly Action onHealClicked;
    private readonly Action onUpgradeClicked;
    private readonly Action onCorruptClicked;
    private readonly Action<int> onDeckCardClicked;
    private readonly Action onRestartClicked;

    private Canvas canvas;
    private TMP_Text titleText;
    private TMP_Text statusText;
    private TMP_Text playerText;
    private TMP_Text deckText;
    private TMP_Text logText;
    private RectTransform enemyGridPanel;
    private RectTransform handPanel;
    private RectTransform choicePanel;

    private readonly Color backgroundColor = new Color(0.08f, 0.06f, 0.05f, 1f);
    private readonly Color panelColor = new Color(0.14f, 0.11f, 0.09f, 0.95f);
    private readonly Color buttonColor = new Color(0.34f, 0.24f, 0.18f, 1f);
    private readonly Color selectedButtonColor = new Color(0.55f, 0.38f, 0.2f, 1f);

    public CombatView(
        Action<CardInstance> onCardClicked,
        Action<CardInstance, GameObject> onCardDropped,
        Action<GridEnemy> onEnemyClicked,
        Action onEndTurnClicked,
        Action onTakeRewardClicked,
        Action onSacrificeRewardClicked,
        Action onHealClicked,
        Action onUpgradeClicked,
        Action onCorruptClicked,
        Action<int> onDeckCardClicked,
        Action onRestartClicked)
    {
        this.onCardClicked = onCardClicked;
        this.onCardDropped = onCardDropped;
        this.onEnemyClicked = onEnemyClicked;
        this.onEndTurnClicked = onEndTurnClicked;
        this.onTakeRewardClicked = onTakeRewardClicked;
        this.onSacrificeRewardClicked = onSacrificeRewardClicked;
        this.onHealClicked = onHealClicked;
        this.onUpgradeClicked = onUpgradeClicked;
        this.onCorruptClicked = onCorruptClicked;
        this.onDeckCardClicked = onDeckCardClicked;
        this.onRestartClicked = onRestartClicked;
    }

    public void Build()
    {
        canvas = UnityEngine.Object.FindAnyObjectByType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("CombatCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        ClearChildren(canvas.transform);

        RectTransform root = CreatePanel(canvas.transform, "CombatRoot", backgroundColor);
        Stretch(root);

        VerticalLayoutGroup rootLayout = root.gameObject.AddComponent<VerticalLayoutGroup>();
        rootLayout.padding = new RectOffset(24, 24, 20, 20);
        rootLayout.spacing = 12;
        rootLayout.childControlWidth = true;
        rootLayout.childControlHeight = true;
        rootLayout.childForceExpandWidth = true;
        rootLayout.childForceExpandHeight = false;

        titleText = CreateText(root, "Title", 34, TextAlignmentOptions.Center);
        AddLayout(titleText.gameObject, -1, 48);

        statusText = CreateText(root, "Status", 24, TextAlignmentOptions.Center);
        AddLayout(statusText.gameObject, -1, 50);

        RectTransform statusRow = CreatePanel(root, "StatusRow", new Color(0f, 0f, 0f, 0f));
        AddLayout(statusRow.gameObject, -1, 145);
        HorizontalLayoutGroup statusLayout = statusRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        statusLayout.spacing = 12;
        statusLayout.childControlWidth = true;
        statusLayout.childControlHeight = true;
        statusLayout.childForceExpandWidth = true;

        playerText = CreateText(statusRow, "PlayerText", 24, TextAlignmentOptions.Left);
        deckText = CreateText(statusRow, "DeckText", 18, TextAlignmentOptions.Left);

        RectTransform middleRow = CreatePanel(root, "MiddleRow", new Color(0f, 0f, 0f, 0f));
        AddLayout(middleRow.gameObject, -1, 330);
        HorizontalLayoutGroup middleLayout = middleRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        middleLayout.spacing = 12;
        middleLayout.childControlWidth = true;
        middleLayout.childControlHeight = true;
        middleLayout.childForceExpandWidth = true;

        enemyGridPanel = CreatePanel(middleRow, "EnemyGrid", panelColor);
        AddPadding(enemyGridPanel, 12);

        choicePanel = CreatePanel(middleRow, "ChoicePanel", panelColor);
        AddPadding(choicePanel, 12);

        handPanel = CreatePanel(root, "HandPanel", panelColor);
        AddLayout(handPanel.gameObject, -1, 210);
        HorizontalLayoutGroup handLayout = handPanel.gameObject.AddComponent<HorizontalLayoutGroup>();
        handLayout.padding = new RectOffset(12, 12, 12, 12);
        handLayout.spacing = 10;
        handLayout.childControlWidth = true;
        handLayout.childControlHeight = true;
        handLayout.childForceExpandWidth = true;

        logText = CreateText(root, "Log", 18, TextAlignmentOptions.Left);
        AddLayout(logText.gameObject, -1, 150);
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
        CardInstance selectedCard,
        int selectedDeckIndex,
        CardData currentRewardCard,
        IReadOnlyList<string> logLines,
        int shrineHealAmount,
        int patronCorruptCost,
        bool canUpgrade,
        bool canCorrupt)
    {
        titleText.text = $"{heroName} / {patronName}     Patron Influence: {patronInfluence}";
        statusText.text = GetStatusText(state, turnNumber, currentEnergy, maxEnergy, selectedCard);
        playerText.text = GetPlayerText(player);
        deckText.text = GetDeckText(deck, selectedDeckIndex);
        logText.text = string.Join("\n", logLines);

        RenderEnemyGrid(formation, selectedCard);
        RenderHand(state, deck, currentEnergy, selectedCard);
        RenderChoices(state, deck, selectedDeckIndex, currentRewardCard, shrineHealAmount, patronCorruptCost, canUpgrade, canCorrupt);
    }

    public void UpdateLog(IReadOnlyList<string> logLines)
    {
        if (logText != null)
        {
            logText.text = string.Join("\n", logLines);
        }
    }

    private string GetStatusText(CombatFlowState state, int turnNumber, int currentEnergy, int maxEnergy, CardInstance selectedCard)
    {
        if (state != CombatFlowState.Combat)
        {
            return $"State: {state}";
        }

        string selectedText = selectedCard == null ? "No card selected" : $"Targeting with {selectedCard.CardName}";
        return $"Turn {turnNumber}   Energy: {currentEnergy}/{maxEnergy}   {selectedText}";
    }

    private string GetPlayerText(Unit player)
    {
        return $"{player.UnitName}\nHP: {player.UnitCurrentHealth}/{player.UnitMaxHealth}\nBlock: {player.UnitBlock}";
    }

    private string GetDeckText(DeckRuntime deck, int selectedDeckIndex)
    {
        string text = $"Deck: {deck.Deck.Count}   Draw: {deck.DrawPile.Count}   Discard: {deck.DiscardPile.Count}\n";

        for (int i = 0; i < deck.Deck.Count; i++)
        {
            CardData cardData = deck.Deck[i].CardData;
            string marker = i == selectedDeckIndex ? "> " : "  ";
            text += $"{marker}{i + 1}. {deck.Deck[i].CardName}";

            if (cardData != null && cardData.isCorrupted)
            {
                text += " [Corrupted]";
            }

            text += "\n";
        }

        return text;
    }

    private void RenderEnemyGrid(EnemyFormation formation, CardInstance selectedCard)
    {
        ClearChildren(enemyGridPanel);
        CreateText(enemyGridPanel, "EnemyGridTitle", 24, TextAlignmentOptions.Center).text = "Enemy Formation";

        for (int row = 1; row >= 0; row--)
        {
            RectTransform rowPanel = CreatePanel(enemyGridPanel, row == 0 ? "FrontRow" : "BackRow", new Color(0f, 0f, 0f, 0f));
            AddLayout(rowPanel.gameObject, -1, 92);

            HorizontalLayoutGroup rowLayout = rowPanel.gameObject.AddComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 8;
            rowLayout.childControlWidth = true;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = true;

            for (int column = 0; column < formation.GridColumns; column++)
            {
                GridEnemy gridEnemy = formation.GetEnemyAt(row, column);

                if (gridEnemy == null || !gridEnemy.IsAlive)
                {
                    CreateText(rowPanel, "EmptySlot", 18, TextAlignmentOptions.Center).text =
                        row == 0 ? $"Front {column + 1}\nEmpty" : $"Back {column + 1}\nEmpty";
                    continue;
                }

                GridEnemy enemyToTarget = gridEnemy;
                Button enemyButton = CreateButton(rowPanel, GetEnemyButtonText(gridEnemy), () => onEnemyClicked(enemyToTarget));
                enemyButton.interactable = selectedCard != null && TargetResolver.RequiresEnemySelection(selectedCard);
                enemyButton.gameObject.AddComponent<EnemyTargetView>().Bind(enemyToTarget);
            }
        }
    }

    private string GetEnemyButtonText(GridEnemy gridEnemy)
    {
        return $"{gridEnemy.PositionName}\n{gridEnemy.Unit.UnitName}\n" +
               $"HP {gridEnemy.Unit.UnitCurrentHealth}/{gridEnemy.Unit.UnitMaxHealth}  Block {gridEnemy.Unit.UnitBlock}\n" +
               $"Intent: Attack {gridEnemy.AttackDamage}";
    }

    private void RenderHand(CombatFlowState state, DeckRuntime deck, int currentEnergy, CardInstance selectedCard)
    {
        ClearChildren(handPanel);

        if (state != CombatFlowState.Combat)
        {
            CreateText(handPanel, "HandClosed", 20, TextAlignmentOptions.Center).text = "Cards are only playable during combat.";
            return;
        }

        foreach (CardInstance card in deck.Hand)
        {
            CardInstance cardToPlay = card;
            bool isPlayable = currentEnergy >= card.EnergyCost;
            Button cardButton = CreateButton(handPanel, GetCardButtonText(card.CardData), null);
            cardButton.interactable = isPlayable;
            cardButton.gameObject.AddComponent<CardDragView>().Bind(cardToPlay, canvas, onCardClicked, onCardDropped, isPlayable);

            if (selectedCard == card)
            {
                cardButton.image.color = selectedButtonColor;
            }
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
        }
        else if (state == CombatFlowState.Shrine)
        {
            RenderShrineChoices(deck, selectedDeckIndex, shrineHealAmount, patronCorruptCost, canUpgrade, canCorrupt);
        }
        else if (state == CombatFlowState.Complete || state == CombatFlowState.Defeat)
        {
            CreateText(choicePanel, "CompleteText", 22, TextAlignmentOptions.Center).text = state == CombatFlowState.Complete
                ? "Run complete."
                : "Defeat. Restart the run.";
            CreateButton(choicePanel, "Restart Run", onRestartClicked);
        }
        else
        {
            CreateText(choicePanel, "CombatHint", 22, TextAlignmentOptions.Center).text =
                "Cards can hit one target, pierce a column, or hit rows. Pick target cards, then click an enemy.";
            CreateButton(choicePanel, "End Turn", onEndTurnClicked);
        }
    }

    private void RenderRewardChoices(CardData currentRewardCard)
    {
        string rewardName = currentRewardCard != null ? currentRewardCard.cardName : "No Reward";
        string rewardText = currentRewardCard != null ? currentRewardCard.cardDescription : "Continue to shrine.";

        CreateText(choicePanel, "RewardTitle", 24, TextAlignmentOptions.Center).text = $"Card Reward\n{rewardName}\n{rewardText}";
        CreateButton(choicePanel, "Add Card To Deck", onTakeRewardClicked).interactable = currentRewardCard != null;
        CreateButton(choicePanel, "Sacrifice For +1 Patron Influence", onSacrificeRewardClicked).interactable = currentRewardCard != null;
    }

    private void RenderShrineChoices(DeckRuntime deck, int selectedDeckIndex, int shrineHealAmount, int patronCorruptCost, bool canUpgrade, bool canCorrupt)
    {
        CreateText(choicePanel, "ShrineTitle", 24, TextAlignmentOptions.Center).text = "Shrine of The Devourer";

        for (int i = 0; i < deck.Deck.Count; i++)
        {
            int cardIndex = i;
            string label = i == selectedDeckIndex ? $"Selected: {deck.Deck[i].CardName}" : $"Select {deck.Deck[i].CardName}";
            CreateButton(choicePanel, label, () => onDeckCardClicked(cardIndex));
        }

        CreateButton(choicePanel, $"Heal {shrineHealAmount}", onHealClicked);

        Button upgradeButton = CreateButton(choicePanel, "Upgrade Selected Card", onUpgradeClicked);
        upgradeButton.interactable = canUpgrade;

        Button corruptButton = CreateButton(choicePanel, $"Corrupt Selected Card ({patronCorruptCost} Influence)", onCorruptClicked);
        corruptButton.interactable = canCorrupt;
    }

    private string GetCardButtonText(CardData card)
    {
        if (card == null)
        {
            return "Missing Card";
        }

        string corruptTag = card.isCorrupted ? " [Corrupted]" : "";
        return $"{card.cardName}{corruptTag}\nCost {card.energyCost}\n{GetTargetText(card)}\n{card.cardDescription}";
    }

    private string GetTargetText(CardData card)
    {
        foreach (CardActionData action in card.actions)
        {
            if (action.target == CardTarget.PierceColumn)
            {
                return "Target: column pierce";
            }

            if (action.target == CardTarget.Enemy)
            {
                return "Target: 1 enemy";
            }

            if (action.target == CardTarget.FirstRow)
            {
                return "Target: front row";
            }

            if (action.target == CardTarget.BackRow)
            {
                return "Target: back row";
            }

            if (action.target == CardTarget.AllEnemies)
            {
                return "Target: all enemies";
            }
        }

        foreach (CardActionData action in card.actions)
        {
            if (action.target == CardTarget.Player)
            {
                return "Target: self";
            }
        }

        return "Target: none";
    }

    private RectTransform CreatePanel(Transform parent, string objectName, Color color)
    {
        GameObject panelObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        Image image = panelObject.GetComponent<Image>();
        image.color = color;

        return panelObject.GetComponent<RectTransform>();
    }

    private TMP_Text CreateText(Transform parent, string objectName, int fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = new Color(0.88f, 0.82f, 0.72f, 1f);
        text.textWrappingMode = TextWrappingModes.Normal;

        return text;
    }

    private Button CreateButton(Transform parent, string label, Action onClick)
    {
        GameObject buttonObject = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();
        image.color = buttonColor;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => onClick?.Invoke());

        TMP_Text labelText = CreateText(buttonObject.transform, "Label", 16, TextAlignmentOptions.Center);
        labelText.text = label;
        Stretch(labelText.rectTransform);

        AddLayout(buttonObject, -1, 82);
        return button;
    }

    private void AddPadding(RectTransform rectTransform, int padding)
    {
        VerticalLayoutGroup layout = rectTransform.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(padding, padding, padding, padding);
        layout.spacing = 8;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
    }

    private void AddLayout(GameObject gameObject, float preferredWidth, float preferredHeight)
    {
        LayoutElement layout = gameObject.GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
        layout.preferredWidth = preferredWidth;
        layout.preferredHeight = preferredHeight;
        layout.flexibleWidth = 1;
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
            UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
        }
    }
}
