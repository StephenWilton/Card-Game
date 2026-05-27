using System.Collections.Generic;
using UnityEngine;

public class ThreatBoardController : MonoBehaviour
{
    [Header("Run Setup")]
    [SerializeField] private HeroClassData selectedClass;
    [SerializeField] private PatronData selectedPatron;
    [SerializeField] private ThreatBoardConfig boardConfig;

    [Header("View")]
    [SerializeField] private ThreatBoardView boardView;

    private readonly RunState runState = new RunState();
    private readonly ThreatBoardState boardState = new ThreatBoardState();
    private readonly ThreatBoardService boardService = new ThreatBoardService();
    private readonly List<string> logLines = new List<string>();

    private void Start()
    {
        EnsureView();
        StartBoardRun();
    }

    public void StartBoardRun()
    {
        runState.Initialize(selectedClass, selectedPatron, selectedClass != null ? selectedClass.startingDeck : null);
        boardState.Initialize(boardConfig);
        logLines.Clear();
        AddLog("The town waits behind nailed gates.");
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        List<ThreatBoardOption> options = boardService.GenerateOptions(boardConfig, boardState, runState);
        boardState.SetOptions(options);

        if (boardState.FinalCrisisReady)
        {
            AddLog("The final crisis has reached the walls.");
        }

        RefreshView();
    }

    private void SelectOption(ThreatBoardOption option)
    {
        if (option == null || option.Data == null)
        {
            return;
        }

        ThreatBoardSelectionResult result = new ThreatBoardSelectionResult(option);

        if (result.IsPatronSuggestion)
        {
            runState.GainPatronInfluence(Mathf.Max(option.Data.patronInfluenceReward, 1));
            AddLog($"{runState.PatronDisplayName} approves. Patron Influence rises.");
        }
        else if (option.Data.patronInfluenceReward > 0)
        {
            runState.GainPatronInfluence(option.Data.patronInfluenceReward);
            AddLog($"Patron Influence +{option.Data.patronInfluenceReward}.");
        }

        boardState.ApplyChoice(option, boardConfig);
        AddLog($"Chosen: {option.Data.displayName}.");
        AddOutcomeLog(result);

        if (boardState.FinalCrisisReady)
        {
            int finalIntegrityLoss = boardConfig != null ? Mathf.Max(boardConfig.safeHavenLossAtFinalCrisis, 0) : 0;

            if (finalIntegrityLoss > 0)
            {
                AddLog($"The town loses {finalIntegrityLoss} integrity as the crisis arrives.");
            }
        }

        GenerateBoard();
    }

    private void AddOutcomeLog(ThreatBoardSelectionResult result)
    {
        switch (result.OutcomeType)
        {
            case ThreatBoardOutcomeType.Combat:
                AddLog(result.Encounter != null
                    ? "Combat encounter selected. Scene routing is the next integration step."
                    : "Combat threat selected, but it has no EncounterData assigned.");
                break;

            case ThreatBoardOutcomeType.Trader:
                AddLog("Trader selected. Shop inventory is not implemented yet.");
                break;

            case ThreatBoardOutcomeType.Shrine:
                AddLog("Shrine selected. Rest-site actions are not integrated with the board yet.");
                break;

            case ThreatBoardOutcomeType.TownDecision:
                AddLog("Town decision selected. Safe haven consequences applied.");
                break;

            case ThreatBoardOutcomeType.FinalCrisis:
                AddLog("Final crisis selected. Final encounter/event is still a design placeholder.");
                break;

            default:
                AddLog("Event selected. Event resolution is not implemented yet.");
                break;
        }
    }

    private void RefreshView()
    {
        boardView.Refresh(
            runState,
            boardState,
            logLines,
            SelectOption,
            StartBoardRun);
    }

    private void AddLog(string message)
    {
        logLines.Insert(0, message);

        while (logLines.Count > 8)
        {
            logLines.RemoveAt(logLines.Count - 1);
        }
    }

    private void EnsureView()
    {
        if (boardView != null)
        {
            boardView.BuildIfNeeded();
            return;
        }

        boardView = FindAnyObjectByType<ThreatBoardView>();

        if (boardView == null)
        {
            boardView = ThreatBoardView.CreateRuntime();
        }

        boardView.BuildIfNeeded();
    }
}
