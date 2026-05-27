#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CardAssetValidator
{
    [MenuItem("Tools/Card Game/Validate Card Assets")]
    public static void ValidateCards()
    {
        string[] cardGuids = AssetDatabase.FindAssets("t:CardData");
        int errorCount = 0;
        int warningCount = 0;

        foreach (string cardGuid in cardGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(cardGuid);
            CardData card = AssetDatabase.LoadAssetAtPath<CardData>(path);

            if (card == null)
            {
                continue;
            }

            ValidateCard(card, path, ref errorCount, ref warningCount);
        }

        if (errorCount == 0 && warningCount == 0)
        {
            Debug.Log($"Card validation passed for {cardGuids.Length} card assets.");
            return;
        }

        Debug.LogWarning($"Card validation finished with {errorCount} error(s) and {warningCount} warning(s).");
    }

    private static void ValidateCard(CardData card, string path, ref int errorCount, ref int warningCount)
    {
        if (string.IsNullOrWhiteSpace(card.cardName))
        {
            LogError(path, "Card is missing a display name.", ref errorCount);
        }

        if (string.IsNullOrWhiteSpace(card.cardDescription))
        {
            LogWarning(path, "Card is missing rules text.", ref warningCount);
        }

        if (card.energyCost < 0)
        {
            LogError(path, "Card energy cost cannot be negative.", ref errorCount);
        }

        if (card.actions == null || card.actions.Count == 0)
        {
            LogWarning(path, "Card has no actions.", ref warningCount);
            return;
        }

        for (int i = 0; i < card.actions.Count; i++)
        {
            ValidateAction(card, card.actions[i], i, path, ref errorCount, ref warningCount);
        }

        if (card.upgradeCardData == card)
        {
            LogError(path, "Card upgrade points to itself.", ref errorCount);
        }

        if (card.corruptedCardData == card)
        {
            LogError(path, "Card corruption points to itself.", ref errorCount);
        }

        if (card.isCorrupted && card.patronType == PatronType.None)
        {
            LogWarning(path, "Corrupted card has no patron type.", ref warningCount);
        }
    }

    private static void ValidateAction(CardData card, CardActionData action, int index, string path, ref int errorCount, ref int warningCount)
    {
        if (action == null)
        {
            LogError(path, $"Action {index} is null.", ref errorCount);
            return;
        }

        switch (action.actionType)
        {
            case CardActionType.Damage:
                RequirePositiveAmount(action, index, path, "damage", ref errorCount);
                RequireTarget(action, index, path, ref errorCount);

                if (action.damageType == DamageType.None)
                {
                    LogWarning(path, $"Action {index} deals damage but has no damage type.", ref warningCount);
                }
                break;

            case CardActionType.Block:
                RequirePositiveAmount(action, index, path, "block", ref errorCount);
                RequireTarget(action, index, path, ref errorCount);
                break;

            case CardActionType.Heal:
                RequirePositiveAmount(action, index, path, "heal", ref errorCount);
                RequireTarget(action, index, path, ref errorCount);
                break;

            case CardActionType.Draw:
                RequirePositiveAmount(action, index, path, "draw", ref errorCount);
                break;

            case CardActionType.ApplyStatus:
                RequireTarget(action, index, path, ref errorCount);

                if (action.statusToApply == StatusType.None)
                {
                    LogError(path, $"Action {index} applies a status but status is None.", ref errorCount);
                }
                break;
        }

        if (action.conditionType != ConditionType.None && action.requiredStatus == StatusType.None)
        {
            if (action.conditionType == ConditionType.PlayerHasStatus || action.conditionType == ConditionType.EnemyHasStatus)
            {
                LogError(path, $"Action {index} has a status condition without a required status.", ref errorCount);
            }
        }
    }

    private static void RequirePositiveAmount(CardActionData action, int index, string path, string label, ref int errorCount)
    {
        if (action.amount <= 0)
        {
            LogError(path, $"Action {index} must have a positive {label} amount.", ref errorCount);
        }
    }

    private static void RequireTarget(CardActionData action, int index, string path, ref int errorCount)
    {
        if (action.target == CardTarget.None)
        {
            LogError(path, $"Action {index} needs a target.", ref errorCount);
        }
    }

    private static void LogError(string path, string message, ref int errorCount)
    {
        errorCount++;
        Debug.LogError($"{path}: {message}");
    }

    private static void LogWarning(string path, string message, ref int warningCount)
    {
        warningCount++;
        Debug.LogWarning($"{path}: {message}");
    }
}
#endif
