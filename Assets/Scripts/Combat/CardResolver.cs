using System;

public class CardResolver
{
    private readonly Unit player;
    private readonly EnemyFormation formation;
    private readonly DeckRuntime deck;
    private readonly Action<string> addLog;

    public CardResolver(Unit player, EnemyFormation formation, DeckRuntime deck, Action<string> addLog)
    {
        this.player = player;
        this.formation = formation;
        this.deck = deck;
        this.addLog = addLog;
    }

    public void Resolve(CardData card, GridEnemy selectedEnemy)
    {
        foreach (CardActionData action in card.actions)
        {
            if (action.conditionType != ConditionType.None)
            {
                continue;
            }

            ResolveAction(action, selectedEnemy);
        }
    }

    private void ResolveAction(CardActionData action, GridEnemy selectedEnemy)
    {
        switch (action.actionType)
        {
            case CardActionType.Damage:
                foreach (Unit target in TargetResolver.GetTargets(action.target, player, formation, selectedEnemy))
                {
                    int damageDealt = target.TakeDamage(action.amount);
                    addLog($"{target.UnitName} takes {damageDealt} damage.");
                }
                break;

            case CardActionType.Block:
                foreach (Unit target in TargetResolver.GetTargets(action.target, player, formation, selectedEnemy))
                {
                    target.AddBlock(action.amount);
                    addLog($"{target.UnitName} gains {action.amount} block.");
                }
                break;

            case CardActionType.Heal:
                foreach (Unit target in TargetResolver.GetTargets(action.target, player, formation, selectedEnemy))
                {
                    target.Heal(action.amount);
                    addLog($"{target.UnitName} heals {action.amount}.");
                }
                break;

            case CardActionType.Draw:
                deck.DrawCards(action.amount);
                addLog($"Drew {action.amount} card(s).");
                break;

            case CardActionType.ApplyStatus:
                addLog($"{action.statusToApply} is not implemented yet.");
                break;
        }
    }
}
