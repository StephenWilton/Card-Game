using System.Collections.Generic;

public class GridEnemy
{
    public Unit Unit { get; }
    public int Row { get; }
    public int Column { get; }
    public int AttackDamage { get; }
    public bool IsMiniBoss { get; }
    public EnemySpecialBehavior SpecialBehavior { get; }
    public IReadOnlyList<EnemyIntentData> Intents => intents;

    public bool IsAlive => Unit != null && !Unit.IsDead;
    public string PositionName => Row == 0 ? $"Front {Column + 1}" : $"Back {Column + 1}";
    public string IntentSummary => GetIntentSummary();
    public bool HasMirrorObserved { get; private set; }

    private readonly List<EnemyIntentData> intents = new List<EnemyIntentData>();

    public GridEnemy(
        Unit unit,
        int row,
        int column,
        int attackDamage,
        bool isMiniBoss,
        EnemySpecialBehavior specialBehavior,
        IReadOnlyList<EnemyIntentData> authoredIntents)
    {
        Unit = unit;
        Row = row;
        Column = column;
        AttackDamage = attackDamage;
        IsMiniBoss = isMiniBoss;
        SpecialBehavior = specialBehavior;

        if (authoredIntents != null)
        {
            foreach (EnemyIntentData intent in authoredIntents)
            {
                if (intent != null)
                {
                    intents.Add(intent);
                }
            }
        }

        if (intents.Count == 0)
        {
            intents.Add(new EnemyIntentData
            {
                intentType = EnemyIntentType.Attack,
                amount = attackDamage
            });
        }
    }

    public void MarkMirrorObserved()
    {
        HasMirrorObserved = true;
    }

    private string GetIntentSummary()
    {
        if (SpecialBehavior == EnemySpecialBehavior.MirrorMiniBoss)
        {
            return HasMirrorObserved ? "Mimic last turn" : "Observe";
        }

        string summary = "";

        foreach (EnemyIntentData intent in intents)
        {
            if (!string.IsNullOrEmpty(summary))
            {
                summary += " + ";
            }

            switch (intent.intentType)
            {
                case EnemyIntentType.Attack:
                    summary += $"Attack {intent.amount}";
                    break;

                case EnemyIntentType.Block:
                    summary += $"Block {intent.amount}";
                    break;

                case EnemyIntentType.ApplyStatus:
                    summary += $"{intent.statusToApply} {intent.amount}";
                    break;
            }
        }

        return summary;
    }
}
