public class ThreatBoardSelectionResult
{
    public ThreatBoardOption Option { get; }
    public EncounterData Encounter => Option != null && Option.Data != null ? Option.Data.encounter : null;
    public ThreatBoardOutcomeType OutcomeType => Option != null && Option.Data != null ? Option.Data.outcomeType : ThreatBoardOutcomeType.Event;
    public bool IsPatronSuggestion => Option != null && Option.IsPatronSuggestion;

    public ThreatBoardSelectionResult(ThreatBoardOption option)
    {
        Option = option;
    }
}
