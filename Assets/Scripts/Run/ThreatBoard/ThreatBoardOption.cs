public class ThreatBoardOption
{
    public ThreatBoardOptionData Data { get; }
    public bool IsPatronSuggestion { get; }
    public int ProjectedThreatLevel { get; }
    public int ProjectedCountdown { get; }
    public int ProjectedSafeHavenIntegrity { get; }

    public ThreatBoardOption(
        ThreatBoardOptionData data,
        bool isPatronSuggestion,
        int projectedThreatLevel,
        int projectedCountdown,
        int projectedSafeHavenIntegrity)
    {
        Data = data;
        IsPatronSuggestion = isPatronSuggestion;
        ProjectedThreatLevel = projectedThreatLevel;
        ProjectedCountdown = projectedCountdown;
        ProjectedSafeHavenIntegrity = projectedSafeHavenIntegrity;
    }
}
