using System.Collections.Generic;

public class ThreatBoardState
{
    public int CountdownRemaining { get; private set; }
    public int ThreatLevel { get; private set; }
    public int SafeHavenIntegrity { get; private set; }
    public int ChoicesMade { get; private set; }
    public bool FinalCrisisReady => CountdownRemaining <= 0;
    public IReadOnlyList<ThreatBoardOption> CurrentOptions => currentOptions;

    private readonly List<ThreatBoardOption> currentOptions = new List<ThreatBoardOption>();

    public void Initialize(ThreatBoardConfig config)
    {
        CountdownRemaining = config != null ? System.Math.Max(config.startingCountdown, 0) : 8;
        ThreatLevel = config != null ? System.Math.Max(config.startingThreatLevel, 0) : 1;
        SafeHavenIntegrity = config != null ? System.Math.Max(config.startingSafeHavenIntegrity, 0) : 10;
        ChoicesMade = 0;
        currentOptions.Clear();
    }

    public void SetOptions(IEnumerable<ThreatBoardOption> options)
    {
        currentOptions.Clear();

        if (options == null)
        {
            return;
        }

        foreach (ThreatBoardOption option in options)
        {
            if (option != null)
            {
                currentOptions.Add(option);
            }
        }
    }

    public void ApplyChoice(ThreatBoardOption option, ThreatBoardConfig config)
    {
        if (option == null || option.Data == null)
        {
            return;
        }

        int minimumCountdownCost = config != null ? System.Math.Max(config.minimumCountdownCost, 0) : 1;
        int globalThreatIncrease = config != null ? System.Math.Max(config.threatLevelIncreasePerChoice, 0) : 1;
        int countdownCost = System.Math.Max(option.Data.countdownCost, minimumCountdownCost);

        CountdownRemaining = System.Math.Max(CountdownRemaining - countdownCost, 0);
        ThreatLevel = System.Math.Max(ThreatLevel + option.Data.threatLevelDelta + globalThreatIncrease, 0);
        SafeHavenIntegrity = System.Math.Max(SafeHavenIntegrity + option.Data.safeHavenIntegrityDelta, 0);
        ChoicesMade++;
        currentOptions.Clear();
    }
}
