using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Threat Board Config", menuName = "Run/Threat Board/Config")]
public class ThreatBoardConfig : ScriptableObject
{
    [Header("Run Clock")]
    public int startingCountdown = 8;
    public int startingThreatLevel = 1;
    public int startingSafeHavenIntegrity = 10;
    public int optionsPerBoard = 4;

    [Header("Pressure")]
    public int minimumCountdownCost = 1;
    public int threatLevelIncreasePerChoice = 1;
    public int safeHavenLossAtFinalCrisis = 0;

    [Header("Board Options")]
    public List<ThreatBoardOptionData> options = new List<ThreatBoardOptionData>();
}
