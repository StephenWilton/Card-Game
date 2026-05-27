using System.Collections.Generic;
using UnityEngine;

public class ThreatBoardService
{
    private readonly List<ThreatBoardOptionData> reusableCandidates = new List<ThreatBoardOptionData>();
    private readonly List<ThreatBoardOptionData> reusableSelected = new List<ThreatBoardOptionData>();

    public List<ThreatBoardOption> GenerateOptions(ThreatBoardConfig config, ThreatBoardState state, RunState runState)
    {
        List<ThreatBoardOption> options = new List<ThreatBoardOption>();

        if (config == null || state == null)
        {
            return options;
        }

        reusableCandidates.Clear();

        foreach (ThreatBoardOptionData optionData in config.options)
        {
            if (optionData == null)
            {
                continue;
            }

            if (state.FinalCrisisReady && optionData.outcomeType != ThreatBoardOutcomeType.FinalCrisis)
            {
                continue;
            }

            if (!state.FinalCrisisReady && optionData.outcomeType == ThreatBoardOutcomeType.FinalCrisis)
            {
                continue;
            }

            int copies = Mathf.Max(optionData.baseWeight, 1);

            for (int i = 0; i < copies; i++)
            {
                reusableCandidates.Add(optionData);
            }
        }

        reusableSelected.Clear();
        int optionCount = Mathf.Max(config.optionsPerBoard, 1);

        while (reusableCandidates.Count > 0 && reusableSelected.Count < optionCount)
        {
            int index = Random.Range(0, reusableCandidates.Count);
            ThreatBoardOptionData selectedData = reusableCandidates[index];
            reusableSelected.Add(selectedData);
            reusableCandidates.RemoveAll(optionData => optionData == selectedData);
        }

        int patronSuggestionIndex = ChoosePatronSuggestionIndex(reusableSelected);

        for (int i = 0; i < reusableSelected.Count; i++)
        {
            ThreatBoardOptionData optionData = reusableSelected[i];
            bool patronSuggestion = i == patronSuggestionIndex;
            int projectedCountdown = Mathf.Max(state.CountdownRemaining - Mathf.Max(optionData.countdownCost, config.minimumCountdownCost), 0);
            int projectedThreat = Mathf.Max(state.ThreatLevel + optionData.threatLevelDelta + config.threatLevelIncreasePerChoice, 0);
            int projectedIntegrity = Mathf.Max(state.SafeHavenIntegrity + optionData.safeHavenIntegrityDelta, 0);

            options.Add(new ThreatBoardOption(
                optionData,
                patronSuggestion,
                projectedThreat,
                projectedCountdown,
                projectedIntegrity));
        }

        return options;
    }

    private int ChoosePatronSuggestionIndex(List<ThreatBoardOptionData> options)
    {
        List<int> eligibleIndexes = new List<int>();

        for (int i = 0; i < options.Count; i++)
        {
            if (options[i] != null && options[i].canBePatronSuggested)
            {
                eligibleIndexes.Add(i);
            }
        }

        if (eligibleIndexes.Count == 0)
        {
            return -1;
        }

        return eligibleIndexes[Random.Range(0, eligibleIndexes.Count)];
    }
}
