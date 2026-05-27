using System.Collections.Generic;

public static class TargetResolver
{
    public static bool HasEnemyEffect(CardInstance card)
    {
        return card != null && HasEnemyEffect(card.CardData);
    }

    public static bool HasEnemyEffect(CardData card)
    {
        if (card == null)
        {
            return false;
        }

        foreach (CardActionData action in card.actions)
        {
            switch (action.target)
            {
                case CardTarget.Enemy:
                case CardTarget.Both:
                case CardTarget.AllEnemies:
                case CardTarget.AllUnits:
                case CardTarget.FirstRow:
                case CardTarget.BackRow:
                case CardTarget.PierceColumn:
                    return true;
            }
        }

        return false;
    }

    public static bool RequiresEnemySelection(CardInstance card)
    {
        return card != null && RequiresEnemySelection(card.CardData);
    }

    public static bool RequiresEnemySelection(CardData card)
    {
        if (card == null)
        {
            return false;
        }

        foreach (CardActionData action in card.actions)
        {
            if (action.target == CardTarget.Enemy || action.target == CardTarget.PierceColumn)
            {
                return true;
            }
        }

        return false;
    }

    public static bool CanSelectEnemy(CardInstance card, GridEnemy enemy)
    {
        return RequiresEnemySelection(card) && enemy != null && enemy.IsAlive;
    }

    public static bool WouldAffectEnemy(CardInstance card, GridEnemy enemy, GridEnemy previewSelectedEnemy)
    {
        if (card == null || card.CardData == null || enemy == null || !enemy.IsAlive)
        {
            return false;
        }

        foreach (CardActionData action in card.CardData.actions)
        {
            switch (action.target)
            {
                case CardTarget.Enemy:
                    if (previewSelectedEnemy == enemy)
                    {
                        return true;
                    }
                    break;

                case CardTarget.Both:
                case CardTarget.AllEnemies:
                case CardTarget.AllUnits:
                    return true;

                case CardTarget.FirstRow:
                    if (enemy.Row == 0)
                    {
                        return true;
                    }
                    break;

                case CardTarget.BackRow:
                    if (enemy.Row == 1)
                    {
                        return true;
                    }
                    break;

                case CardTarget.PierceColumn:
                    if (previewSelectedEnemy != null && enemy.Column == previewSelectedEnemy.Column)
                    {
                        return true;
                    }
                    break;
            }
        }

        return false;
    }

    public static List<Unit> GetTargets(CardTarget target, Unit player, EnemyFormation formation, GridEnemy selectedEnemy)
    {
        List<Unit> targets = new List<Unit>();

        switch (target)
        {
            case CardTarget.Player:
                targets.Add(player);
                break;

            case CardTarget.Enemy:
                if (selectedEnemy != null && selectedEnemy.IsAlive)
                {
                    targets.Add(selectedEnemy.Unit);
                }
                break;

            case CardTarget.Both:
            case CardTarget.AllUnits:
                targets.Add(player);
                formation.AddAllLivingEnemies(targets);
                break;

            case CardTarget.AllEnemies:
                formation.AddAllLivingEnemies(targets);
                break;

            case CardTarget.FirstRow:
                formation.AddLivingEnemiesInRow(targets, 0);
                break;

            case CardTarget.BackRow:
                formation.AddLivingEnemiesInRow(targets, 1);
                break;

            case CardTarget.PierceColumn:
                if (selectedEnemy != null)
                {
                    formation.AddLivingEnemiesInColumn(targets, selectedEnemy.Column);
                }
                break;
        }

        return targets;
    }
}
