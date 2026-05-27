using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [Header("Run Identity")]
    [SerializeField] private HeroClassData selectedClass;
    [SerializeField] private PatronData selectedPatron;

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

    private readonly RunState runState = new RunState();
    private readonly CardRewardService rewardService = new CardRewardService();
    private readonly RestSiteService restSiteService = new RestSiteService();
    private readonly CombatPlayValidator playValidator = new CombatPlayValidator();
    private readonly List<string> logLines = new List<string>();
    private readonly List<CardData> playedCardsThisTurn = new List<CardData>();
    private readonly List<CardData> lastCompletedPlayerTurnCards = new List<CardData>();

    private DeckRuntime Deck => runState.Deck;

    private EnemyFormation formation;
    private CardResolver cardResolver;
    private EnemyTurnResolver enemyTurnResolver;
    private CombatView view;

    private CombatFlowState currentState;
    private int turnNumber;
    private int currentEnergy;
    private int selectedDeckIndex = -1;
    private CardInstance selectedCard;
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
        cardResolver = new CardResolver(player, formation, Deck, AddLog);
        enemyTurnResolver = new EnemyTurnResolver(player, formation, AddLog);

        view = new CombatView(
            SelectOrPlayCard,
            DropCard,
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

        runState.Initialize(selectedClass, selectedPatron, GetStartingDeckCards());
        player.Initialize(runState.HeroDisplayName, runState.HeroMaxHealth);
        selectedDeckIndex = -1;
        selectedCard = null;
        currentRewardCard = null;
        logLines.Clear();
        view?.ResetTransientState();

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

        if (Deck.Deck.Count == 0)
        {
            currentState = CombatFlowState.Defeat;
            AddLog("No starting deck assigned on the Combat component.");
            RefreshUi();
            return;
        }

        StartCombat();
    }

    private IEnumerable<CardData> GetStartingDeckCards()
    {
        if (selectedClass != null && selectedClass.startingDeck.Count > 0)
        {
            return selectedClass.startingDeck;
        }

        return startingDeckCards;
    }

    private void StartCombat()
    {
        currentState = CombatFlowState.Combat;
        turnNumber = 1;
        currentEnergy = combatRules.maxEnergy;
        selectedCard = null;
        playedCardsThisTurn.Clear();
        lastCompletedPlayerTurnCards.Clear();

        player.ResetForCombat();
        formation.Spawn();
        Deck.DrawNewHand(combatRules.handSize);

        AddPatronLine(PatronDialogueTrigger.CombatStart, "Combat begins. Use energy, pick targets, then end turn.");
        RefreshUi();
    }

    private void StartPlayerTurn()
    {
        turnNumber++;
        currentEnergy = combatRules.maxEnergy;
        selectedCard = null;
        playedCardsThisTurn.Clear();

        player.ClearBlock();
        formation.ClearBlock();
        Deck.DrawNewHand(combatRules.handSize);

        AddLog($"Turn {turnNumber} begins.");
        RefreshUi();
    }

    private void SelectOrPlayCard(CardInstance card)
    {
        if (!CanBeginCardPlay(card))
        {
            return;
        }

        if (TargetResolver.RequiresEnemySelection(card))
        {
            selectedCard = card;
            AddLog($"Choose an enemy target for {card.CardName}.");
            RefreshUi();
            return;
        }

        PlayCard(card, null);
    }

    private void DropCard(CardInstance card, GameObject dropTarget)
    {
        if (!CanBeginCardPlay(card))
        {
            return;
        }

        if (TargetResolver.RequiresEnemySelection(card))
        {
            EnemyTargetView targetView = dropTarget != null ? dropTarget.GetComponentInParent<EnemyTargetView>() : null;

            if (targetView != null && targetView.Enemy != null && targetView.Enemy.IsAlive)
            {
                PlayCard(card, targetView.Enemy);
                return;
            }

            AddLog($"{card.CardName} needs a living enemy target.");
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

    private void PlayCard(CardInstance card, GridEnemy selectedEnemy)
    {
        if (!playValidator.CanPlay(currentState, Deck, currentEnergy, card, selectedEnemy, out string failureReason))
        {
            if (!string.IsNullOrWhiteSpace(failureReason))
            {
                AddLog(failureReason);
                RefreshUi();
            }

            return;
        }

        currentEnergy -= card.EnergyCost;
        Deck.PlayCard(card);
        playedCardsThisTurn.Add(card.CardData);
        selectedCard = null;

        AddLog($"Played {card.CardName}.");
        cardResolver.Resolve(card, selectedEnemy);

        if (!TryResolveCombatEnd())
        {
            RefreshUi();
        }
    }

    private bool CanBeginCardPlay(CardInstance card)
    {
        if (currentState != CombatFlowState.Combat)
        {
            return false;
        }

        if (card == null || card.CardData == null || !Deck.Hand.Contains(card))
        {
            return false;
        }

        if (currentEnergy < card.EnergyCost)
        {
            AddLog($"Not enough energy for {card.CardName}.");
            RefreshUi();
            return false;
        }

        return true;
    }

    private void EndPlayerTurn()
    {
        if (currentState != CombatFlowState.Combat)
        {
            return;
        }

        selectedCard = null;
        lastCompletedPlayerTurnCards.Clear();
        lastCompletedPlayerTurnCards.AddRange(playedCardsThisTurn);
        playedCardsThisTurn.Clear();
        Deck.DiscardHand();

        AddLog("Player ends turn.");
        enemyTurnResolver.Resolve(lastCompletedPlayerTurnCards);

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
            AddPatronLine(PatronDialogueTrigger.Defeat, $"The {runState.HeroDisplayName} falls.");
            RefreshUi();
            return true;
        }

        return false;
    }

    private void EnterReward()
    {
        currentState = CombatFlowState.Reward;
        currentRewardCard = rewardService.PickReward(selectedClass, rewardCards);
        selectedCard = null;
        Deck.Hand.Clear();
        AddLog("Combat won. Choose a reward or sacrifice it for Patron Influence.");
    }

    private void TakeReward()
    {
        if (currentRewardCard != null)
        {
            Deck.AddCard(currentRewardCard);
            AddLog($"{currentRewardCard.cardName} added to the deck.");
        }

        EnterShrine();
    }

    private void SacrificeReward()
    {
        runState.GainPatronInfluence(1);
        AddLog("Reward sacrificed. Patron Influence +1.");
        AddPatronLine(PatronDialogueTrigger.CardSacrificed, "A gift refused is still a gift to me.");
        EnterShrine();
    }

    private void EnterShrine()
    {
        currentState = CombatFlowState.Shrine;
        selectedDeckIndex = Deck.Deck.Count > 0 ? 0 : -1;
        AddPatronLine(PatronDialogueTrigger.RestSiteEntered, $"{runState.PatronDisplayName}'s shrine waits.");
        RefreshUi();
    }

    private void SelectDeckCard(int cardIndex)
    {
        if (currentState != CombatFlowState.Shrine || cardIndex < 0 || cardIndex >= Deck.Deck.Count)
        {
            return;
        }

        selectedDeckIndex = cardIndex;
        RefreshUi();
    }

    private void HealAtShrine()
    {
        player.Heal(shrineHealAmount);
        AddPatronLine(PatronDialogueTrigger.Prayer, "Kneel, and be less broken.");
        CompleteRun($"The shrine heals the Paladin for {shrineHealAmount}.");
    }

    private void UpgradeSelectedCard()
    {
        if (!restSiteService.TryUpgrade(Deck, selectedDeckIndex, out string message))
        {
            return;
        }

        CompleteRun(message);
    }

    private void CorruptSelectedCard()
    {
        if (!restSiteService.TryCorrupt(runState, selectedDeckIndex, patronCorruptCost, out string message))
        {
            return;
        }

        AddPatronLine(PatronDialogueTrigger.CardCorrupted, "Something holy gives way to hunger.");
        CompleteRun(message);
    }

    private bool SelectedCardCanUpgrade()
    {
        return restSiteService.CanUpgrade(Deck, selectedDeckIndex);
    }

    private bool SelectedCardCanCorrupt()
    {
        return restSiteService.CanCorrupt(runState, selectedDeckIndex, patronCorruptCost);
    }

    private void CompleteRun(string message)
    {
        currentState = CombatFlowState.Complete;
        AddLog(message);
        AddPatronLine(PatronDialogueTrigger.Victory, "Run complete.");
        RefreshUi();
    }

    private void AddPatronLine(PatronDialogueTrigger trigger, string fallback)
    {
        string patronLine = runState.Patron != null ? runState.Patron.GetLine(trigger) : null;

        if (string.IsNullOrWhiteSpace(patronLine))
        {
            AddLog(fallback);
            return;
        }

        AddLog($"{runState.PatronDisplayName}: {patronLine}");
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
            runState.HeroDisplayName,
            runState.PatronDisplayName,
            player,
            formation,
            Deck,
            turnNumber,
            currentEnergy,
            combatRules != null ? combatRules.maxEnergy : 0,
            runState.PatronInfluence,
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
