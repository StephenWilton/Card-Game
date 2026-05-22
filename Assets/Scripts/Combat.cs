using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [Header("Units")]
    [SerializeField] private Unit player;

    [Header("Cards")]
    [SerializeField] private List<CardData> startingDeckCards = new List<CardData>();
    [SerializeField] private List<CardData> rewardCards = new List<CardData>();

    [Header("Combat Setup")]
    [SerializeField] private CombatRulesData combatRules;
    [SerializeField] private EncounterData encounter;

    [Header("Shrine")]
    [SerializeField] private int shrineHealAmount = 12;
    [SerializeField] private int patronCorruptCost = 1;

    private readonly DeckRuntime deck = new DeckRuntime();
    private readonly List<string> logLines = new List<string>();

    private EnemyFormation formation;
    private CardResolver cardResolver;
    private EnemyTurnResolver enemyTurnResolver;
    private CombatView view;

    private CombatFlowState currentState;
    private int turnNumber;
    private int currentEnergy;
    private int patronInfluence;
    private int selectedDeckIndex = -1;
    private CardData selectedCard;
    private CardData currentRewardCard;

    private void Start()
    {
        EnsurePlayer();
        CreateSystems();
        view.Build();
        StartRun();
    }

    private void OnDestroy()
    {
        formation?.Clear();
    }

    private void CreateSystems()
    {
        formation = new EnemyFormation(encounter);
        cardResolver = new CardResolver(player, formation, deck, AddLog);
        enemyTurnResolver = new EnemyTurnResolver(player, formation, AddLog);

        view = new CombatView(
            SelectOrPlayCard,
            TargetEnemy,
            EndPlayerTurn,
            TakeReward,
            SacrificeReward,
            HealAtShrine,
            UpgradeSelectedCard,
            CorruptSelectedCard,
            SelectDeckCard,
            StartRun);
    }

    private void StartRun()
    {
        EnsurePlayer();

        player.Initialize("Paladin", 42);
        patronInfluence = 0;
        selectedDeckIndex = -1;
        selectedCard = null;
        currentRewardCard = null;
        logLines.Clear();

        deck.Initialize(startingDeckCards);

        if (combatRules == null)
        {
            currentState = CombatFlowState.Defeat;
            AddLog("No combat rules assigned on the Combat component.");
            RefreshUi();
            return;
        }

        if (encounter == null || encounter.enemies.Count == 0)
        {
            currentState = CombatFlowState.Defeat;
            AddLog("No encounter assigned on the Combat component.");
            RefreshUi();
            return;
        }

        if (deck.Deck.Count == 0)
        {
            currentState = CombatFlowState.Defeat;
            AddLog("No starting deck assigned on the Combat component.");
            RefreshUi();
            return;
        }

        StartCombat();
    }

    private void StartCombat()
    {
        currentState = CombatFlowState.Combat;
        turnNumber = 1;
        currentEnergy = combatRules.maxEnergy;
        selectedCard = null;

        player.ResetForCombat();
        formation.Spawn();
        deck.DrawNewHand(combatRules.handSize);

        AddLog("Combat begins. Use energy, pick targets, then end turn.");
        RefreshUi();
    }

    private void StartPlayerTurn()
    {
        turnNumber++;
        currentEnergy = combatRules.maxEnergy;
        selectedCard = null;

        player.ClearBlock();
        formation.ClearBlock();
        deck.DrawNewHand(combatRules.handSize);

        AddLog($"Turn {turnNumber} begins.");
        RefreshUi();
    }

    private void SelectOrPlayCard(CardData card)
    {
        if (currentState != CombatFlowState.Combat || card == null || !deck.Hand.Contains(card))
        {
            return;
        }

        if (currentEnergy < card.energyCost)
        {
            AddLog($"Not enough energy for {card.cardName}.");
            RefreshUi();
            return;
        }

        if (TargetResolver.RequiresEnemySelection(card))
        {
            selectedCard = card;
            AddLog($"Choose an enemy target for {card.cardName}.");
            RefreshUi();
            return;
        }

        PlayCard(card, null);
    }

    private void TargetEnemy(GridEnemy target)
    {
        if (selectedCard == null || target == null || !target.IsAlive)
        {
            return;
        }

        PlayCard(selectedCard, target);
    }

    private void PlayCard(CardData card, GridEnemy selectedEnemy)
    {
        if (currentState != CombatFlowState.Combat || card == null || !deck.Hand.Contains(card) || currentEnergy < card.energyCost)
        {
            return;
        }

        currentEnergy -= card.energyCost;
        deck.PlayCard(card);
        selectedCard = null;

        AddLog($"Played {card.cardName}.");
        cardResolver.Resolve(card, selectedEnemy);

        if (!TryResolveCombatEnd())
        {
            RefreshUi();
        }
    }

    private void EndPlayerTurn()
    {
        if (currentState != CombatFlowState.Combat)
        {
            return;
        }

        selectedCard = null;
        deck.DiscardHand();

        AddLog("Player ends turn.");
        enemyTurnResolver.Resolve();

        if (!TryResolveCombatEnd())
        {
            StartPlayerTurn();
        }
    }

    private bool TryResolveCombatEnd()
    {
        if (formation.AllEnemiesDefeated())
        {
            EnterReward();
            RefreshUi();
            return true;
        }

        if (player.IsDead)
        {
            currentState = CombatFlowState.Defeat;
            AddLog("The Paladin falls.");
            RefreshUi();
            return true;
        }

        return false;
    }

    private void EnterReward()
    {
        currentState = CombatFlowState.Reward;
        currentRewardCard = rewardCards.Count > 0 ? rewardCards[0] : null;
        selectedCard = null;
        deck.Hand.Clear();
        AddLog("Combat won. Choose a reward or sacrifice it for Patron Influence.");
    }

    private void TakeReward()
    {
        if (currentRewardCard != null)
        {
            deck.AddCard(currentRewardCard);
            AddLog($"{currentRewardCard.cardName} added to the deck.");
        }

        EnterShrine();
    }

    private void SacrificeReward()
    {
        patronInfluence++;
        AddLog("Reward sacrificed. Patron Influence +1.");
        EnterShrine();
    }

    private void EnterShrine()
    {
        currentState = CombatFlowState.Shrine;
        selectedDeckIndex = deck.Deck.Count > 0 ? 0 : -1;
        AddLog("The Devourer's shrine waits.");
        RefreshUi();
    }

    private void SelectDeckCard(int cardIndex)
    {
        if (currentState != CombatFlowState.Shrine || cardIndex < 0 || cardIndex >= deck.Deck.Count)
        {
            return;
        }

        selectedDeckIndex = cardIndex;
        RefreshUi();
    }

    private void HealAtShrine()
    {
        player.Heal(shrineHealAmount);
        CompleteRun($"The shrine heals the Paladin for {shrineHealAmount}.");
    }

    private void UpgradeSelectedCard()
    {
        if (!SelectedCardCanUpgrade())
        {
            return;
        }

        CardData oldCard = deck.Deck[selectedDeckIndex];
        deck.ReplaceCard(selectedDeckIndex, oldCard.upgradeCardData);
        CompleteRun($"{oldCard.cardName} upgraded to {deck.Deck[selectedDeckIndex].cardName}.");
    }

    private void CorruptSelectedCard()
    {
        if (!SelectedCardCanCorrupt())
        {
            return;
        }

        CardData oldCard = deck.Deck[selectedDeckIndex];
        patronInfluence -= patronCorruptCost;
        deck.ReplaceCard(selectedDeckIndex, oldCard.corruptedCardData);
        CompleteRun($"{oldCard.cardName} becomes {deck.Deck[selectedDeckIndex].cardName}.");
    }

    private bool SelectedCardCanUpgrade()
    {
        return selectedDeckIndex >= 0 &&
               selectedDeckIndex < deck.Deck.Count &&
               deck.Deck[selectedDeckIndex].upgradeCardData != null;
    }

    private bool SelectedCardCanCorrupt()
    {
        return selectedDeckIndex >= 0 &&
               selectedDeckIndex < deck.Deck.Count &&
               patronInfluence >= patronCorruptCost &&
               deck.Deck[selectedDeckIndex].corruptedCardData != null;
    }

    private void CompleteRun(string message)
    {
        currentState = CombatFlowState.Complete;
        AddLog(message);
        AddLog("Run complete.");
        RefreshUi();
    }

    private void AddLog(string message)
    {
        logLines.Insert(0, message);

        while (logLines.Count > 8)
        {
            logLines.RemoveAt(logLines.Count - 1);
        }

        view?.UpdateLog(logLines);
    }

    private void RefreshUi()
    {
        view.Refresh(
            currentState,
            player,
            formation,
            deck,
            turnNumber,
            currentEnergy,
            combatRules != null ? combatRules.maxEnergy : 0,
            patronInfluence,
            selectedCard,
            selectedDeckIndex,
            currentRewardCard,
            logLines,
            shrineHealAmount,
            patronCorruptCost,
            SelectedCardCanUpgrade(),
            SelectedCardCanCorrupt());
    }

    private void EnsurePlayer()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.Find("Player") ?? new GameObject("Player");
            player = playerObject.GetComponent<Unit>() ?? playerObject.AddComponent<Unit>();
        }
    }
}
