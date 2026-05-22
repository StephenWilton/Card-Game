using System.Collections.Generic;

public static class TargetResolver
{
    public static bool RequiresEnemySelection(CardData card)
    {
        foreach (CardActionData action in card.actions)
        {
            if (action.target == CardTarget.Enemy || action.target == CardTarget.PierceColumn)
            {
                return true;
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
