using UnityEngine;

public enum ThreatBoardOptionType
{
    NormalThreat,
    EliteThreat,
    Trader,
    Shrine,
    RandomEvent,
    PatronGuided,
    TownDecision,
    FinalCrisis
}

public enum ThreatBoardOutcomeType
{
    Combat,
    Trader,
    Shrine,
    Event,
    TownDecision,
    FinalCrisis
}

[CreateAssetMenu(fileName = "New Threat Board Option", menuName = "Run/Threat Board/Option")]
public class ThreatBoardOptionData : ScriptableObject
{
    public string displayName = "New Threat";
    public ThreatBoardOptionType optionType = ThreatBoardOptionType.NormalThreat;
    public ThreatBoardOutcomeType outcomeType = ThreatBoardOutcomeType.Combat;
    [TextArea] public string description;
    public EncounterData encounter;
    public int countdownCost = 1;
    public int threatLevelDelta = 1;
    public int safeHavenIntegrityDelta = 0;
    public int patronInfluenceReward = 0;
    public int baseWeight = 1;
    public bool canBePatronSuggested = true;
}
