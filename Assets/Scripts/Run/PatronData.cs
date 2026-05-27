using System.Collections.Generic;
using UnityEngine;

public enum PatronDialogueTrigger
{
    RunStart,
    CombatStart,
    CardSacrificed,
    CardCorrupted,
    RestSiteEntered,
    Prayer,
    Victory,
    Defeat
}

[System.Serializable]
public class PatronDialogueLine
{
    public PatronDialogueTrigger trigger;
    [TextArea] public string line;
}

[CreateAssetMenu(fileName = "New Patron", menuName = "Run/Patron")]
public class PatronData : ScriptableObject
{
    public PatronType patronType = PatronType.TheDevourer;
    public string displayName = "The Devourer";
    public int maxInfluence = 10;
    public Color influenceColor = new Color(0.55f, 0.13f, 0.11f, 1f);
    public List<PatronDialogueLine> dialogueLines = new List<PatronDialogueLine>();

    public string GetLine(PatronDialogueTrigger trigger)
    {
        List<string> matchingLines = new List<string>();

        foreach (PatronDialogueLine dialogueLine in dialogueLines)
        {
            if (dialogueLine != null &&
                dialogueLine.trigger == trigger &&
                !string.IsNullOrWhiteSpace(dialogueLine.line))
            {
                matchingLines.Add(dialogueLine.line);
            }
        }

        if (matchingLines.Count == 0)
        {
            return null;
        }

        return matchingLines[Random.Range(0, matchingLines.Count)];
    }
}
